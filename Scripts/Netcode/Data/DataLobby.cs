namespace Sankari.Netcode
{
    public class DataLobby
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ushort MaxPlayerCount { get; set; }
        public byte HostId { get; set; }
        public bool AllowJoining { get; set; }
    }
}