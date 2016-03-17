using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using Net.Pokeshot.JiveSdk.Models;
using System.Web;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class ContentsClient : JiveClient
    {
        string contentUrl { get { return JiveCommunityUrl + "/api/core/v3/contents"; } }

        public ContentsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        /// <summary>
        /// Retrieve the abuse reports for the specified content
        /// </summary>
        /// <param name="contentID">ID of the content marked as abusive</param>
        /// <param name="fields">Fields to be returned in the abuse report response</param>
        /// <returns>AbuseReport[] containing the abuse reports for the specified content</returns>
        public List<AbuseReport> GetAbuseReports(int contentID, List<string> fields = null)
        {
            string url = contentUrl + "/" + contentID.ToString() + "/abuseReports";
            if (fields != null && fields.Count > 0)
            {
                url += "?fields=";
                foreach (var field in fields)
                {
                    url += field + ",";
                }
            }

            string json;
            try
            {
                json = GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.WebEventCode)
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or mark this content as abusive");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified content does not exist");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<AbuseReport>>();
        }
        //public List<OutcomeType> GetChildOutComeTypes
    }
}
