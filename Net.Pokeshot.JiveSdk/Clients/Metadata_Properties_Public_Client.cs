using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class Metadata_Properties_Public_Client : JiveClient
    {
        public Metadata_Properties_Public_Client(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
