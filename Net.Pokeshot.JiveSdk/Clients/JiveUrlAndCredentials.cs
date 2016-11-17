using System;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class JiveUrlAndCredentials: IJiveUrlAndCredentials
    {
        public string Url { get; set; }
        public NetworkCredential Credentials { get; set; }

        public JiveUrlAndCredentials()
        {
        }

        public JiveUrlAndCredentials(Uri url, string username, string password)
        {
            Url = url.ToString();
            Credentials = new NetworkCredential(username, password);
        }
    }
}