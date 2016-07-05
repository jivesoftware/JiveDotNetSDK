﻿using System;
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

            JObject results = JObject.Parse(json);

            return results.ToObject<Comment>();
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

            JObject results = JObject.Parse(json);

            return results.ToObject<Comment>();
        }
    }
}
