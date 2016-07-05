using System.Net;
using Net.Pokeshot.JiveSdk.Models.Dto;
using System.Collections.Generic;
using System;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class DESClient : JiveClient
    {
        string _desUrl { get { return "https://api.jivesoftware.com/analytics/v2/export"; } }
        string _clientId;
        string _clientSecret;

        public DESClient(string communityUrl, NetworkCredential credentials, string clientId, string clientSecret) : base(communityUrl, credentials) {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public List<JiveDEAActivityInstance> GetActivity(List<string> filter = null, int count = 100, List<string> fields = null, DateTime? before = null, DateTime? after = null,
            bool showAll = false)
        {
            List<JiveDEAActivityInstance> activityList = new List<JiveDEAActivityInstance>();

            string url = _desUrl + "/activity";
            url += "?count=" + (count > 1000 ? 1000 : count).ToString();
            url += "&show-all=" + showAll;
            if (after != null)
                url += "&after=" + jiveDateFormat(after.Value);
            if (before != null)
                url += "&before=" + jiveDateFormat(before.Value);
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
            if (filter != null && filter.Count > 0)
            {
                foreach (var item in filter)
                {
                    url += "&filter=" + item;
                }
            }

            // jive returns a paginated list, so we have to loop through all of the pages.
            while (true)
            {
                string json;
                try
                {
                    json = GetAbsolute(url, getAuthorization());
                }
                catch (HttpException e)
                {
                    switch (e.GetHttpCode())
                    {
                        case 400:
                            throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this", e);
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);


                activityList.AddRange(results["list"].ToObject<List<JiveDEAActivityInstance>>());

                if (results["paging"] == null || results["paging"]["next"] == null)
                    break;
                else
                    url = results["paging"]["next"].ToString();
            }
            return activityList;
        }


        /// <summary>
        /// Gets the Authorization needed for downloading data from the DES.
        /// </summary>
        /// <returns>string to be used as the authorization header for web request to the des.</returns>
        private string getAuthorization() {
            string url = "https://api.jivesoftware.com/analytics/v1/auth/login?";
            url += "clientId=" + _clientId;
            url += "&clientSecret=" + _clientSecret;

            return PostAbsolute(url, "");
        }
    }
}
