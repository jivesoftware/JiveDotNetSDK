using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class JiveUrlAndCredentials: IJiveUrlAndCredentials
    {
        public string Url { get; set; }
        public NetworkCredential Credentials { get; set; }
    }
}