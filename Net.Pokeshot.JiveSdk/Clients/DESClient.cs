using System.Net;
using Net.Pokeshot.JiveSdk.Models.Dto;
using System.Collections.Generic;
using System;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class DESClient : JiveClient
    {
        static volatile string _token = null;
        static Mutex mut = new Mutex();

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
            // Jive's DES server seems to off by a few seconds. When making calls using before or after, if either is ahead of the Jive's server, we get a 400 Bad Request Error.
            // For that reason, we push back the these values by 20 seconds. It should be noted that this is problem may get resolved later or not appear on certain clients.
            before = before?.AddSeconds(-20);
            after = after?.AddSeconds(-20);

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
                        case 401: // If the token happens to have expired, try once more before giving up.
                            json = retry(url);
                            break;
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

        private string retry(string url, int numTries = 0)
        {
            int maxTry = 10;
            try
            {
                return GetAbsolute(url, getAuthorization());
            }
            catch (HttpException e)
            {
                if (e.GetHttpCode() == 401)
                {
                    if (numTries > maxTry)
                        throw;

                    return retry(url, ++numTries);
                }
                else
                    throw;
            }
        }

        /// <summary>
        /// Gets the Authorization needed for downloading data from the DES. This method is thread safe.
        /// </summary>
        /// <returns>string to be used as the authorization header for web request to the des.</returns>
        private string getAuthorization() {
            string url = "https://api.jivesoftware.com/analytics/v1/auth/login?";
            url += "clientId=" + _clientId;
            url += "&clientSecret=" + _clientSecret;

            mut.WaitOne();
            // The token expires after 30 minutes. However, if a new one is created, the old one quits working.
            if (!isValid(_token))
            {
                _token = PostAbsolute(url, "");
            }
            mut.ReleaseMutex();

            return _token;
        }

        /// <summary>
        /// Tries the token. If it works, returns true. Otherwise, returns false.
        /// </summary>
        /// <param name="_token"></param>
        /// <returns></returns>
        private bool isValid(string _token)
        {
            if (_token == null)
                return false;

            var url = _desUrl + "/activity?count=1&fields=actorID";
            try
            {
                GetAbsolute(url, _token);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
