using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class WebhooksClient : JiveClient
    {
        public WebhooksClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
