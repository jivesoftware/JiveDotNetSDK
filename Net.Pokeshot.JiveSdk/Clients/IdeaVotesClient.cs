using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Net.Pokeshot.JiveSdk.Models;
using System.Web;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class IdeaVotesClient : JiveClient
    {
        string ideaVotesUrl { get { return JiveCommunityUrl + "/api/core/ext/idea-type-plugin/v1/ideaVotes"; } }

        public IdeaVotesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        /// <summary>
        /// Cast a vote on the specified idea, replacing any previous vote by the requesting person. The incoming JSON must include a boolean "promote" field
        /// that is true if the requestor is promoting this idea, or false if the requestor is demoting it.
        /// </summary>
        /// <param name="contentID">The ID of the content object for which to cast a "promote" vote</param>
        /// <param name="idea_vote">The vote entity containing the promote field (assumed to be true if not present)</param>
        public void CreateVote(int contentID, IdeaVote idea_vote)
        {
            string url = ideaVotesUrl;
            url += "/" + contentID.ToString();

            string json = JsonConvert.SerializeObject(idea_vote, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            try
            {
                PostAbsolute(url, json);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "If any input field is malformed, or the ID does not specify an idea", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "If you are not allowed to access the specified content object", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "If the specified content object does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "You attempted to vote on an issue for which voting has been disabled", e);
                    case 410:
                        throw new HttpException(e.WebEventCode, "If this Jive instance is not licensed for the Ideation module", e);
                }
            }

            return;
        }

        /// <summary>
        /// Return a paginated list of votes on the specified idea.
        /// </summary>
        /// <param name="contentID">The ID of the idea content object for which to retrieve votes</param>
        /// <param name="count">The number of votes to be retrieved per Jive HTTP request</param>
        /// <param name="startIndex">The zero-relative index of the first vote to be retrieved</param>
        /// <param name="fields">The names of the fields to be returned</param>
        /// <returns>a List of IdeaVotes representing the votes on the specified idea</returns>
        public List<IdeaVote> GetVotes(int contentID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<IdeaVote> voteList = new List<IdeaVote>();

            //construcs the correct url based on the user's specifications
            string url = ideaVotesUrl;
            url += "/" + contentID.ToString();
            url += "?count=" + (count > 1000 ? 1000 : count).ToString();
            if (startIndex != 0)
            {
                url += "&startIndex=" + startIndex.ToString();
            }
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
            JObject results;
            while (true)
            {
                try
                {
                    json = GetAbsolute(url);
                }
                catch (HttpException e)
                {
                    switch (e.GetHttpCode())
                    {
                        case 400:
                            throw new HttpException(e.WebEventCode, "If any input field is malformed, or the ID does not specify an idea", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "If you are not allowed to access the specified content object", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "If the specified content object does not exist", e);
                        case 410:
                            throw new HttpException(e.WebEventCode, "If this Jive instance is not licensed for the Ideation module", e);
                        default:
                            throw;
                    }
                }

                results = JObject.Parse(json);
                voteList.AddRange(results["list"].ToObject<List<IdeaVote>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }

            return voteList;
        }
    }
}
