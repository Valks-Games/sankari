namespace Sankari.Netcode.Client;

public class DummyClient : ENetClient
{
    protected override void Sent(ClientPacketOpcode opcode)
    {
        Net.PingSent = DateTime.Now;
    }
}
