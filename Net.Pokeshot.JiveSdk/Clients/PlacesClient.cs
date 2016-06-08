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
    public class PlacesClient : JiveClient
    {
        string placesUrl { get { return JiveCommunityUrl + "/api/core/v3/places"; } }
        public PlacesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        /// <summary>
        /// Checks for the presence of the given categories in the given Place, and creates any that are not present.
        /// Note: will throw an HttpException if the max number of categories has been reached.
        /// </summary>
        /// <param name="placeID">ID of the Place the categories are supposed to be found or created</param>
        /// <param name="categories">a list of the categories to be found or created</param>
        /// <returns>a string List of the categories that were successfully created</returns>
        public List<string> CheckAndCreateCategories(int placeID, List<string> categories)
        {
            //pulls all of the current categories for the defined Place
            List<Category> categoryList = GetPlaceCategories(placeID);

            //list of categories that are created
            List<string> addedList = new List<string>();

            bool found;
            Category newCategory;
            foreach (var category in categories)
            {
                found = false;
                foreach (var oldCategory in categoryList)
                {
                    if (category == oldCategory.name) found = true;
                }
                if (!found)
                {
                    newCategory = new Category();
                    newCategory.name = category;
                    newCategory.type = "category";
                    try
                    {
                        CreatePlaceCategory(placeID, newCategory);
                    }
                    catch (HttpException e)
                    {
                        if (e.GetHttpCode() == 400) throw;
                    }
                    addedList.Add(category);
                }
            }

            return addedList;
        }


        /// <summary>
        /// Create a new content object with specified characteristics, and return an entity representing the newly created content object.
        /// </summary>
        /// <param name="placeID">ID of the place the content should be added to</param>
        /// <param name="content">a GenericContent object describing the content object to be created</param>
        /// <param name="published">a DateTime of when this content object was originally created. Set 'updated' param as well. Only set this field when importing content.</param>
        /// <param name="updated">a DateTime of when this content object was most recently updated. Set 'published' param as well. Only set this field when importing content.</param>
        /// <param name="fields">The fields to include in the returned entity</param>
        /// <returns>a GenericContent object representing the newly created content</returns>
        public GenericContent CreateContent(int placeID, GenericContent content, DateTime? published = null, DateTime? updated = null, List<string> fields = null)
        {
            //constructs the url for the HTTP request based on the user specifications
            bool first = true;
            string url = placesUrl + "/" + placeID.ToString() + "/contents";
            if (published != null)
            {
                url += "?published=" + jiveDateFormat((DateTime)published);
                first = false;
            }
            if (updated != null)
            {
                if (first) url += "?updated=";
                else url += "&updated=";
                url += jiveDateFormat((DateTime)updated);
                first = false;
            }
            if (fields != null && fields.Count > 0)
            {
                if (first == true)
                {
                    url += "?fields=";
                    first = false;
                }
                else url += "&fields=";

                foreach (var field in fields)
                {
                    url += field + ",";
                }
                //remove last comma
                url = url.Remove(url.Length - 1);
            }

            //converts the content to be added into JSON format and makes the HTTP request
            string json = JsonConvert.SerializeObject(content, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
            string result;
            try
            {
                result = PostAbsolute(url, json);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content or place");
                    case 409:
                        throw new HttpException(e.WebEventCode, "The new entity would conflict with system restrictions (such as two contents of the same type with the same name");
                    default:
                        throw;
                }
            }

            //converts the returned JSON into a GenericContent object and returns it to the user
            JObject Json = JObject.Parse(result);
            return Json.ToObject<GenericContent>();
        }

        /// <summary>
        /// Create a new category for a place with specified characteristics, and return an entity representing the newly created category.
        /// </summary>
        /// <param name="placeID">ID of the place for which to create a category</param>
        /// <param name="new_category">Category object describing the category to be created</param>
        /// <param name="autoCategorize">Flag indicating whether existing content of the place will be categorized under the new category</param>
        /// <param name="fields">Fields to be returned (default is @all)</param>
        /// <returns>Category object representing the newly created category</returns>
        public Category CreatePlaceCategory(int placeID, Category new_category, bool autoCategorize = false, List<string> fields = null)
        {
            //construct the url for the HTTP request based on the user specifications
            string url = placesUrl + "/" + placeID.ToString() + "/categories";
            url += "?autoCategorize=" + autoCategorize.ToString();
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

            //convert the Category object to JSON format and post via HTTP
            string json = JsonConvert.SerializeObject(new_category, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
            string result;
            try
            {
                result = PostAbsolute(url, json);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed or max number of categories has been reached");
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to manage categories for the place");
                    case 409:
                        throw new HttpException(e.WebEventCode, "The new entity would conflict with system restrictions (such as two categories with the same name");
                    default:
                        throw;
                }
            }

            //convert the returned JSON into a Category object and return to the user
            JObject Json = JObject.Parse(result);
            return Json.ToObject<Category>();
        }

        /// <summary>
        /// Delete the existing category for the specified place. Only admins of the place can manage place categories.
        /// </summary>
        /// <param name="placeID">ID of the place for which the category is to be deleted</param>
        /// <param name="categoryID">ID of the category to delete</param>
        public void DestroyPlaceCategory(int placeID, int categoryID)
        {
            string url = placesUrl + "/" + placeID.ToString() + "/categories/" + categoryID.ToString();

            try
            {
                DeleteAbsolute(url); //makes the HTTP DELETE call
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to delete this image");
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified place or image does not exist");
                    default:
                        throw;
                }
            }

            return;
        }

        //GetActivity()
        //GetAppliedEntitlements()

        /// <summary>
        /// Return a list of featured content objects in the specified place.
        /// </summary>
        /// <param name="placeID">ID of the place the objects are being retrieved from</param>
        /// <param name="sort">Requested sort order.
        /// Possible strings:
        /// dateCreatedAsc - Sort by the date this place was created, in ascending order.
        /// dateCreatedDesc - Sort by the date this place was created, in descending order. Default if none was specified.
        /// latestActivityAsc - Sort by the date this place had the most recent activity, in ascending order.
        /// latestActivityDesc - Sort by the date this place had the most recent activity, in descending order.
        /// titleAsc - Sort by place name, in ascending order.</param>
        /// <param name="type">one or more object types of desired contained content objects (document, discussion, post, poll) separated by commas</param>
        /// <param name="fields">The fields to be returned on each content</param>
        /// <param name="startIndex">The zero-relative index of the first matching content to be returned</param>
        /// <param name="count">The maximum number of contents to be returned per Jive HTTP request</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <returns>a GenericContent object list of the matched content objects</returns>
        public List<GenericContent> GetContents(int placeID, string sort = "dateCreatedDesc", List<string> type = null, List<string> fields = null, int startIndex = 0, int count = 25, bool abridged = false)
        {
            //construct url for the HTTP request with the user specifications
            string url = placesUrl + "/" + placeID.ToString() + "/contents";
            url += "?sort=" + sort;
            url += "&abridged=" + abridged.ToString();
            url += "&count=" + (count > 100 ? 100 : count).ToString();
            if (startIndex != 0) url += "&startIndex=" + startIndex.ToString();
            if (type != null && type.Count > 0)
            {
                url += "&filter=type(";
                foreach (var item in type)
                {
                    url += item + ",";
                }
                //remove last comma
                url.Remove(url.Length - 1);

                url += ")";
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
            string json;
            JObject results;
            List<GenericContent> contentList = new List<GenericContent>();
            while (true)
            {
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
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object or place");
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified place does not exist");
                        default:
                            throw;
                    }
                }
                results = JObject.Parse(json);

                contentList.AddRange(results["list"].ToObject<List<GenericContent>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return contentList;

        }

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

        /// <summary>
        /// Return categories associated to the specified place.
        /// </summary>
        /// <param name="placeID">ID of the place to return the categories of</param>
        /// <param name="fields">Fields to be returned (default is @all)</param>
        /// <returns>a Category object list of the categories for the given place</returns>
        public List<Category> GetPlaceCategories(int placeID, List<string> fields = null)
        {
            List<Category> categoryList = new List<Category>();

            //creates the url for the HTTP request based on the user specifications
            string url = placesUrl + "/" + placeID.ToString() + "/categories";
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

            // jive returns a paginated list, so we have to loop through all of the pages.
            string json;
            JObject results;
            while (true)
            {
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
                results = JObject.Parse(json);

                categoryList.AddRange(results["list"].ToObject<List<Category>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }

            return categoryList;
        }

        /// <summary>
        /// Return the specified category of a place.
        /// </summary>
        /// <param name="placeID">ID of the place that is associated to the category</param>
        /// <param name="categoryID">ID of the category to return</param>
        /// <param name="fields">Fields to be returned (default is @all)</param>
        /// <returns>a Category object containing the specified category</returns>
        public Category GetPlaceCategory(int placeID, int categoryID, List<string> fields = null)
        {
            //creates the url for the HTTP request based on the user specifications
            string url = placesUrl + "/" + placeID.ToString() + "/categories/" + categoryID.ToString();
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

            //makes the HTTP GET request and parses the result into a Category object before returning
            string result;
            try
            {
                result = GetAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field is malformed");
                    case 404:
                        throw new HttpException(e.WebEventCode, "If the specified category does not exist");
                    default:
                        throw;
                }
            }
            JObject Json = JObject.Parse(result);
            return Json.ToObject<Category>();
        }

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
