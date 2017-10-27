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
    public class SearchClient : JiveClient
    {
        string searchUrl { get { return JiveCommunityUrl + "/api/core/v3/search"; } }
        public SearchClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }


        /// <summary>
        /// Search for and return tags that match the specified filter criteria. When calling this endpoint, you must specify a search filter with at least one keyword.
        /// </summary>
        /// <param name="search">One or more search terms. You must escape any of the following special characters embedded in the search terms: comma (","), backslash ("\"), left parenthesis ("("), and right parenthesis (")") by preceding them with a backslash. Remember to URL encode any special character.</param>
        /// <param name="count">Maximum number of matches to return</param>
        /// <param name="startIndex">Zero-relative index of the first matching result to return</param>
        /// <param name="origin">Client that sent this search request</param>
        /// <returns></returns>
        public List<ContentTag> GetTags(List<string> search = null, int count = 25, int startIndex = 0, string origin = null)
        {
            List<ContentTag> tagList = new List<ContentTag>();

            string searchString = "";
            if (search != null && search.Count > 0)
            {
                foreach (var item in search)
                {
                    searchString += item + ",";
                }
                // remove last comma
                searchString = searchString.Remove(searchString.Length - 1);
            }

            string url = searchUrl + "/tags";
            url += "?filter=" + "search(" + searchString + ")";
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += origin != null ? "&origin=" + origin : "";

            while (true)
            {
                string json;
                try
                {
                    json = GetAbsolute(url);
                }
                catch (HttpException e)
                {
                    Console.WriteLine(e.Message);
                    switch (e.GetHttpCode())
                    {
                        case 400:
                            throw new HttpException(e.WebEventCode, "An input parameter is missing or malformed.", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is attempting to reference an object that they do not have access to.", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Requesting user is attempting to reference an object that does not exist.");
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                tagList.AddRange(results["list"].ToObject<List<ContentTag>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return tagList;
        }
    }
}
