using Net.Pokeshot.JiveSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Net.Pokeshot.JiveSdk.Retrievers
{
    public class ActivityRetriever : JiveRetriever
    {
        public ActivityRetriever(string communityUrl, NetworkCredential credential) : base(communityUrl, credential) { }

        /// <summary>
        /// Queries Jive's Activities endpoint for all Activities after "time".
        /// </summary>
        /// <param name="time">The time representing the earliest or lates time to consider (depending on the value of "after").</param>
        /// <param name="after">If true, the request will be made for the times after "time". If false, the request is made for times before "time".</param>
        /// <param name="count">Maximum number of activities to return in this request (you may get more activities than this in order to get all of the activities in the last collection)(max is 1000)</param>
        /// <param name="filters">Filter expression(s) used to select matching results (e.g. type(likes,social,profiles,endorsements))</param>
        /// <param name="fields">The fields to be included in returned activities</param>
        /// <param name="collapse">Whether collapse the results such that there is only one entry per jive object</param>
        /// <param name="oldestUnread">Effective only when "collapse" is true. When this flag is set to true, service includes oldest unread item in collapsed list</param>
        public List<Activity> GetActivities(DateTime time, bool after = true, int count = 25, List<string> filters = null, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities?";
            url += (after ? "after" : "before") + "=" + time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "%2B0000";
            // Reset any count greater than 1000 to 1000.
            url += "&count=" + ((count > 1000) ? 1000 : count).ToString();

            if (filters != null && filters.Count > 0)
            {
                url += "&filter=";
                foreach (var filter in filters)
                {
                    url += filter + ",";
                }
            }

            if (fields != null && fields.Count > 0)
            {
                url += "&fields=";
                foreach (var field in fields)
                {
                    url += field + ",";
                }
            }

            string activitiesJson = ExecuteAbsolute(url);
            JObject results = JObject.Parse(activitiesJson);

            return results["list"].ToObject<List<Activity>>();
        }
        /// <summary>
        /// Queries the API for the number of Activities since "after".
        /// </summary>
        /// <param name="after">The oldest date to consider when counting the activities.</param>
        /// <param name="max">The maximum number of new activity counts to return. Default is 50.</param>
        /// <param name="exclude">Flag indicating whether activity performed by the user should be omitted. Default is false.</param>
        /// <returns></returns>
        public int GetActivitiesCount(DateTime after, int max = 50, bool exclude = false)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/count?";
            url += "after=" + after.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "%2B0000";
            // Using max over 100000000 causes wierd errors, so we limit it to that number.
            url += "&max=" + ((max > 100000000) ? 100000000 : max).ToString();
            url += "&exclude=" + exclude.ToString();

            string activitiesCountJson = ExecuteAbsolute(url);
            JObject results = JObject.Parse(activitiesCountJson);

            return int.Parse(results["count"].ToString());
        }
    }
}
