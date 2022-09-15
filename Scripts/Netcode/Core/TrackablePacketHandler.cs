using System.Dynamic;
using System.Runtime.Remoting.Channels;

using System.Collections.Generic;
using System;
using Newtonsoft.Json.Converters;
using ENet;
using Sankari.Netcode.Server;

namespace Sankari;

public class TrackablePacketHandler
{
    private HashSet<uint> seenPlayers;
    private HashSet<uint> WantedPlayers;
    private Action Callback;

    public TrackablePacketHandler(HashSet<uint> wantedPlayers, Action callback)
    {
        WantedPlayers = wantedPlayers;
        Callback = callback;
    }

    public bool OnResponse(Peer peer, APacket packet)
    {
        if (GameManager.Net.Server.Players.ContainsKey((byte)peer.ID))
        {
            WantedPlayers.Remove(peer.ID);
        }
        if (WantedPlayers.Count == 0)
        {
            Callback();
            return true;
        }
        return false;
    }
}