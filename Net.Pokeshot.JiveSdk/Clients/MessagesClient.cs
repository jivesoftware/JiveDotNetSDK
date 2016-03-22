using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Net.Pokeshot.JiveSdk.Models;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class MessagesClient : JiveClient
    {
        string messagesUrl { get { return JiveCommunityUrl + "/api/core/v3/messages"; } }

        public MessagesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        public List<Message> GetContentReplies(int contentID, int count = 25, bool excludeReplies = false, List<string> filter = null, bool hierarchical = true,
            int startIndex = 0, string anchor = null, List<string> fields = null)
        {
            List<Message> messageList = new List<Message>();

            string url = messagesUrl + "/contents/" + contentID.ToString();
            url += "?excludeReplies=" + excludeReplies.ToString();
            url += "&hierarchical=" + hierarchical.ToString();
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += "&hierarchical=" + hierarchical.ToString();
            if (anchor != null)
                url += "&anchor=" + anchor;
            if (filter != null && filter.Count > 0)
            {
                url += "&filter=";
                foreach (var item in filter)
                {
                    url += item + ",";
                }
                // remove last comma
                url = url.Remove(url.Length - 1);
            }
            if (fields != null && fields.Count > 0)
            {
                url += "&fields=";
                foreach (var field in fields)
                {
                    url += field + ",";
                }
                // remove last comma
                url = url.Remove(url.Length - 1);
            }

            // jive returns a paginated list, so we have to loop through all of the pages.
            while (true)
            {
                string json;
                try
                {
                    json = GetAbsolute(url);
                }
                catch (HttpException e)
                {
                    switch (e.GetHttpCode())
                    {
                        case 400:
                            throw new HttpException(e.WebEventCode, "An input field is malformed");
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object or its messages");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                messageList.AddRange(results["list"].ToObject<List<Message>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return messageList;
        }
    }
}
