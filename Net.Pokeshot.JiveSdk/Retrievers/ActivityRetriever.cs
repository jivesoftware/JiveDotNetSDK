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
    partial class JiveRetriever
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">The time representing the earliest or lates time to consider (depending on the value of "after").</param>
        /// <param name="after">If true, the request will be made for the times after "time". If false, the request is made for times before "time".</param>
        /// <param name="count">Maximum number of activities to return in this request (you may get more activities than this in order to get all of the activities in the last collection)(max is 1000)</param>
        /// <param name="filters">Filter expression(s) used to select matching results (e.g. type(likes,social,profiles,endorsements))</param>
        /// <param name="fields">The fields to be included in returned activities</param>
        /// <param name="collapse">Whether collapse the results such that there is only one entry per jive object</param>
        /// <param name="oldestUnread">Effective only when "collapse" is true. When this flag is set to true, service includes oldest unread item in collapsed list</param>
        public List<Activity> getActivities(DateTime time, bool after = true, int count = 1000, List<string> filters = null, List<string> fields = null)
        {
            string activitiesUrl = JiveCommunityUrl + "/api/core/v3/activities?";
            activitiesUrl += "&" + (after ? "after" : "before") + "=" + time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "%2B0000";
            // Reset any count greater than 1000 to 1000.
            activitiesUrl += "&count=" + ((count > 1000) ? 1000 : count);

            if (filters.Count > 0)
                activitiesUrl += "&filter=";
            foreach (var filter in filters)
            {
                activitiesUrl += filter + ",";
            }

            if (fields.Count > 0)
                activitiesUrl += "&fields=";
            foreach (var field in fields)
            {
                activitiesUrl += field + ",";
            }

            string activitiesJson = ExecuteAbsolute(activitiesUrl);
            JObject results = JObject.Parse(activitiesJson);

            return results["list"].ToObject<List<Activity>>();
        }
    }
}
