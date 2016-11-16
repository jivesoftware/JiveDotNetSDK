using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    class PublicationsClient : JiveClient
    {
        public PublicationsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
