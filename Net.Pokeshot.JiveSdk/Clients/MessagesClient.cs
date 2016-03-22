using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Net.Pokeshot.JiveSdk.Models;

namespace Net.Pokeshot.JiveSdk.Clients
{
    class MessagesClient : JiveClient
    {
        string messagesUrl { get { return JiveCommunityUrl + "/api/core/v3/contents"; } }

        public MessagesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        public List<Mess GetContentReplies()

    }
}
