using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class VotesClient : JiveClient
    {
        string votesUrl { get { return JiveCommunityUrl + "/api/core/v3/votes"; } }

        public VotesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        public List<Votes> GetVotes(int contentID)
        {
            string url = votesUrl + "/" + contentID.ToString();
            if (fields != null && fields.Count > 0)
            {
                url += "?fields=";
                foreach (var field in fields)
                {
                    url += field + ",";
                }
                // remove last comma
                url = url.Remove(url.Length - 1);
            }

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
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified place");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified place does not exist");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<GenericPlace>();
        }
    }
}
