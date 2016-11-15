using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class TileDefsClient : JiveClient
    {
        public TileDefsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
