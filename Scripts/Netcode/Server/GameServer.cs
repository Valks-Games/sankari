using ENet;

namespace Sankari.Netcode.Server;

using Event = ENet.Event;

public class GameServer : ENetServer
{
    /// <summary>
    /// This property is not thread safe
    /// </summary>
    public Dictionary<byte, DataPlayer> Players = new();

    /// <summary>
    /// This property is not thread safe
    /// </summary>
    public uint HostId { get; set; }

    /// <summary>
    /// This property is not thread safe
    /// </summary>
    public STimer LevelUpdateLoop { get; }

    public GameServer()
    {
        LevelUpdateLoop = new STimer(NetIntervals.HEARTBEAT, () =>
        {
            // ImHost needs to only see OtherClient position (and all other client positions)
            // OtherClient needs to only see ImHost position (and all other client positions)

            // Lets say 'player' = ImHost
            foreach (var player in Players)
            {
                // [ImHost, OtherClient]
                var playerPositions = new Dictionary<byte, DataPlayer>(Players).ToDictionary(x => x.Key, x => x.Value.Position);

                // RemoveAt ImHost from the list of player positions
                // [OtherClient]
                playerPositions.Remove(player.Key);

                // Send OtherClient position to everyone but ImHost
                // Sending OtherClient position (and every other position) to ImHost
                if (playerPositions.Count != 0)
                    Send(ServerPacketOpcode.PlayerPositions, new SPacketPlayerPositions
                    {
                        PlayerPositions = playerPositions
                    }, player.Key);
            }
        }, false);
    }

    /// <summary>
    /// This method is not thread safe
    /// </summary>
    private Dictionary<byte, DataPlayer> GetOtherPlayers(byte id)
    {
        var otherPlayers = new Dictionary<byte, DataPlayer>(Players);
        otherPlayers.Remove(id);
        return otherPlayers;
    }

    /// <summary>
    /// This method is not thread safe
    /// </summary>
    private Peer[] GetOtherPlayerPeers(uint id) => Players.Keys.Where(x => x != id).Select(x => Peers[x]).ToArray();

    /// <summary>
    /// This method is not thread safe
    /// </summary>
    private Peer[] GetAllPlayerPeers() => Players.Keys.Select(x => Peers[x]).ToArray();

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public void SendToAllPlayers(ServerPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        ENetCmds.Enqueue(new ENetServerCmd(ENetServerOpcode.SendPackets, new ENetSend
        {
            ENetSendType = ENetSendType.Everyone,
            ServerPacketOpcode = opcode,
            PacketData = data,
            PacketFlags = flags
        }));
    }

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public void SendToEveryoneButHost(ServerPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        ENetCmds.Enqueue(new ENetServerCmd(ENetServerOpcode.SendPackets, new ENetSend
        {
            ENetSendType = ENetSendType.EveryoneExcludingHost,
            ServerPacketOpcode = opcode,
            PacketData = data,
            PacketFlags = flags
        }));
    }

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public void SendToOtherPlayers(uint id, ServerPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        ENetCmds.Enqueue(new ENetServerCmd(ENetServerOpcode.SendPackets, new ENetSend
        {
            ENetSendType = ENetSendType.EveryoneExcludingSomeone,
            ExcludedPeerId = id,
            ServerPacketOpcode = opcode,
            PacketData = data,
            PacketFlags = flags
        }));
    }

    protected override void ServerCmds()
    {
        while (ENetCmds.TryDequeue(out ENetServerCmd cmd))
        {
            switch (cmd.Opcode)
            {
                case ENetServerOpcode.Stop:
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        Log("Server is in the middle of stopping");
                        break;
                    }

                    KickAll(DisconnectOpcode.Stopping);
                    CancellationTokenSource.Cancel();
                    break;

                case ENetServerOpcode.Restart:
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        Log("Server is in the middle of restarting");
                        break;
                    }

                    KickAll(DisconnectOpcode.Restarting);
                    CancellationTokenSource.Cancel();
                    QueueRestart = true;
                    break;

                case ENetServerOpcode.SendPackets:
                    var enetSendData = (ENetSend)cmd.Data;

                    var opcode = enetSendData.ServerPacketOpcode;
                    var data = enetSendData.PacketData;
                    var flags = enetSendData.PacketFlags;

                    switch (enetSendData.ENetSendType)
                    {
                        case ENetSendType.Everyone:

                            var allPlayers = GetAllPlayerPeers();

                            if (data == null)
                                Send(opcode, flags, default(Peer), allPlayers);
                            else
                                Send(opcode, data, flags, default(Peer), allPlayers);

                            break;

                        case ENetSendType.EveryoneExcludingHost:

                            var otherPeers = GetOtherPeers(HostId);
                            if (otherPeers.Length == 0)
                                return;

                            if (data == null)
                                Send(opcode, flags, default(Peer), otherPeers);
                            else
                                Send(opcode, data, flags, default(Peer), otherPeers);

                            break;

                        case ENetSendType.EveryoneExcludingSomeone:

                            var otherPlayers = GetOtherPlayerPeers(enetSendData.ExcludedPeerId);
                            if (otherPlayers.Length == 0)
                                return;

                            if (data == null)
                                Send(opcode, flags, default(Peer), otherPlayers);
                            else
                                Send(opcode, data, flags, default(Peer), otherPlayers);

                            break;
                    }

                    break;
            }
        }
    }

    protected override void Started(ushort port, int maxClients)
    {
        Log($"Server listening on port {port}");
    }

    protected override void Connect(ref Event netEvent)
    {
        Log($"Client connected with id: {netEvent.Peer.ID}");
    }

    protected override void Received(Peer peer, PacketReader packetReader, ClientPacketOpcode opcode)
    {
        var logOpcode = true;

        if (GameManager.Linker.IgnoreOpcodesFromClient != null)
            foreach (var dontLogOpcode in GameManager.Linker.IgnoreOpcodesFromClient)
                if (opcode == dontLogOpcode)
                {
                    logOpcode = false;
                    break;
                }

        if (logOpcode)
            Log($"Received: {opcode}");

        if (!HandlePacket.ContainsKey(opcode))
        {
            Logger.LogWarning($"[Server] Received malformed opcode: {opcode} (Ignoring)");
            return;
        }

        var handlePacket = HandlePacket[opcode];
        try
        {
            handlePacket.Read(packetReader);
        }
        catch (System.IO.EndOfStreamException e)
        {
            Logger.LogWarning($"[Server] Received malformed opcode: {opcode} {e.Message} (Ignoring)");
            return;
        }
        handlePacket.Handle(peer);
    }

    protected override void Disconnect(ref Event netEvent)
    {
        Log($"Client disconnected with id: {netEvent.Peer.ID}");
    }

    protected override void Timeout(ref Event netEvent)
    {
        Log($"Client timed out with id: {netEvent.Peer.ID}");
    }

    protected override void Leave(ref Event netEvent)
    {
        var username = Players[(byte)netEvent.Peer.ID].Username;

        SendToOtherPlayers(netEvent.Peer.ID, ServerPacketOpcode.GameInfo, new SPacketGameInfo
        {
            ServerGameInfo = ServerGameInfo.PlayerJoinLeave,
            Username = username,
            Joining = false,
            Id = (byte)netEvent.Peer.ID
        });

        Players.Remove((byte)netEvent.Peer.ID);

        Log($"Player with '{username}' left");
    }

    protected override void Stopped()
    {
        Log("Server stopped");
    }

    /// <summary>
    /// This method is thread safe
    /// </summary>
    public void Log(object obj) => Logger.Log($"[Server] {obj}", ConsoleColor.Green);
}
