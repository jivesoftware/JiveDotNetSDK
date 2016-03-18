using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    class PeopleClient : JiveClient
    {
        public PeopleClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
