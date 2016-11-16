using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    class AddOnsClient : JiveClient
    {
        public AddOnsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
