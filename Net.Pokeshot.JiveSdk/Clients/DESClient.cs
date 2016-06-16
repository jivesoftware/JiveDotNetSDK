using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class DESClient : JiveClient
    {
        string contentUrl { get { return "https://api.jivesoftware.com/analytics/v2/export"; } }
        string _clientId;
        string _clientSecret;

        public DESClient(string communityUrl, NetworkCredential credentials, string clientId, string clientSecret) : base(communityUrl, credentials) {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

    }
}
