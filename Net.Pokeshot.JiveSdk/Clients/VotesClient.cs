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
    public class VotesClient : JiveClient
    {
        string votesUrl { get { return JiveCommunityUrl + "/api/core/v3/votes"; } }

        public VotesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
        public VotesClient(IJiveUrlAndCredentials jiveUrlAndCredentials) : base(jiveUrlAndCredentials) { }

        /// <summary>
        /// Return the number of votes cast for each poll option.
        /// </summary>
        /// <param name="contentID">ID of the poll for which to retrieve votes</param>
        /// <returns>JSON array where each entry has an option attribute and a count attribute that holds the number of votes the option received.</returns>
        public List<Vote> GetVotes(int contentID)
        {
            string url = votesUrl + "/" + contentID.ToString();

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
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified poll", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified poll does not exist", e);
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Vote>>();
        }
    }
}
