using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Net.Pokeshot.JiveSdk.Models;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class MessagesClient : JiveClient
    {
        string messagesUrl { get { return JiveCommunityUrl + "/api/core/v3/messages"; } }

        public MessagesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        /// <summary>
        /// Return a list of messages for the specified content object, which must be a discussion, optionally limiting the results to direct replies only.
        /// </summary>
        /// <param name="contentID">ID of the content object (must be a discussion) for which to return reply messages</param>
        /// <param name="count">Maximum number of messages to be returned (default is 25)</param>
        /// <param name="excludeReplies">Flag indicating whether to exclude replies (and therefore return direct replies only) (default is false)</param>
        /// <param name="filter">The filter criteria used to select reply messages</param>
        /// <param name="hierarchical">Flag indicating that replies should be returned in hierarchical order instead of chronological order. (default is true) Since v3.1</param>
        /// <param name="startIndex">Zero-relative index of the first message to be returned (default is 0)</param>
        /// <param name="anchor">optional URI for a message to anchor at. Specifying a anchor will try to return the page containing the anchor. If the anchor could not be found then the first page of messages will be returned.</param>
        /// <param name="fields">Fields to be returned in the selected messages</param>
        /// <returns>Message[] containing the requested messages</returns>
        public List<Message> GetContentReplies(int contentID, int count = 25, bool excludeReplies = false, List<string> filter = null, bool hierarchical = true,
            int startIndex = 0, string anchor = null, List<string> fields = null)
        {
            List<Message> messageList = new List<Message>();

            string url = messagesUrl + "/contents/" + contentID.ToString();
            url += "?excludeReplies=" + excludeReplies.ToString();
            url += "&hierarchical=" + hierarchical.ToString();
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += "&hierarchical=" + hierarchical.ToString();
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object or its messages");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified content object does not exist");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                messageList.AddRange(results["list"].ToObject<List<Message>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return messageList;
        }

        //GetEditableMessages()
        //GetExtendedProperties()
        //GetExtendedPropertiesForAddon()
        //GetHaveMarkedHelpful()
        //GetHaveMarkedUnhelpful()

        /// <summary>
        /// Return the specified message with the specified fields.
        /// </summary>
        /// <param name="messageID">ID of the message to be returned</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <param name="fields">Fields to be returned</param>
        /// <returns>Message containing the specified message</returns>
        public Message GetMessage(int messageID, bool abridged = false, List<string> fields = null)
        {
            string url = messagesUrl + "/" + messageID.ToString();
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
                        throw new HttpException(e.WebEventCode, "An input field is missing or malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified comment");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified comment does not exist");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<Message>();
        }

        //GetMessageLikes()
        //GetMessageOutcomes()
        //GetOutcomeTypes()

        /// <summary>
        /// Return a list of messages that are replies to the specified message, optionally limiting the returned results to direct replies only.
        /// </summary>
        /// <param name="messageID">ID of the message for which to return reply messages</param>
        /// <param name="count">Maximum number of messages to be returned (default is 25)</param>
        /// <param name="excludeReplies">Flag indicating whether to exclude replies (and therefore return direct replies only) (default is false)</param>
        /// <param name="filter">The filter criteria used to select reply messages</param>
        /// <param name="hierarchical">Flag indicating that replies should be returned in hierarchical order instead of chronological order. (default is true) Since v3.1</param>
        /// <param name="startIndex">Zero-relative index of the first message to be returned (default is 0)</param>
        /// <param name="anchor">optional URI for a message to anchor at. Specifying a anchor will try to return the page containing the anchor. If the anchor could not be found then the first page of messages will be returned.</param>
        /// <param name="fields">Fields to be returned in the selected messages</param>
        /// <param name="messageTarget">is the target from which you want to replies. Must be one of "replyToTlo", "replyAsComment", "all"</param>
        /// <returns>Message[] containing the requested messages</returns>
        public List<Message> GetReplies(int messageID, int count = 25, bool excludeReplies = false, List<string> filter = null, bool hierarchical = true,
                    int startIndex = 0, string anchor = null, string messageTarget = "all", List<string> fields = null)
        {
            List<Message> messageList = new List<Message>();

            string url = messagesUrl + "/" + messageID.ToString() + "/messages";
            url += "?excludeReplies=" + excludeReplies.ToString();
            url += "&hierarchical=" + hierarchical.ToString();
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += "&hierarchical=" + hierarchical.ToString();
            url += "&messageTarget=" + messageTarget;
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified message or its replies");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified message does not exist");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                messageList.AddRange(results["list"].ToObject<List<Message>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return messageList;
        }
    }
}
