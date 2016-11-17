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
    public class CommentsClient : JiveClient
    {
        string commentUrl { get { return JiveCommunityUrl + "/api/core/v3/comments"; } }

        public CommentsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
        public CommentsClient(IJiveUrlAndCredentials jiveUrlAndCredentials) : base(jiveUrlAndCredentials) { }

        /// <summary>
        /// Register that the requesting person considers the specified comment as abusive.
        /// </summary>
        /// <param name="commentID">ID of the comment to be marked as abusive</param>
        /// <param name="abuse_report">AbuseReport object containing the abuse report information</param>
        /// <returns>AbuseReport object containing the newly created abuse report</returns>
        public AbuseReport CreateAbuseReport(int commentID, AbuseReport abuse_report)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/abuseReports";

            string json = JsonConvert.SerializeObject(abuse_report, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            string result = "";
            try
            {
                result = PostAbsolute(url, json); //makes the HTTP request
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or mark this content as abusive", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified content does not exist", e);
                    default:
                        throw;
                }
            }

            JObject Json = JObject.Parse(result);
            return Json.ToObject<AbuseReport>();
        }

        /// <summary>
        /// Create a new comment with the specified characteristics. The parent field must contain the URI of either a content object for which this is a direct reply,
        /// or the URI of a previous comment to which this is a reply.
        /// </summary>
        /// <param name="new_comment">Comment object describing the comment to be created</param>
        /// <param name="author">Flag indicating if new comment is an author comment or a regular comment (only valid for documents). By default a regular comment will be created.</param>
        /// <param name="published">Date and time when this content object was originally created. Set 'updated' param as well. Only set this field when importing content.</param>
        /// <param name="updated">Date and time when this content object was most recently updated. Set 'published' param as well. Only set this field when importing content.</param>
        /// <param name="fields">Fields to be included in the returned Comment object</param>
        /// <returns>Comment object representing the created comment</returns>
        public Comment CreateComment(Comment new_comment, bool author = false, DateTime? published = null, DateTime? updated = null, List<string> fields = null)
        {
            //adds the query strings to the url if present
            string url = commentUrl;
            url += "?author=" + author.ToString();
            if (published != null)
            {
                url += "&published=" + jiveDateFormat((DateTime)published);
            }
            if (updated != null)
            {
                url += "&updated=" + jiveDateFormat((DateTime)updated);
            }
            if (fields != null && fields.Count > 0)
            {
                url += "&fields=";

                foreach (var field in fields)
                {
                    url += field + ",";
                }
                //remove last comma
                url = url.Remove(url.Length - 1);
            }

            string json = JsonConvert.SerializeObject(new_comment, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
            string result = "";
            try
            {
                result = PostAbsolute(url, json); //makes the HTTP request
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified parent object (or comment) could not be found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Attempt to add a comment to a content object that does not support them, or for which comments have been closed", e);
                }
            }

            return JsonConvert.DeserializeObject<Comment>(result);

            //JObject results = JObject.Parse(json); //this statement has been observed to have the possibility of dropping fields from the returned object

            //return results.ToObject<Comment>();
        }

        /// <summary>
        /// Register that the requesting person considers the specified comment helpful.
        /// </summary>
        /// <param name="commentID">ID of the comment to be marked as helpful</param>
        public void CreateCommentHelpful(int commentID)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/helpful";

            try
            {
                GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or mark this comment as helpful", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Register that the requesting person likes the specified comment.
        /// </summary>
        /// <param name="commentID">ID of the comment to be liked</param>
        public void CreateCommentLike(int commentID)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/likes";

            try
            {
                GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or like this comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Register that the requesting person considers the specified comment unhelpful.
        /// </summary>
        /// <param name="commentID">ID of the comment to be marked as unhelpful</param>
        public void CreateCommentUnhelpful(int commentID)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/unhelpful";

            try
            {
                GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or mark this comment as unhelpful", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Create a new comment as a reply to an existing comment with the specified characteristics.
        /// </summary>
        /// <param name="commentID">ID of the comment being replied to</param>
        /// <param name="new_comment">Comment object describing the reply comment to be created</param>
        /// <param name="fields">Fields to include in the returned Comment object</param>
        /// <param name="published">Date and time when this content object was originally created. Set 'updated' param as well. Only set this field when importing content. Since 3.6.</param>
        /// <param name="updated">Date and time when this content object was most recently updated. Set 'published' param as well. Only set this field when importing content. Since 3.6.</param>
        /// <returns>Comment object describing the new reply comment</returns>
        public Comment CreateReply(int commentID, Comment new_comment, List<string> fields = null, DateTime? published = null, DateTime? updated = null)
        {
            //adds the query strings to the url if present
            bool first = true;
            string url = commentUrl + "/" + commentID.ToString() + "/comments";
            if (published != null)
            {
                url += "?published=" + jiveDateFormat(published.Value);
                first = false;
            }
            if (updated != null)
            {
                if (first)
                {
                    url += "?updated=";
                    first = false;
                }
                else url += "&updated=";

                url += jiveDateFormat(updated.Value);
            }
            if (fields != null && fields.Count > 0)
            {
                if (first)
                {
                    url += "?fields=";
                    first = false;
                }
                url += "&fields=";

                foreach (var field in fields)
                {
                    url += field + ",";
                }
                //remove last comma
                url = url.Remove(url.Length - 1);
            }

            string json = JsonConvert.SerializeObject(new_comment, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
            string result = "";
            try
            {
                result = PostAbsolute(url, json); //makes the HTTP request
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified parent object (or comment) could not be found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Attempt to add a comment to a content object that does not support them, or for which comments have been closed", e);
                    default:
                        throw;
                }
            }

            return JsonConvert.DeserializeObject<Comment>(result);

            //JObject results = JObject.Parse(json); //this statement has been observed to have the possibility of dropping fields from the returned object

            //return results.ToObject<Comment>();
        }

        /// <summary>
        /// Delete the specified comment and its sub comments recursively
        /// </summary>
        /// <param name="commentID">ID of the comment to be deleted</param>
        /// <param name="recursiveDelete">Flag indicating if the delete is recursive. The flag defaults to true if not provided</param>
        public void DestroyComment(int commentID, bool recursiveDelete = true)
        {
            string url = commentUrl + "/" + commentID.ToString();
            url += "?recursiveDelete=" + recursiveDelete.ToString();

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                }
            }
        }

        /// <summary>
        /// Delete the helpful mark of the specified comment by the requesting user.
        /// </summary>
        /// <param name="commentID">ID of the comment for which a like is being removed</param>
        public void DestroyCommentHelpful(int commentID)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/helpful";

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or remove the helpful mark for this comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "You do not currently have a helpful mark registered for this comment", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Delete the like of the specified content object by the requesting user.
        /// </summary>
        /// <param name="commentID">ID of the comment for which a like is being removed</param>
        public void DestroyCommentLike(int commentID)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/likes";

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access or unlike this comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "You do not currently have a like registered for this comment", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Delete the registration of the specified comment as unhelpful by the requesting user.
        /// </summary>
        /// <param name="commentID">ID of the comment for which an unhelpful registration is being removed</param>
        public void DestroyCommentUnhelpful(int commentID)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/unhelpful";

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access this comment or remove the registration of this comment as unhelpful", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "You do not currently have an unhelpful mark registered for this comment", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Retrieve the abuse reports for the specified comment
        /// </summary>
        /// <param name="commentID">ID of the comment marked as abusive</param>
        /// <param name="fields">Fields to be returned in the abuse report response</param>
        /// <returns>a list of AbuseReport objects containing abuse reports for the specified content</returns>
        public List<AbuseReport> GetAbuseReports(int commentID, List<string> fields = null)
        {
            List<AbuseReport> reportList = new List<AbuseReport>();

            string url = commentUrl + "/" + commentID.ToString() + "/abuseReports";
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

            //Jive returns a paginated list, so it loops through as many times as necessary
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
                            throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access these contents", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist", e);
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                reportList.AddRange(results["list"].ToObject<List<AbuseReport>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }

            return reportList;
        }

        /// <summary>
        /// Return the specified comment object with the specified fields.
        /// </summary>
        /// <param name="commentID">ID of the comment to be returned</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <param name="fields">Fields to be returned</param>
        /// <returns>Comment object containing the specified comment</returns>
        public Comment GetComment(int commentID, bool abridged = false, List<string> fields = null)
        {
            string url = commentUrl + "/" + commentID.ToString();
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
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    default:
                        throw;
                }
            }

            return JsonConvert.DeserializeObject<Comment>(json);

            //JObject results = JObject.Parse(json);

            //return results.ToObject<Comment>();
        }

        /// <summary>
        /// Return a paginated list of the people who like the specified comment.
        /// </summary>
        /// <param name="commentID">ID of the comment for which to return liking people</param>
        /// <param name="count">Maximum number of people to be returned</param>
        /// <param name="startIndex">Zero-relative index of the first person to be returned</param>
        /// <param name="fields">Fields to be returned on people that like the comment</param>
        /// <returns>a list of Person objects listing the people who like the specified comment</returns>
        public List<Person> GetCommentLikes(int commentID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> personList = new List<Person>();

            string url = commentUrl + "/" + commentID.ToString() + "/likes";
            url += "?count=" + (count > 1000 ? 1000 : count).ToString();
            url += "&startIndex=" + startIndex.ToString();
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

            //Jive returns a paginated list, so it loops through as many times as necessary
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
                            throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this comment", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist", e);
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
        /// Return a paginated list of comments to the specified content object, optionally limiting the returned results to direct replies only.
        /// The specified content object type must support comments, or be a comment itself (in which case replies to this comment only are returned).
        /// </summary>
        /// <param name="commentID">ID of the comment for which to return reply comments</param>
        /// <param name="excludeReplies">Flag indicating whether to exclude replies (and therefore return direct replies only)</param>
        /// <param name="count">Maximum number of comments to be returned</param>
        /// <param name="startIndex">Zero-relative index of the first comment to be returned</param>
        /// <param name="anchor">optional URI for a comment to anchor at. Specifying a anchor will try to return the page containing the anchor.
        /// If the anchor could not be found then the first page of comments will be returned.</param>
        /// <param name="fields">Fields to be returned in the selected comments</param>
        /// <returns>a list of Comment objects</returns>
        public List<Comment> GetComments(int commentID, bool excludeReplies = false, int count = 25, int startIndex = 0, string anchor = null, List<string> fields = null)
        {
            List<Comment> commentList = new List<Comment>();

            string url = commentUrl + "/" + commentID.ToString() + "/comments";
            url += "?excludeReplies=" + excludeReplies.ToString();
            url += "&count=" + (count > 1000 ? 1000 : count).ToString();
            url += "&startIndex=" + startIndex.ToString();
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

            //Jive returns a paginated list, so it loops through as many times as necessary
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
                            throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment or its replies", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist", e);
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
        /// Return the specified editable content object with the specified fields.
        /// </summary>
        /// <param name="commentID">ID of the comment to be returned</param>
        /// <param name="fields">Fields to be returned</param>
        /// <returns>Comment object containing the specified editable comment</returns>
        public Comment GetEditableComment(int commentID, List<string> fields = null)
        {
            string url = commentUrl + "/" + commentID.ToString() + "/editable";
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
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Content is already being edited by another user", e);
                    default:
                        throw;
                }
            }

            return JsonConvert.DeserializeObject<Comment>(json);
        }

        /// <summary>
        /// Return a paginated list of the people who consider this comment helpful.
        /// </summary>
        /// <param name="commentID">ID of the comment that has been marked helpful</param>
        /// <param name="count">Maximum number of people to be returned (default is 25)</param>
        /// <param name="startIndex">Zero-relative index of the first person to be returned (default is 0)</param>
        /// <param name="fields">Fields to be returned on people who have marked this comment as helpful (default is @summary)</param>
        /// <returns>a list of Person objects listing people who also have marked this comment helpful</returns>
        public List<Person> GetHaveMarkedHelpful(int commentID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> personList = new List<Person>();

            string url = commentUrl + "/" + commentID.ToString() + "/helpful";
            url += "?count=" + (count > 1000 ? 1000 : count).ToString();
            url += "&startIndex=" + startIndex.ToString();
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

            //Jive returns a paginated list, so it loops through as many times as necessary
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
                            throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this comment", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
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
        /// Return a paginated list of the people who consider this comment unhelpful.
        /// </summary>
        /// <param name="commentID">ID of the comment that has been marked unhelpful</param>
        /// <param name="count">Maximum number of people to be returned (default is 25)</param>
        /// <param name="startIndex">Zero-relative index of the first person to be returned (default is 0)</param>
        /// <param name="fields">Fields to be returned on people who have marked this comment as unhelpful (default is @summary)</param>
        /// <returns>a list of Person objects listing people who also have marked this comment unhelpful</returns>
        public List<Person> GetHaveMarkedUnhelpful(int commentID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> personList = new List<Person>();

            string url = commentUrl + "/" + commentID.ToString() + "/unhelpful";
            url += "?count=" + (count > 1000 ? 1000 : count).ToString();
            url += "&startIndex=" + startIndex.ToString();
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

            //Jive returns a paginated list, so it loops through as many times as necessary
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
                            throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this comment", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
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
        /// Update the specified comment with the specified characteristics
        /// </summary>
        /// <param name="commentID">ID of the comment to be updated</param>
        /// <param name="comment">Comment object containing the updated comment</param>
        /// <param name="updated">Date and time when this content object was most recently updated</param>
        /// <param name="fields">Fields to include in the returned entity</param>
        /// <returns></returns>
        public Comment UpdateComment(int commentID, Comment comment, DateTime? updated = null, List<string> fields = null)
        {
            //adds the query strings to the url if present
            string url = commentUrl + "/" + commentID.ToString();
            bool first = true;
            if (updated != null)
            {
                url += "?updated=" + jiveDateFormat((DateTime)updated);
                first = false;
            }
            if (fields != null && fields.Count > 0)
            {
                if (first) url += "?fields=";
                else url += "&fields=";

                foreach (var field in fields)
                {
                    url += field + ",";
                }
                //remove last comma
                url = url.Remove(url.Length - 1);
            }

            string json = JsonConvert.SerializeObject(comment, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
            string result = "";
            try
            {
                result = PutAbsolute(url, json); //makes the HTTP request
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Attempt to add a comment to a content object that does not support them, or for which comments have been closed", e);
                }
            }

            return JsonConvert.DeserializeObject<Comment>(result);

            //JObject results = JObject.Parse(json);

            //return results.ToObject<Comment>();
        }

        /// <summary>
        /// Update the specified editable comment with the specified characteristics.
        /// </summary>
        /// <param name="commentID">ID of the editable comment to be updated</param>
        /// <param name="comment">Comment object containing the updated comment</param>
        /// <param name="updated">Date and time when this content object was most recently updated.</param>
        /// <param name="fields">Fields to include in the returned entity</param>
        /// <returns>Comment object representing the updated comment</returns>
        public Comment UpdateEditableComment(int commentID, Comment comment, DateTime? updated = null, List<string> fields = null)
        {
            //adds the query strings to the url if present
            string url = commentUrl + commentID.ToString() + "/editable";
            bool first = true;
            if (updated != null)
            {
                url += "?updated=" + jiveDateFormat((DateTime)updated);
                first = false;
            }
            if (fields != null && fields.Count > 0)
            {
                if (first) url += "?fields=";
                else url += "&fields=";

                foreach (var field in fields)
                {
                    url += field + ",";
                }
                //remove last comma
                url = url.Remove(url.Length - 1);
            }

            string json = JsonConvert.SerializeObject(comment, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
            string result = "";
            try
            {
                result = PutAbsolute(url, json); //makes the HTTP request
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Attempt to add a comment to a content object that does not support them, or for which comments have been closed", e);
                }
            }

            return JsonConvert.DeserializeObject<Comment>(result);
        }
    }
}
