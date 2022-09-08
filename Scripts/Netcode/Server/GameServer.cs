using ENet;

namespace Sankari.Netcode.Server;

using Event = ENet.Event;

public class GameServer : ENetServer
{
    public Dictionary<byte, DataPlayer> Players = new();
    public DataLobby Lobby { get; set; }

    public GameServer(Net networkManager) : base(networkManager) { }

    public Dictionary<byte, DataPlayer> GetOtherPlayers(byte id)
    {
        var otherPlayers = new Dictionary<byte, DataPlayer>(Players);
        otherPlayers.Remove(id);
        return otherPlayers;
    }

    public Peer[] GetOtherPlayerPeers(uint id) => Players.Keys.Where(x => x != id).Select(x => Peers[x]).ToArray();

    public Peer[] GetAllPlayerPeers() => Players.Keys.Select(x => Peers[x]).ToArray();

    public void SendToAllPlayers(ServerPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        var allPlayers = GetAllPlayerPeers();

        if (data == null)
            Send(opcode, flags, default(Peer), allPlayers);
        else
            Send(opcode, data, flags, default(Peer), allPlayers);
    }

    public void SendToOtherPeers(uint id, ServerPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        var otherPeers = GetOtherPeers(id);
        if (otherPeers.Length == 0)
            return;

        if (data == null)
            Send(opcode, flags, default(Peer), otherPeers);
        else
            Send(opcode, data, flags, default(Peer), otherPeers);
    }

    public void SendToOtherPlayers(uint id, ServerPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        var otherPlayers = GetOtherPlayerPeers(id);
        if (otherPlayers.Length == 0)
            return;

        if (data == null)
            Send(opcode, flags, default(Peer), otherPlayers);
        else
            Send(opcode, data, flags, default(Peer), otherPlayers);
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
                    queueRestart = true;
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
        Log($"Received: {opcode}");

        if (!HandlePacket.ContainsKey(opcode))
        {
            Logger.LogWarning($"[Server]: Received malformed opcode: {opcode} (Ignoring)");
            return;
        }

        var handlePacket = HandlePacket[opcode];
        try
        {
            handlePacket.Read(packetReader);
        }
        catch (System.IO.EndOfStreamException e)
        {
            Logger.LogWarning($"[Server]: Received malformed opcode: {opcode} {e.Message} (Ignoring)");
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

        SendToOtherPlayers(netEvent.Peer.ID, ServerPacketOpcode.PlayerJoinLeave, new SPacketPlayerJoinLeave 
        {
            Username = username,
            Joining = false
        });

        Players.Remove((byte)netEvent.Peer.ID);

        Log($"Player with '{username}' left");
    }

    protected override void Stopped()
    {
        Log("Server stopped");
    }

    public void Log(object obj) => Logger.Log($"[Server]: {obj}", ConsoleColor.Green);
}
