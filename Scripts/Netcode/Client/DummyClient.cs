namespace Sankari.Netcode.Client;

public partial class DummyClient : ENetClient
{
    public DummyClient(Net networkManager) : base(networkManager)
    { }

    protected override void Sent(ClientPacketOpcode opcode)
    {
        networkManager.PingSent = DateTime.Now;
    }
}
