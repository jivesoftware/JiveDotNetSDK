using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class Admin_ProfileFields_Client : JiveClient
    {
        public Admin_ProfileFields_Client(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
    }
}
