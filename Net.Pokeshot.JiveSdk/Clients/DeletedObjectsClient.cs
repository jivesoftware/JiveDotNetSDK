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
    public class DeletedObjectsClient : JiveClient
    {
        string deletedObjectsUrl { get { return JiveCommunityUrl + "/api/core/v3/deletedObjects"; } }

        public DeletedObjectsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        /// <summary>
        /// Return a list of entities, each representing a content object that has been deleted.
        /// </summary>
        /// <param name="since">Restricts results to contain entities representing deleted objects removed on or after this date.</param>
        /// <param name="types">One or more content object types (discussion, post, file, ...).</param>
        /// <param name="sort">Sort order; default returns the most recently deleted objects first.
        /// eventDateAsc - Sort by the date the content object was removed, in ascending order.
        /// eventDateDesc - Sort by the date the content object was removed, in descending order. Default if none was specified.</param>
        /// <param name="startIndex">Zero-relative index of the first entity to be returned.</param>
        /// <param name="count">Maximum number of entities to be returned from Jive at a time.</param>
        /// <param name="fields">Fields to be returned in the selected entities.</param>
        /// <returns>List of DeletedObjects</returns>
        public List<DeletedObject> GetDeletedContents(DateTime? since = null, List<string> types = null, string sort = "eventDateDesc", int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<DeletedObject> deletedObjectsList = new List<DeletedObject>();

            var filter = new List<string>();
            if (types != null && types.Count > 0) {
                StringBuilder typeString = new StringBuilder();
                typeString.Append("type(");

                foreach (var type in types)
                {
                    typeString.Append(type + ",");
                }
                
                // Remove last comma.
                typeString.Remove(typeString.Length - 1, 1);
                typeString.Append(")");
                filter.Add(typeString.ToString());
            };
            if(since != null)
                filter.Add("since(" + jiveDateFormat(since.Value) + ")");

            string url = deletedObjectsUrl + "/contents";
            url += "?sort=" + sort;
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
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
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                deletedObjectsList.AddRange(results["list"].ToObject<List<DeletedObject>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return deletedObjectsList;
        }

        /// <summary>
        /// Return the specified deleted object entity with the specified fields.
        /// </summary>
        /// <param name="deletedObjectId">UUID of the deleted object entity to be returned</param>
        /// <param name="fields">Fields to be returned</param>
        /// <returns>DeletedObject</returns>
        public DeletedObject GetDeletedObject(string deletedObjectId, List<string> fields = null)
        {
            string url = deletedObjectsUrl + "/" + deletedObjectId;
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
                        throw new HttpException(e.WebEventCode, "An input field is malformed");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified deleted object entity does not exist");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<DeletedObject>();
        }

        /// <summary>
        /// Return a paginated list of entities, each representing a person object that has been deleted.
        /// </summary>
        /// <param name="since">Restricts results to contain entities representing deleted objects removed on or after this date.</param>
        /// <param name="types">One or more content object types (discussion, post, file, ...).</param>
        /// <param name="sort">Sort order; default returns the most recently deleted objects first.
        /// eventDateAsc - Sort by the date the content object was removed, in ascending order.
        /// eventDateDesc - Sort by the date the content object was removed, in descending order. Default if none was specified.</param>
        /// <param name="startIndex">Zero-relative index of the first entity to be returned.</param>
        /// <param name="count">Maximum number of entities to be returned from Jive at a time.</param>
        /// <param name="fields">Fields to be returned in the selected entities.</param>
        /// <returns>List of DeletedObjects</returns>>
        public List<DeletedObject> GetDeletedPeople(DateTime? since = null, List<string> types = null, string sort = "eventDateDesc", int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<DeletedObject> deletedObjectsList = new List<DeletedObject>();

            List<string> filter = new List<string>();
            if (since != null)
                filter.Add("since(" + jiveDateFormat(since.Value) + ")");

            string url = deletedObjectsUrl + "/people";
            url += "?sort=" + sort;
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
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
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                deletedObjectsList.AddRange(results["list"].ToObject<List<DeletedObject>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return deletedObjectsList;
        }

        /// <summary>
        /// Return a paginated list of entities, each representing a place object that has been deleted. 
        /// </summary>
        /// <param name="since">Restricts results to contain entities representing deleted objects removed on or after this date.</param>
        /// <param name="types">One or more content object types (discussion, post, file, ...).</param>
        /// <param name="sort">Sort order; default returns the most recently deleted objects first.
        /// eventDateAsc - Sort by the date the content object was removed, in ascending order.
        /// eventDateDesc - Sort by the date the content object was removed, in descending order. Default if none was specified.</param>
        /// <param name="startIndex">Zero-relative index of the first entity to be returned.</param>
        /// <param name="count">Maximum number of entities to be returned from Jive at a time.</param>
        /// <param name="fields">Fields to be returned in the selected entities.</param>
        /// <returns>List of DeletedObjects</returns>
        public List<DeletedObject> GetDeletedPlaces(DateTime? since = null, List<string> types = null, string sort = "eventDateDesc", int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<DeletedObject> deletedObjectsList = new List<DeletedObject>();

            var filter = new List<string>();
            if (types != null && types.Count > 0)
            {
                StringBuilder typeString = new StringBuilder();
                typeString.Append("type(");

                foreach (var type in types)
                {
                    typeString.Append(type + ",");
                }

                // Remove last comma.
                typeString.Remove(typeString.Length - 1, 1);
                typeString.Append(")");
                filter.Add(typeString.ToString());
            };
            if (since != null)
                filter.Add("since(" + jiveDateFormat(since.Value) + ")");

            string url = deletedObjectsUrl + "/places";
            url += "?sort=" + sort;
            url += "&startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
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
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                deletedObjectsList.AddRange(results["list"].ToObject<List<DeletedObject>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return deletedObjectsList;
        }
    }
}