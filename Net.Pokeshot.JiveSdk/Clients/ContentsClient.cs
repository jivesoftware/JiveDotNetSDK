﻿using System;
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
        }/// <summary>
         /// Return a paginated list of the possible OutcomeTypes for the children of the specified object.
         /// </summary>
         /// <param name="contentID">ID of the content object's children to get the outcome types for</param>
         /// <param name="isCreate">Is list of types for create or view</param>
         /// <param name="startIndex">Zero-relative index of the first outcome type to be returned</param>
         /// <param name="count">Maximum number of outcome types to be returned</param>
         /// <param name="fields">Fields to be returned on outcome types</param>
         /// <returns>OutcomeType[] listing the outcome types who like the specified comment</returns>
        public List<OutcomeType> GetChildOutComeTypes(int contentID, bool isCreate = true, int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<OutcomeType> outcomeList = new List<OutcomeType>();

            string url = contentUrl + "/" + contentID.ToString() + "/childOutcomeTypes";
            url += "?isCreate=" + isCreate.ToString();
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 1000 ? 1000 : count).ToString();
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

            // jive returns a paginated list, so we have to loop through all of the pages.
            while (true)
            {
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
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this comment");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified comment does not exist");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                outcomeList.AddRange(results["list"].ToObject<List<OutcomeType>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return outcomeList;
        }
        /// <summary>
        /// Return a paginated list of comments to the specified content object, optionally limiting the returned results to direct replies only. The specified content object type must support comments, or be a comment itself (in which case replies to this comment only are returned).
        /// </summary>
        /// <param name="contentID">ID of the content object for which to return comments</param>
        /// <param name="filter">The filter criteria used to select comments</param>
        /// <param name="excludeReplies">Flag indicating whether to exclude replies (and therefore return direct comments only)</param>
        /// <param name="hierarchical">Flag indicating that comments should be returned in hierarchical order instead of chronological order</param>
        /// <param name="author">Flag indicating if author comments should be returned or regular comments (only valid for documents). By default regular comments are returned.</param>
        /// <param name="inline">Flag indicating if inline comments should be returned or regular comments (only valid for binary documents). By default regular comments are returned.</param>
        /// <param name="sort">Parameter indicating the sort order of the returned comments (only valid for inline comments). By default the sort order is dateasc</param>
        /// <param name="startIndex">Zero-relative index of the first comment to be returned</param>
        /// <param name="count">Maximum number of comments to be returned</param>
        /// <param name="anchor">optional URI for a comment to anchor at. Specifying a anchor will try to return the page containing the anchor. If the anchor could not be found then the first page of comments will be returned.</param>
        /// <param name="fields">Fields to be returned in the selected comments</param>
        /// <returns>Comment[]</returns>
        public List<Comment> GetComments(int contentID, List<string> filter = null, bool excludeReplies = false, bool hierarchical = true, bool author = false,
            bool inline = false, string sort = "dateasc", int startIndex = 0, int count = 25, string anchor = null, List<string> fields = null)
        {
            List<Comment> commentList = new List<Comment>();

            string url = contentUrl + "/" + contentID.ToString() + "/comments";
            url += "?excludeReplies=" + excludeReplies.ToString();
            url += "&hierarchical=" + hierarchical.ToString();
            url += "&author=" + author.ToString();
            url += "&inline=" + inline.ToString();
            url += "&sort=" + sort;
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            if (anchor != null)
                url += "&anchor=" + anchor;
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

            // jive returns a paginated list, so we have to loop through all of the pages.
            while (true)
            {
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object or comments");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist");
                        case 501:
                            throw new HttpException(e.WebEventCode, "The specified content object is of a type that does not support comments");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                commentList.AddRange(results["list"].ToObject<List<Comment>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return commentList;
        }
        /// <summary>
        /// Return the specified content object with the specified fields.
        /// </summary>
        /// <param name="contentID">ID of the content object to be returned</param>
        /// <param name="fields">Fields to be returned</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <returns>Content containing the specified content</returns>
        public GenericContent GetContent(int contentID, List<string> fields = null, bool abridged = false)
        {
            string url = contentUrl + "/" + contentID.ToString();
            url += "?abridged=" + abridged.ToString();
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
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified content does not exist");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<GenericContent>();
        }

        // public byte[] GetContentData(int ContentID)

        /// <summary>
        /// Return a paginated list of Persons about people who are following the specified content.
        /// </summary>
        /// <param name="contentID">ID of the content object to check for followers</param>
        /// <param name="startIndex">Zero-relative index of the first instance to be returned</param>
        /// <param name="count">Maximum number of instances to be returned (i.e. the page size)</param>
        /// <param name="fields">Fields to be returned (or null for summary fields)</param>
        /// <returns>Person listing people following the specified content</returns>
        public List<Person> GetContentFollowers(int contentID, int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<Person> personList = new List<Person>();

            string url = contentUrl + "/" + contentID.ToString() + "/followers";
            url += "?startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 1000 ? 1000 : count).ToString();
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
                    switch (e.GetHttpCode())
                    {
                        case 400:
                            throw new HttpException(e.WebEventCode, "The request criteria are malformed");
                        case 403:
                            throw new HttpException(e.WebEventCode, "The requesting user is not authorize to retrieve this user information");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified user cannot be found");
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                personList.AddRange(results["list"].ToObject<List<Person>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return personList;
        }

        // GetContentFollowingIn

        /// <summary>
        /// Return a paginated list of the people who like the specified content object.
        /// </summary>
        /// <param name="contentID">ID of the content object for which to return liking people</param>
        /// <param name="startIndex">Zero-relative index of the first person to be returned</param>
        /// <param name="count">Maximum number of people to be returned</param>
        /// <param name="fields">Fields to be returned on liking people</param>
        /// <returns>Person[] listing people who like the specified content object</returns>
        public List<Person> GetContentLikes(int contentID, int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<Person> personList = new List<Person>();

            string url = contentUrl + "/" + contentID.ToString() + "/likes";
            url += "?startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 1000 ? 1000 : count).ToString();
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
                    switch (e.GetHttpCode())
                    {
                        case 400:
                            throw new HttpException(e.WebEventCode, "An input field is malformed");
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this content object");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist");
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                personList.AddRange(results["list"].ToObject<List<Person>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }

            return personList;
        }
        /// <summary>
        /// Return a list of content objects that match the specified filter criteria.
        /// </summary>
        /// <param name="creationDate">A Tuple representing the creation date range to consider when querying for Content.
        /// The first DateTime in the Tuple is after date - Creation date of content must be greater than or equal to this date. Use null to specify no restriction.
        /// The second DateTime in the Tuple is before date - Creation date of content must be less than or equal to this date. Use null to specify no restriction.
        /// This overrides any modificationDate that may be specified in filter.</param>
        /// <param name="modificationDate">A Tuple representing the modification date range to consider when querying for Content.
        /// The first DateTime in the Tuple is after date - Modification date of content must be greater than or equal to this date. Use null to specify no restriction.
        /// The second DateTime in the Tuple is before date - Modification date of content must be less than or equal to this date. Use null to specify no restriction.
        /// This overrides any creationDate that may be specified in filter.</param>
        /// <param name="filter">The filter criteria used to select content objects</param>
        /// <param name="sort">The requested sort order</param>
        /// <param name="startIndex">The zero-relative index of the first matching content to be returned</param>
        /// <param name="count">The maximum number of contents to be returned at a time.</param>
        /// <param name="fields">The fields to be returned on each content</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <param name="includeBlogs">Flag indicating that filters should include blog containers</param>
        /// <returns>Content[] of the matched content objects</returns>
        public List<GenericContent> GetContents(Tuple<DateTime, DateTime> creationDate = null, Tuple<DateTime, DateTime> modificationDate = null, int count = 25, List<string> filter = null,
            string sort = "dateCreatedDesc", int startIndex = 0, List<string> fields = null, bool abridged = false, bool includeBlogs = true)
        {
            List<GenericContent> contentList = new List<GenericContent>();

            if (filter == null)
                filter = new List<string>();

            if (creationDate != null)
            {
                if (creationDate.Item1 == null || creationDate.Item2 == null)
                {
                    filter.Add("creationDate(" + (creationDate.Item2 == null ? "null" : jiveDateFormat(creationDate.Item2)) + "," +
                        (creationDate.Item1 == null ? "null" : jiveDateFormat(creationDate.Item1) + ")"));
                }
                else {
                    DateTime earliest = (creationDate.Item1 < creationDate.Item2) ? creationDate.Item1 : creationDate.Item2;
                    DateTime latest = (earliest == creationDate.Item1) ? creationDate.Item2 : creationDate.Item1;
                    filter.Add("creationDate(" + jiveDateFormat(latest) + "," + jiveDateFormat(earliest) + ")");
                }
            }

            if (modificationDate != null)
            {
                if (modificationDate.Item1 == null || modificationDate.Item2 == null)
                {
                    filter.Add("creationDate(" + (modificationDate.Item2 == null ? "null" : jiveDateFormat(creationDate.Item2)) + "," +
                        (modificationDate.Item1 == null ? "null" : jiveDateFormat(creationDate.Item1) + ")"));
                }
                else {
                    DateTime earliest = (modificationDate.Item1 < modificationDate.Item2) ? modificationDate.Item1 : modificationDate.Item2;
                    DateTime latest = (earliest == modificationDate.Item1) ? modificationDate.Item2 : modificationDate.Item1;
                    filter.Add("modificationDate(" + jiveDateFormat(latest) + "," + jiveDateFormat(earliest) + ")");
                }
            }


            string url = contentUrl;
            url += "?sort=" + sort;
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += "&abridged=" + abridged.ToString();
            url += "&includeBlogs=" + includeBlogs.ToString();
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
                            throw new HttpException(e.WebEventCode, "An input field is malformed");
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                contentList.AddRange(results["list"].ToObject<List<GenericContent>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return contentList;
        }

        //public GetExtProps()
        //public GetExpPropsForAddon()

        /// <summary>
        /// Return a list of featured content objects that match the specified filter criteria.
        /// </summary>
        /// <param name="places">A list of URIs of places to consider when queying the featured content.
        /// (e.g. http://domain/api/core/v3/places/1006)
        /// Do not specify a place filter in the filter parameter!</param>
        /// <param name="count">The maximum number of contents to be returned</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <param name="filter">The filter criteria used to select content objects</param>
        /// <param name="fields">The fields to be returned on each content</param>
        /// <returns>Content[] of the matched content objects</returns>
        public List<GenericContent> GetFeaturedContent(List<string> places, int count = 25, bool abridged = false, List<string> filter = null, List<string> fields = null)
        {
            List<GenericContent> contentList = new List<GenericContent>();

            if (filter == null)
                filter = new List<string>();

            string placeFilter = "place(";
            foreach (var place in places)
            {
                placeFilter += place + ",";
            }
            // remove last comma
            placeFilter = placeFilter.Remove(placeFilter.Length - 1);
            placeFilter += ")";
            filter.Add(placeFilter);

            string url = contentUrl + "/featured";
            url += "?count=" + (count > 1000 ? 1000 : count).ToString();
            url += "&abridged=" + abridged.ToString();
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

            while (true)
            {
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this content object");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified place in the filter does not exist");
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                contentList.AddRange(results["list"].ToObject<List<GenericContent>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }

            return contentList;
        }

        //public GetMyEntitlements()
        //public GetOutcomes
        //public getOutcomeTypes

        /// <summary>
        /// Return a list of popular content objects for the authenticated user. Use this service when recommendation is disabled. Do a GET to /api/core/v3/metadata/properties/feature.recommender.enabled to figure out whether recommendation service is enabled or not.
        /// The returned list may contain a mixture of content entities of various types.On any given content object entity, use the type field to determine the type of that particular content.
        /// </summary>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <param name="fields">The fields to be returned on each content</param>
        /// <returns>Content[] of the matched content objects</returns>
        public List<GenericContent> GetPopularContent(bool abridged = false, List<string> fields = null)
        {
            string url = contentUrl + "/popular";
            url += "?abridged=" + abridged.ToString();
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
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<GenericContent>>();
        }

        //public GetPreviewImage()
        //public List<Content> GetRecentContent()
        //public List<Content> GetRecommendedContent()
        //public List<Content> GetTrendingContent()
        //public GetUserEntitlements()
    }
}