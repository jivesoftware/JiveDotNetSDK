using Net.Pokeshot.JiveSdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Net.Pokeshot.JiveSdk.Clients
{
    /// <summary>
    /// Can be used to make requests to the Jive /activities endpoint
    /// </summary>
    public class ActivitiesClient : JiveClient
    {
        /// <summary>
        /// Initializes a new instance of ActivityRetriever
        /// </summary>
        /// <param name="communityUrl">The url of the targeted Jive community (e.g. https://jivecommunity.jiveon.com)</param>
        /// <param name="credential">The NetworkCredential containing the username and password for a user in the given Jive Community</param>
        public ActivitiesClient(string communityUrl, NetworkCredential credential) : base(communityUrl, credential) { }


        //TODO: add PUT request


        /// <summary>
        /// Queries Jive's Activities endpoint for all Activities after "time".
        /// </summary>
        /// <param name="time">The time representing the earliest or lates time to consider (depending on the value of "after").</param>
        /// <param name="after">If true, the request will be made for the times after "time". If false, the request is made for times before "time".</param>
        /// <param name="count">Maximum number of activities to return in this request (you may get more activities than this in order to get all of the activities in the last collection)(max is 1000)</param>
        /// <param name="filter">Filter expression(s) used to select matching results (e.g. type(likes,social,profiles,endorsements))</param>
        /// <param name="fields">The fields to be included in returned activities</param>
        /// <param name="collapse">Whether collapse the results such that there is only one entry per jive object</param>
        /// <param name="oldestUnread">Effective only when "collapse" is true. When this flag is set to true, service includes oldest unread item in collapsed list</param>
        /// <returns>The specified activities from the stream of all activity visible to the requesting person.</returns>
        public List<Activity> GetActivities(DateTime time, bool after = true, int count = 25, List<string> filter = null, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities?";
            url += (after ? "after" : "before") + "=" + jiveDateFormat(time);
            // Reset any count greater than 1000 to 1000.
            url += "&count=" + ((count > 1000) ? 1000 : count).ToString();

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

            string json;
            try
            {
                json = GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode()) {
                    case 400:
                        throw new HttpException(e.WebEventCode, "The specified stream ID is missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve activities for the specified stream");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The activities or the specified stream is not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Activity>>();
        }
        /// <summary>
        /// Queries the API for the number of Activities since "after".
        /// </summary>
        /// <param name="after">The oldest date to consider when counting the activities.</param>
        /// <param name="max">The maximum number of new activity counts to return. Default is 50.</param>
        /// <param name="exclude">Flag indicating whether activity performed by the user should be omitted. Default is false.</param>
        /// <returns>Count of new activities for the given user since the provided time</returns>
        public int GetActivitiesCount(DateTime after, int max = 50, bool exclude = false)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/count?";
            url += "after=" + jiveDateFormat(after);
            // Using max over 100000000 causes wierd errors, so we limit it to that number.
            url += "&max=" + ((max > 100000000) ? 100000000 : max).ToString();
            url += "&exclude=" + exclude.ToString();

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
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return int.Parse(results["count"].ToString());
        }
        /// <summary>
        /// Return the discovery stream, containing recommended, trending, and matters most items.
        /// </summary>
        /// <param name="fields">Fields to be included in returned place entities</param>
        /// <returns></returns>
        public List<Activity> GetDiscoveryChannel(List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/discovery";
            if (fields != null && fields.Count > 0)
            {
                url += "?fields=";
                foreach (var field in fields)
                {
                    url += fields.ToString() + ",";
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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve recently viewed places");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The places are not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Activity>>();
        }
        /// <summary>
        /// Return the most frequently viewed content that is visible to the requesting person.
        /// </summary>
        /// <param name="count">Maximum number of content entities to return in this request (bounded at 20)</param>
        /// <param name="fields">Fields to be included in returned content entities</param>
        /// <param name="filter">Filter expression(s) used to select matching results (currently supports type only). Since 3.5.</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <returns>Content[] representing frequently viewed content objects</returns>
        public List<Content> GetFrequentContent(int count = 20, List<string> fields = null, List<string> filter = null, bool abridged = false)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/frequent/content";
            url += "?count=" + count;
            if (fields != null && fields.Count > 0)
            {
                url += "&fields=";
                foreach (var field in fields)
                {
                    url += fields.ToString() + ",";
                }
                // remove last comma
                url = url.Remove(url.Length - 1);
            }
            if (filter != null && filter.Count > 0)
            {
                url += "&filter=";
                foreach (var item in filter)
                {
                    url += item.ToString() + ",";
                }
                // remove last comma
                url = url.Remove(url.Length - 1);
            }
            url += "&abridged=" + abridged.ToString();

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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve frequently viewed content");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The content is not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Content>>();
        }
        /// <summary>
        /// Return the most frequently viewed people that are visible to the requesting person.
        /// </summary>
        /// <param name="count">Maximum number of person entities to return in this request (bounded at 20)</param>
        /// <param name="fields">Fields to be included in returned person entities</param>
        /// <returns>Person[] representing frequently viewed people</returns>
        public List<Person> GetFrequentPeople(int count = 20, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/frequent/people";
            url += "?count=" + count;
            if (fields != null && fields.Count > 0)
            {
                url += "&fields=";
                foreach (var field in fields)
                {
                    url += fields.ToString() + ",";
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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve frequently viewed people");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The people are not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Person>>();
        }
        /// <summary>
        /// Return the most frequently viewed places that are visible to the requesting person.
        /// </summary>
        /// <param name="count">Maximum number of place entities to return in this request (bounded at 20)</param>
        /// <param name="fields">Fields to be included in returned place entities</param>
        /// <returns>Place[] representing frequently viewed places</returns>
        public List<JivePlace> GetFrequentPlaces(int count = 20, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/frequent/places";
            url += "?count=" + count;
            if (fields != null && fields.Count > 0)
            {
                url += "&fields=";
                foreach (var field in fields)
                {
                    url += fields.ToString() + ",";
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
                        throw new HttpException(e.WebEventCode, "The required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve frequently viewed places");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The places are not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<JivePlace>>();
        }
        /// <summary>
        /// Return the most recently viewed content that is visible to the requesting person.
        /// </summary>
        /// <param name="before">Date and time representing the maximum "last activity" timestamp. Since 3.5.</param>
        /// <param name="count">Maximum number of content entities to return in this request</param>
        /// <param name="fields">Fields to be included in returned content entities</param>
        /// <param name="filter">Filter expression(s) used to select matching results (currently supports type only)</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <returns>Content[] representing recently viewed content</returns>
        public List<Content> GetRecentContent(DateTime before, int count = 10, List<string> fields = null, List<string> filter = null, bool abridged = false)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/recent/content";
            url += "?before=" + jiveDateFormat(before);
            // Reset any count greater than 1000 to 1000.
            url += "&count=" + ((count > 1000) ? 1000 : count).ToString();
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
            url += "&abridged=" + abridged.ToString();

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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve recently viewed content");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The content is not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Content>>();
        }
        /// <summary>
        /// Return the most recently viewed people that are visible to the requesting person.
        /// </summary>
        /// <param name="count">Maximum number of person entities to return in this request</param>
        /// <param name="fields">Fields to be included in returned person entities</param>
        /// <returns>Person[] representing recently viewed people</returns>
        public List<Person> GetRecentPeople(int count = 25, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/recent/people";
            url += "?count=" + count.ToString();
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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve recently viewed people");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The people are not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Person>>();
        }
        /// <summary>
        /// Return the most recently viewed places that are visible to the requesting person.
        /// </summary>
        /// <param name="count">Maximum number of place entities to return in this request</param>
        /// <param name="fields">Fields to be included in returned place entities</param>
        /// <returns>Place[] representing recently viewed places</returns>
        public List<JivePlace> GetRecentPlaces(int count = 25, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/recent/places";
            url += "?count=" + count.ToString();
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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve recently viewed places");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The places are not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<JivePlace>>();
        }
        /// <summary>
        /// Return a paginated list of social news for the authenticated user.
        /// </summary>
        /// <param name="before">Date and time in ISO-8601 format that indicates the maximum date. By default is 'now'</param>
        /// <param name="count">The maximum number of news to be returned. By default is 5.</param>
        /// <param name="fields">The fields to be returned on each news</param>
        /// <returns>Activity[] representing social news</returns>
        public List<Activity> GetSocialNews(DateTime before, int count = 5, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/social/news";
            url += "?before=" + jiveDateFormat(before);
            url += "&count=" + ((count > 1000) ? 1000 : count).ToString();
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
                        throw new HttpException(e.WebEventCode, "The requesting user is not authorize to retrieve this information");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Activity>>();
        }
        /// <summary>
        /// Return the people most frequently interacted with in the inbox that are visible to the requesting person.
        /// </summary>
        /// <param name="count">Maximum number of person entities to return in this request (bounded at 100)</param>
        /// <param name="fields">Fields to be included in returned person entities</param>
        /// <returns>Person[] representing frequently viewed people</returns>
        public List<Person> GetSocialPeople(int count = 25, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/activities/social/people";
            url += "?count=" + ((count > 100) ? 100 : count).ToString();
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
                        throw new HttpException(e.WebEventCode, "Any required parameters are missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve frequently viewed people");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The people are not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Person>>();
        }
    }
}
