namespace Sankari.Netcode
{
    public partial class DataPlayer : DataTransform 
    {
        public string Username { get; set; }
        public bool Ready { get; set; }
        public bool Host { get; set; }
    }
}