using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Net.Pokeshot.JiveSdk.Models;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class PeopleClient : JiveClient
    {
        string peopleUrl { get { return JiveCommunityUrl + "/api/core/v3/people"; } }
        public PeopleClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        /// <summary>
        /// Accept the terms and conditions for the authenticated user.
        /// </summary>
        /// <param name="personID">Authenticated user. User @me or the ID of the authenticated user.</param>
        public void AcceptTermsAndConditions(string personID)
        {
            string url = peopleUrl;
            url += "/" + personID.ToString() + "/acceptTermsAndConditions";

            try
            {
                PostAbsolute(url, "");
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "Specified user is not the authenticated user");
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Add expertise tag(s) to a person
        /// </summary>
        /// <param name="personID">ID of the person</param>
        /// <param name="tags">a string list of tags, limited to 200 tags per call</param>
        public void AddExpertiseTags(int personID, List<string> tags)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/expertise/endorse";

            JArray tagList = new JArray();
            foreach (var tag in tags)
            {
                tagList.Add((JToken)tag);
            }

            string json = JsonConvert.SerializeObject(tagList, Formatting.Indented);
            try
            {
                PostAbsolute(url, json);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field was malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perfrom this operation");
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist");
                    case 410:
                        throw new HttpException(e.WebEventCode, "Expertise feature is disabled");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Used to approve a tag that user has been endorsed with. The current user and the specified user must be the same or an authorization error will occur.
        /// </summary>
        /// <param name="personID">Current user ID</param>
        /// <param name="tagName">Name of the tag to approve endorsements for</param>
        public void ApproveExpertiseTag(int personID, string tagName)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/expertise/endorse/" + tagName;

            try
            {
                PutAbsolute(url, "");
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation");
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified tag or person does not exist");
                    case 410:
                        throw new HttpException(e.WebEventCode, "Expertise feature is disabled");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Create a following relationship between the specified user the specified followed user.
        /// </summary>
        /// <param name="personID">ID of the user who will be following</param>
        /// <param name="followedPersonID">ID of the user who will be followed</param>
        public void UpdateFollowing(int personID, int followedPersonID)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/@following/" + followedPersonID.ToString();

            try
            {
                PutAbsolute(url, "");
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to create this relationship");
                    case 404:
                        throw new HttpException(e.WebEventCode, "One of both of the specified users cannot be found");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Remove an expertise tag from a person.
        /// Note: backslashes are not allowed in the tagName string.
        /// </summary>
        /// <param name="personID">ID of the person</param>
        /// <param name="tagName">Name of the tag</param>
        public void DestroyExpertiseTag(int personID, string tagName)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/expertise/" + tagName;

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation");
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified tag or person does not exist");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Remove an expertise tag from a person, where the name of the tag is specified via a query string.
        /// This alternative version allows backslashes in the tag name.
        /// </summary>
        /// <param name="personID">ID of the person</param>
        /// <param name="tagName">Name of the tag</param>
        public void DestroyExpertiseTag2(int personID, string tagName)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/expertise?tagName=" + tagName;

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation");
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified tag or person does not exist");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Delete a following relationship between the specified user and the specified followed user.
        /// </summary>
        /// <param name="personID">ID of the user who is following</param>
        /// <param name="followedPersonID">ID of the user who is followed</param>
        public void DestoryFollowing(int personID, int followedPersonID)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/@following/" + followedPersonID.ToString();

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to delete this relationship");
                    case 404:
                        throw new HttpException(e.WebEventCode, "One of both of the specified users cannot be found");
                    case 409:
                        throw new HttpException(e.WebEventCode, "Following relationship does not exist between these two users");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Return the specified profile activities for the specified user.
        /// </summary>
        /// <param name="personID">ID of the user for which to return profile activities</param>
        /// <param name="time">The time representing the earliest or lates time to consider (depending on the value of "after").</param>
        /// <param name="after">If true, the request will be made for the times after "time". If false, the request is made for times before "time".</param>
        /// <param name="count">Maximum number of activities to return in this request (you may get more activities than this in order to get all of the activities in the last collection)</param>
        /// <param name="filter">Filter expression(s) used to select matching results</param>
        /// <param name="fields">Fields to be included in the returned activities</param>
        /// <returns>Activity[]</returns>
        public List<Activity> GetActivity(int personID, DateTime? time = null, bool after = false, int count = 25, List<string> filter = null, List<string> fields = null)
        {
            string url = JiveCommunityUrl + "/api/core/v3/people/";
            url += personID.ToString() + "/activities?";
            url += (after ? "after" : "before") + "=" + jiveDateFormat(time ?? DateTime.UtcNow);
            // Reset any count greater than 1000 to 1000.
            url += "&count=" + ((count > 1000) ? 1000 : count).ToString();

            if (filter != null && filter.Count > 0)
            {
                foreach (var item in filter)
                {
                    url += "&filter=" + item;
                }
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
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "Specified user ID is missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve activities for the specified user");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The activities or the specified user is not found");
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Activity>>();
        }        
        
        //GetAllPeople()
        //GetAvatar()
        //GetAvatarDeactivated()
        //GetBlog()
        //GetColleagues()
        //GetCommonBidirectional()
        //GetExtProps()
        //GetExrPropsForAddOn()
        //GetFeaturedContent()
        //GetFilterableFields()
        //GetFollowers()
        //GetFollowing()
        //GetFollowingIn()
        //GetFollowingPerson()
        //GetManager()
        //GetMetadata()
        //GetNews()
        //GetPages()
        //GetPagesPrototype()
        //GetPendingExpertiseTags()


        /// <summary>
        /// Return a list of Persons for users that match the specified criteria.
        /// </summary>
        /// <param name="ids">List of Person IDs of the individual people to be returned</param>
        /// <param name="query">List of Query strings containing search terms (or null for no search criteria)</param>
        /// <param name="startIndex">Zero-relative index of the first instance to be returned</param>
        /// <param name="count">Maximum number of instances to be returned (i.e. the page size)</param>
        /// <param name="fields">Fields to be returned (or null for summary fields)</param>
        /// <param name="sort">Optional sort to apply to the search results
        /// Sort Options: dateJoinedAsc--Sort by joined date in ascending order.
        /// dateJoinedDesc--Sort by joined date in descending order.
        /// firstNameAsc--Sort by first name in ascending order. This is the default sort order.
        /// lastNameAsc--Sort by last name in ascending order.
        /// lastProfileUpdateAsc--Sort by last profile update date/time in ascending order.
        /// lastProfileUpdateDesc--Sort by last profile update date/time in descending order.
        /// relevanceDesc--Sort by relevance, in descending order.
        /// statusLevelDesc--Sort by status level in descending order.
        /// updatedAsc--Sort by the date this person was most recently updated, in ascending order.
        /// updatedDesc--Sort by the date this person was most recently updated, in descending order.</param>
        /// <param name="company">Single value to match against the Company profile field.</param>
        /// <param name="deparment">Single value to match against the Department profile field.</param>
        /// <param name="hireDate">One or two dates in ISO-8601 format. One date indicates selection of all people hired on or after the specified date. Two dates indicates selection of all people hired between the specified dates.</param>
        /// <param name="includeDisabled">Optional boolean value indicating whether disabled users should be returned (without a filter, defaults to false).</param>
        /// <param name="includeExternal">Optional boolean value indicating whether external (non-person) users should be returned (without a filter, defaults to false).</param>
        /// <param name="includeOnline">Optional boolean value indicating whether only online users should be returned (without a filter, defaults to false).</param>
        /// <param name="includePartner">Optional boolean value indicating whether partner (external contributor) users should be returned (without a filter, defaults to true).</param>
        /// <param name="lastProfileUpdate">One or two timestamps in ISO-8601 format. If one timestamp is specified, all persons who have updated their profile since that timestamp will be selected. If two timestamps are specified, all persons who updated their profile in the specified range will be selected.</param>
        /// <param name="location">Single value to match against the Location profile field.</param>
        /// <param name="nameonly">Optional boolean value indicating whether or not to limit search results to only people that match by name. Without a filter, defaults to false.</param>
        /// <param name="published">One or two timestamps in ISO-8601 format. If one timestamp is specified, all persons created since that timestamp will be selected. If two timestamps are specified, all persons created in the specified range will be selected.</param>
        /// <param name="search">One or more search terms. You must escape any of the following special characters embedded in the search terms: comma (","), backslash ("\"), left parenthesis ("("), and right parenthesis (")") by preceding them with a backslash.</param>
        /// <param name="tag">One or more tag values. A match on any of the tags will cause this person to be returned.	</param>
        /// <param name="title">Single value to match against the Title profile field.</param>
        /// <param name="updated">One or two timestamps in ISO-8601 format. If one timestamp is specified, all persons updated since that timestamp will be selected. If two timestamps are specified, all persons updated in the specified range will be selected.</param>
        /// <returns></returns>
        public List<Person> GetPeople(List<string> ids = null, List<string> query = null, int startIndex = 0, int count = 25, List<string> fields = null,
            string sort = "firstNameAsc", string company = null, string deparment = null, Tuple<DateTime, DateTime?> hireDate = null, bool includeDisabled = false,
            bool includeExternal = false, bool includeOnline = false, bool includePartner = true, Tuple<DateTime, DateTime?> lastProfileUpdate = null, string location = null, bool nameonly = false,
            Tuple<DateTime, DateTime?> published = null, List<string> search = null, List<string> tag = null, string title = null, Tuple<DateTime, DateTime?> updated = null)
        {
            List<Person> peopleList = new List<Person>();

            List<string> filter = new List<string>();
            if (company != null)
            {
                filter.Add("company(" + company + ")");
            }
            if (deparment != null)
            {
                filter.Add("deparment(" + deparment + ")");
            }
            if (title != null)
            {
                filter.Add("title(" + title + ")");
            }
            if (hireDate != null)
            {
                string dateString = "hire-date(";
                dateString += jiveDateFormat(hireDate.Item1);
                dateString += (hireDate.Item2 != null ? ("," + jiveDateFormat(hireDate.Item2.Value)) : "") + ")";
                filter.Add(dateString);
            }
            if (lastProfileUpdate != null)
            {
                string dateString = "lastProfileUpdate(";
                dateString += jiveDateFormat(lastProfileUpdate.Item1);
                dateString += (lastProfileUpdate.Item2 != null ? ("," + jiveDateFormat(lastProfileUpdate.Item2.Value)) : "") + ")";
                filter.Add(dateString);
            }
            if (published != null)
            {
                string dateString = "published(";
                dateString += jiveDateFormat(published.Item1);
                dateString += (published.Item2 != null ? ("," + jiveDateFormat(published.Item2.Value)) : "") + ")";
                filter.Add(dateString);
            }
            if (updated != null)
            {
                string dateString = "updated(";
                dateString += jiveDateFormat(updated.Item1);
                dateString += (updated.Item2 != null ? ("," + jiveDateFormat(updated.Item2.Value)) : "") + ")";
                filter.Add(dateString);
            }
            if (includeDisabled != false)
                filter.Add("include-disabled(" + includeDisabled.ToString() + ")");
            if (includeExternal != false)
                filter.Add("include-external(" + includeExternal.ToString() + ")");
            if (includeOnline != false)
                filter.Add("include-online(" + includeOnline.ToString() + ")");
            if (includePartner != true)
                filter.Add("include-partner(" + includePartner.ToString() + ")");

            filter.Add("nameonly(" + nameonly.ToString() + ")");

            if (search != null && search.Count > 0)
            {
                string searchString = "";
                foreach (var item in search)
                {
                    searchString += item + ",";
                }
                // remove last comma
                searchString = searchString.Remove(searchString.Length - 1);
                filter.Add("search(" + searchString + ")");
            }
            if (tag != null && tag.Count > 0)
            {
                string tagString = "";
                foreach (var item in search)
                {
                    tagString += item + ",";
                }
                // remove last comma
                tagString = tagString.Remove(tagString.Length - 1);
                filter.Add("tag(" + tagString + ")");
            }


            if (ids != null && ids.Count > 0)
            {
                string idString = "&ids=";
                foreach (var id in ids)
                {
                    idString += id + ",";
                }
                // remove last comma
                idString = idString.Remove(idString.Length - 1);
            }

            string url = peopleUrl;
            url += "?sort=" + sort;
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            if (ids != null && ids.Count > 0)
            {
                url += "&ids=";
                foreach (var id in ids)
                {
                    url += id + ",";
                }
                // remove last comma
                url = url.Remove(url.Length - 1);
            }
            if (query != null)
            {
                url += "&query=" + query;
            }
            if (filter != null && filter.Count > 0)
            {
                foreach (var item in filter)
                {
                    url += "&filter=" + item;
                }
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed");
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user information");
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                peopleList.AddRange(results["list"].ToObject<List<Person>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return peopleList;
        }
        /// <summary>
        /// Return a Person describing the requested Jive user by ID.
        /// </summary>
        /// <param name="personID">ID of the requested Jive user</param>
        /// <param name="fields">Field names to be returned (default is all)</param>
        /// <returns>Person</returns>
        public Person GetPerson(int personID, List<string> fields = null)
        {
            string url = peopleUrl + "/" + personID.ToString();
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
                Console.WriteLine(e.Message);
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "Specified ID is malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user");
                    case 404:
                        throw new HttpException(e.WebEventCode, "ID does not identify a valid user");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<Person>();
        }
        /// <summary>
        /// Return a Person describing the requested Jive user by email address.
        /// </summary>
        /// <param name="email">Email address of the requested Jive user</param>
        /// <param name="fields">Field names to be returned (default is @all)</param>
        /// <returns>Person</returns>
        public Person GetPersonByEmail(string email, List<string> fields = null)
        {
            string url = peopleUrl + "/email/" + email;
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
                Console.WriteLine(e.Message);
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "Specified email address is malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user");
                    case 404:
                        throw new HttpException(e.WebEventCode, "ID does not identify a valid user");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<Person>();
        }

        //GetPersonByExternalIdentity()
        //GetPersonByUsername()
        //GetProfileFieldPrivacy()
        //GetProfileFieldsPrivacy()
        //GetProfileImage()
        //GetProfileImageData()
        //GetProfileImages()
        //GetRecognition()
        //GetRecommendedPeople()
        //GetReport()
        //GetReports()
        //GetResources()
        //GetRoles()
        //GetSecurityGroups()
        //GetSocialUsers()
        //GetStreams()
        //GetSupportedFields()
        //GetTagsUserTaggedOnUser()
        //GetTasks()
        //GetTermsAndConditions()
        //GetTopExpertise()
        //GetTrendingContent()
        //GetTrendingPeople()
        //GetTrendingPlaces()
        //GetUsersByExpertise()
        //GetWhoEndorsed()
    }
}
