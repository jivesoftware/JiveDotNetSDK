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
    public class PlacesClient : JiveClient
    {
        string placesUrl { get { return JiveCommunityUrl + "/api/core/v3/places"; } }
        public PlacesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        //GetActivity()
        //GetAppliedEntitlements()
        //GetContent()
        //GetExtProps()
        //GetExtPropsForAddOn()
        //GetFeaturedContent()
        //GetPages()

        /// <summary>
        /// Return the specified place with the specified fields.
        /// </summary>
        /// <param name="placeID">ID of the place to be returned</param>
        /// <param name="fields">Fields to be returned</param>
        /// <returns>GenericPlace</returns>
        public GenericPlace GetPlace(int placeID, List<string> fields = null)
        {
            string url = placesUrl + "/" + placeID.ToString();
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
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified place");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified place does not exist");
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<GenericPlace>();
        }

        //GetPlaceAnnouncements()
        //GetPlaceAvatar()
        //GetPlaceCatagories()
        //GetPlaceCategory()
        //GetPlaceFollowers()
        //GetPlaceFollowingIn()
        //GetPlacePermissions()

        /// <summary>
        /// Return a paginated list of places contained within the specified place.
        /// </summary>
        /// <param name="placeID">ID of the parent place for which to retrieve contained places</param>
        /// <param name="startIndex">Zero-relative index at which to start results</param>
        /// <param name="count">Maximum number of places to be returned</param>
        /// <param name="sort">Requested sort order.
        /// Possible strings:
        /// dateCreatedAsc - Sort by the date this place was created, in ascending order.
        /// dateCreatedDesc - Sort by the date this place was created, in descending order.
        /// latestActivityAsc - Sort by the date this place had the most recent activity, in ascending order.
        /// latestActivityDesc - Sort by the date this place had the most recent activity, in descending order.
        /// titleAsc - Sort by place name, in ascending order.</param>
        /// <param name="fields">Fields to be included in the returned entities</param>
        /// <param name="search">One or more search terms. You must escape any of the following special characters embedded in the search terms: comma (","), backslash ("\"), left parenthesis ("("), and right parenthesis (")") by preceding them with a backslash. Wildcards can be used, e.g. to search by substring use "*someSubstring*".</param>
        /// <param name="tag">One or more tags (matching any tag will select a place)</param>
        /// <param name="type">One or more object types of desired contained places (blog, project, space)</param>
        /// <returns>GenericPlace[] listing the contained places</returns>
        public List<GenericPlace> GetPlacePlaces(int placeID, int startIndex = 0, int count = 25, string sort = "titleAsc", List<string> fields = null, List<string> search = null, List<string> tag = null, List<string> type = null)
        {
            List<GenericPlace> placesList = new List<GenericPlace>();
            List<string> filter = new List<string>();

            // If search terms are provided, add them to the filter
            if (search != null && search.Count > 0)
            {
                string searchString = "search(";
                foreach (var searchTerm in search)
                {
                    searchString += searchTerm + ",";
                }
                // remove last comma
                searchString = searchString.Remove(searchString.Length - 1);

                searchString += ")";
                filter.Add(searchString);
            }

            if (tag != null && tag.Count > 0)
            {
                string tagString = "tag(";
                foreach (var item in tag)
                {
                    tagString += item + ",";
                }
                // remove last comma
                tagString = tagString.Remove(tagString.Length - 1);

                tagString += ")";
                filter.Add(tagString);
            }

            if (type != null && type.Count > 0)
            {
                string typeString = "type(";
                foreach (var item in type)
                {
                    typeString += item + ",";
                }
                // remove last comma
                typeString = typeString.Remove(typeString.Length - 1);

                typeString += ")";
                filter.Add(typeString);
            }


            string url = placesUrl + "/" + placeID.ToString() + "/places";
            url += "?startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += "&sort=" + sort.ToString();
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this place");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified place does not exist");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                placesList.AddRange(results["list"].ToObject<List<GenericPlace>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return placesList;
        }
        /// <summary>
        /// Return a paginated list of places contained within the specified place.
        /// </summary>
        /// <param name="startIndex">Zero-relative index at which to start results</param>
        /// <param name="count">Maximum number of places to be returned</param>
        /// <param name="sort">Requested sort order.
        /// Possible strings:
        /// dateCreatedAsc - Sort by the date this place was created, in ascending order.
        /// dateCreatedDesc - Sort by the date this place was created, in descending order.
        /// latestActivityAsc - Sort by the date this place had the most recent activity, in ascending order.
        /// latestActivityDesc - Sort by the date this place had the most recent activity, in descending order.
        /// titleAsc - Sort by place name, in ascending order.</param>
        /// <param name="fields">Fields to be included in the returned entities</param>
        /// <param name="search">One or more search terms. You must escape any of the following special characters embedded in the search terms: comma (","), backslash ("\"), left parenthesis ("("), and right parenthesis (")") by preceding them with a backslash. Wildcards can be used, e.g. to search by substring use "*someSubstring*".</param>
        /// <param name="tag">One or more tags (matching any tag will select a place)</param>
        /// <param name="type">One or more object types of desired contained places (blog, project, space)</param>
        /// <returns>GenericPlace[] listing the matching places</returns>
        public List<GenericPlace> GetPlaces(int startIndex = 0, int count = 25, string sort = "titleAsc", List<string> fields = null, List<string> search = null, List<string> tag = null, List<string> type = null)
        {
            List<GenericPlace> placesList = new List<GenericPlace>();
            List<string> filter = new List<string>();

            // If search terms are provided, add them to the filter
            if (search != null && search.Count > 0)
            {
                string searchString = "search(";
                foreach (var searchTerm in search)
                {
                    searchString += searchTerm + ",";
                }
                // remove last comma
                searchString = searchString.Remove(searchString.Length - 1);

                searchString += ")";
                filter.Add(searchString);
            }

            if (tag != null && tag.Count > 0)
            {
                string tagString = "tag(";
                foreach (var item in tag)
                {
                    tagString += item + ",";
                }
                // remove last comma
                tagString = tagString.Remove(tagString.Length - 1);

                tagString += ")";
                filter.Add(tagString);
            }

            if (type != null && type.Count > 0)
            {
                string typeString = "type(";
                foreach (var item in type)
                {
                    typeString += item + ",";
                }
                // remove last comma
                typeString = typeString.Remove(typeString.Length - 1);

                typeString += ")";
                filter.Add(typeString);
            }


            string url = placesUrl;
            url += "?startIndex=" + startIndex.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            url += "&sort=" + sort.ToString();
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access this place");
                        default:
                            throw;
                    }
                }
                JObject results = JObject.Parse(json);

                placesList.AddRange(results["list"].ToObject<List<GenericPlace>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return placesList;
        }

        //GetPlaceSettings()
        //GetPlaceStatics()
        //GetPlaceSuggestedPlaces()
        //GetPlaceTasks()
        //GetRecommendedPlaces()

        /// <summary>
        /// Return the root space for this Jive instance.
        /// </summary>
        /// <returns>Space</returns>
        public Space GetRootSpace()
        {
            string url = placesUrl + "/" + "root";

            string json;
            try
            {
                json = GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<Space>();
        }

        //GetSuggestedPlaces()
        //GetTrendingPlaces()

    }
}
