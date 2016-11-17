using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public interface IJiveUrlAndCredentials
    {
        string Url { get; }
        NetworkCredential Credentials { get; }
    }
}