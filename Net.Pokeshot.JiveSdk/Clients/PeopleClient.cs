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
        public PeopleClient(IJiveUrlAndCredentials jiveUrlAndCredentials) : base(jiveUrlAndCredentials) { }

        /// <summary>
        /// Method used for importing content from another site and an author for the content is required. The method first checks to see if any user
        /// with any of the external user's emails is already present on the site, and returns it if present, otherwise getting or creating and returning
        /// a default "anonymous" user to be used for the import.
        /// Note: this will print an error message to the console for every email tried that isn't present, this can be commented out in GetAbsolute in JiveClient
        /// </summary>
        /// <param name="extPerson">the Person object from the external site (usually the author of the external content)</param>
        /// <returns>a Person object that can be used as the author for the newly imported content</returns>
        public Person FindPerson(Person extPerson)
        {
            Person person = null;
            bool found = false;
            if (extPerson.emails != null)
            {
                foreach (var address in extPerson.emails)
                {
                    if (!found)
                    {
                        try
                        {
                            person = GetPersonByEmail(address.value);
                            found = true;
                        }
                        catch (HttpException)
                        {
                            found = false; //shouldn't be necessary, but just to make sure found isn't set to true when no user is found
                        }
                    }
                }
            }
            if (!found)
            {
                try
                {
                    person = GetPersonByUsername("anonymous@test.com");
                }
                catch (HttpException)
                {
                    person = new Person();
                    person.emails = new List<ProfileEntry>();
                    person.emails.Add(new ProfileEntry());
                    person.emails[0].value = "anonymous@test.com"; //use some dummy address here, Jive requires this field not be null
                    person.emails[0].jive_label = "Email";
                    person.emails[0].primary = true;
                    person.emails[0].type = "work";
                    person.jive = new JivePerson();
                    person.jive.username = "anonymous@test.com";
                    person.jive.password = "guest";
                    person.name = new Name();
                    person.name.familyName = "Guest";
                    person.name.givenName = "Anonymous";
                    person.type = "person";
                    person = CreatePerson(person);
                }
            }

            return person;
        }

        /// <summary>
        /// Method used for importing content from another site and an author for the content is required. The method first checks to see if any user
        /// with the external user's email is already present on the site, and returns it if present, otherwise getting or creating and returning
        /// a default "anonymous" user to be used for the import.
        /// Note: this will print an error message to the console if the email tried isn't present, this can be commented out in GetAbsolute in JiveClient
        /// </summary>
        /// <param name="email">the email from the external site (usually the author of the external content)</param>
        /// <returns>a Person object that can be used as the author for the newly imported content</returns>
        public Person FindPersonByEmail(string email)
        {
            Person person = null;
            bool found = false;
            try
            {
                person = GetPersonByEmail(email);
                found = true;
            }
            catch (HttpException)
            {
                found = false; //shouldn't be necessary, but just to make sure found isn't set to true when no user is found
            }
            if (!found)
            {
                try
                {
                    person = GetPersonByUsername("anonymous@test.com");
                }
                catch (HttpException)
                {
                    person = new Person();
                    person.emails = new List<ProfileEntry>();
                    person.emails.Add(new ProfileEntry());
                    person.emails[0].value = "anonymous@test.com"; //use some dummy address here, Jive requires this field not be null
                    person.emails[0].jive_label = "Email";
                    person.emails[0].primary = true;
                    person.emails[0].type = "work";
                    person.jive = new JivePerson();
                    person.jive.username = "anonymous";
                    person.jive.password = "guest";
                    person.name = new Name();
                    person.name.familyName = "Guest";
                    person.name.givenName = "Anonymous";
                    person.type = "person";
                    person = CreatePerson(person);
                }
            }

            return person;
        }

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
                        throw new HttpException(e.WebEventCode, "Specified user is not the authenticated user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist", e);
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
                        throw new HttpException(e.WebEventCode, "An input field was malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist", e);
                    case 410:
                        throw new HttpException(e.WebEventCode, "Expertise feature is disabled", e);
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
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified tag or person does not exist", e);
                    case 410:
                        throw new HttpException(e.WebEventCode, "Expertise feature is disabled", e);
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
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to create this relationship", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "One of both of the specified users cannot be found", e);
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Create a new Page object for a user based on the contents of the specified Page. Only modifiable fields that actually provide a value
        /// in the incoming entity are processed.
        /// </summary>
        /// <param name="personID">Authenticated user. Use @me or the ID of the authenticated user</param>
        /// <param name="page">the Page object containing the information on the page to be created</param>
        public void CreatePage(string personID, Page page)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/pages";

            string json = JsonConvert.SerializeObject(page, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            try
            {
                PostAbsolute(url, json);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "An input field was malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Specified user is not the authenticated user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist", e) ;
                    case 409:
                        throw new HttpException(e.WebEventCode, "Requested change would cause business rules to be violated");
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Create a Person object for a new user based on the contents of the specified Person. Only modifiable fields that actually provide a value
        /// in the incoming entity are processed.
        /// </summary>
        /// <param name="new_person">the Person object containing information describing the new user</param>
        /// <param name="welcome">Flag indicating that a welcome email should be sent to the newly created user</param>
        /// <param name="published">Date and time when this person was originally created. Only set this field when importing people.</param>
        /// <param name="fields">The fields to include in the returned entity</param>
        /// <returns>a Person object representing the created user</returns>
        public Person CreatePerson(Person new_person, bool welcome = false, DateTime? published = null, List<string> fields = null)
        {
            DateTime tmp;

            //construct the url for the HTTP request based on the user's specifications
            string url = peopleUrl;
            url += "?welcome=" + welcome.ToString();
            if (published != null)
            {
                tmp = (DateTime)published;
                url += "&published=" + jiveDateFormat(tmp);
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

            string json = JsonConvert.SerializeObject(new_person, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
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
                        throw new HttpException(e.WebEventCode, "Any of the input fields are malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorized to make changes to the specified user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Requested change would cause business rules to be violated (such as more than one user with the same email address", e);
                    case 500:
                        throw new HttpException(e.WebEventCode, "Internal server error (e.g. username must be valid email address)", e);
                    case 501:
                        throw new HttpException(e.WebEventCode, "User creation is not supported in this Jive instance", e);
                    default:
                        throw;
                }
            }

            JObject Json = JObject.Parse(result);
            return Json.ToObject<Person>();
        }

        /// <summary>
        /// Create a personal task.
        /// </summary>
        /// <param name="personID">ID of the user for which to create a task</param>
        /// <param name="task">Task object containing information describing the personal task</param>
        /// <param name="fields">fields to return in the returned Task object</param>
        /// <returns>a Task object representing the created task</returns>
        public Models.Task CreateTask(int personID, Models.Task task, List<string> fields = null)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/tasks";
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

            string json = JsonConvert.SerializeObject(task, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, Formatting = Formatting.Indented });
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
                        throw new HttpException(e.WebEventCode, "Any of the input fields are malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "New entity would conflict with system restrictions (such as two contents of the same type with the same name", e);
                    default:
                        throw;
                }
            }

            JObject Json = JObject.Parse(result);
            return Json.ToObject<Models.Task>();
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
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified tag or person does not exist", e);
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Delete the active avatar for the specified user. Only uploaded avatars can be deleted. The system avatar will be selected for the user after the delete operation is completed.
        /// </summary>
        /// <param name="personID">ID of the specified user</param>
        public void DestroyAvatar(int personID)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/avatar";

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorized to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user or profile image cannot be found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Attempt to delete an avatar that cannot be deleted", e);
                    default:
                        throw;
                }
            }
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
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified tag or person does not exist", e);
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
        public void DestroyFollowing(int personID, int followedPersonID)
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
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to delete this relationship", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "One of both of the specified users cannot be found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Following relationship does not exist between these two users", e);
                    default:
                        throw;
                }
            }

            return;
        }

        /// <summary>
        /// Trigger a background task to delete the specified person, and all of their content. Returns an HTTP 202 (Accepted) status to indicate that the deletion request
        /// has been accepted. The only way that a client can tell it has been completed is by trying a GET on the person URI, and waiting until a NotFoundException is returned.
        /// WARNING = It is possible that errors during the deletion process might cause the delete to be abandoned.
        /// </summary>
        /// <param name="personID">ID of the person to be deleted</param>
        public void DestroyPerson(int personID)
        {
            string url = peopleUrl + "/" + personID.ToString();

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "Specified ID is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorized to delete this user (Jive admin only)", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "ID does not identity a valid user", e);
                    case 501:
                        throw new HttpException(e.WebEventCode, "User deletion is not supported in this Jive instance", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Delete the specified profile image for the specified user.
        /// </summary>
        /// <param name="personID">ID of the specified user</param>
        /// <param name="index">1-relative index of the specified profile image</param>
        public void DestroyProfileImage(int personID, int index)
        {
            string url = peopleUrl + "/";
            url += personID.ToString() + "/images/";
            url += index.ToString();

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "Specified index is out of range", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user or profile image cannot be found", e);
                    case 410:
                        throw new HttpException(e.WebEventCode, "Profile images are not enabled in this Jive instance", e);
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Delete (i.e. retire) an existing manager-report relationship between the specified manager user and the specified report user.
        /// </summary>
        /// <param name="personID">ID of the user which is the manager in the existing relationship</param>
        /// <param name="reportPersonID">ID of the user which is the direct report in the existing relationship</param>
        public void DestroyReport(int personID, int reportPersonID)
        {
            string url = peopleUrl + "/";
            url += personID.ToString() + "/@reports/";
            url += reportPersonID.ToString();

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to delete this relationship", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "One or both of the specified users cannot be found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Manager-report relationship does not currently exist between these two users", e);
                    case 410:
                        throw new HttpException(e.WebEventCode, "Organization Chart relationships are not supported by this Jive instance", e);
                    default:
                        throw;
                }
            }
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
                        throw new HttpException(e.WebEventCode, "Specified user ID is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requesting user is not allowed to retrieve activities for the specified user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The activities or the specified user is not found", e);
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results["list"].ToObject<List<Activity>>();
        }

        /// <summary>
        /// Return a paginated list of Persons for users that match the specified criteria. Users will be sorted by userID ascending.
        /// </summary>
        /// <param name="count">Maximum number of instances to be returned per Jive HTTP request</param>
        /// <param name="startIndex">Zero-relative index of the first instance to be returned</param>
        /// <param name="fields">Fields to be returned (or null for summary fields)</param>
        /// <param name="includeDisabled">Include deactivated users (default=false)</param>
        /// <param name="includeExternal">Include external users (default=false). These are users that represent external systems or are external people that have been invited to join a place.</param>
        /// <param name="visibleOnly">Do not include invisible users (default=true means only visible users)</param>
        /// <returns>a list of Person objects for users that match the specified criteria</returns>
        public List<Person> GetAllPeople(int count = 25, int startIndex = 0, List<string> fields = null, bool includeDisabled = false, bool includeExternal = false,
            bool visibleOnly = true)
        {
            List<Person> peopleList = new List<Person>();

            //check for user provided filters for the request
            List<string> filter = new List<string>();
            if (includeDisabled == true) filter.Add("include-disabled(true)");
            if (includeExternal == true) filter.Add("include-external(true)");
            if (visibleOnly == false) filter.Add("visible-only(false)");

            //construct the url for the HTTP request based on the user provided specifications
            string url = peopleUrl + "/@all";
            url += "?count=" + count.ToString();
            if (startIndex != 0) url += "&startIndex=" + startIndex.ToString();
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

            //Jive returns a paginated list that must be looped through
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorized to retrieve this user information", e);
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
        /// Return the binary content of the avatar image for the specified user.
        /// </summary>
        /// <param name="personID">ID of the user for which to return an avatar</param>
        /// <param name="width">Suggested width for resizing the image. If image is small than the requested size then original size is preserved.</param>
        /// <param name="height">Suggested height for resizing the image. If image is small than the requested size then original size is preserved.</param>
        /// <param name="preserveAspectRatio">boolean indicating whether to preserve the original image's aspect ratio</param>
        /// <returns>Binary content of the avatar image</returns>
        public byte[] GetAvatar(int personID, int? width = null, int? height = null, bool? preserveAspectRatio = null)
        {
            //constructs the url for the HTTP request based on the user specifications
            bool first = true;
            string url = peopleUrl + "/" + personID.ToString() + "/avatar";
            if (width != null)
            {
                url += "?width=" + width.Value.ToString();
                first = false;
            }
            if (height != null)
            {
                if (first) url += "?height=" + height.Value.ToString();
                else url += "&height=" + height.Value.ToString();
                first = false;
            }
            if (preserveAspectRatio != null)
            {
                if (first) url += "?preserveAspectRatio=" + preserveAspectRatio.Value.ToString();
                else url += "&preserveAspectRatio=" + preserveAspectRatio.Value.ToString();
                first = false;
            }

            byte[] data;
            try
            {
                data = GetBytesAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "Specified user ID is missing or malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to retrieve the avatar for the specified user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Avatar image for the specified user was not found", e);
                    default:
                        throw;
                }
            }

            return data;
        }

        /// <summary>
        /// Return the binary content of the avatar image for deactivated users.
        /// </summary>
        /// <param name="width">Suggested width for resizing the image (must be less than original width)</param>
        /// <param name="height">Suggested height for resizing the image (must be less than original height)</param>
        /// <param name="preserveAspectRatio">boolean indicating whether to preserve the original image's aspect ratio</param>
        /// <returns>Binary content of the avatar image</returns>
        public byte[] GetAvatarDeactivated(int? width = null, int? height = null, bool? preserveAspectRatio = null)
        {
            //constructs the url for the HTTP request based on the user specifications
            bool first = true;
            string url = peopleUrl + "/avatar/deactivated";
            if (width != null)
            {
                url += "?width=" + width.Value.ToString();
                first = false;
            }
            if (height != null)
            {
                if (first) url += "?height=" + height.Value.ToString();
                else url += "&height=" + height.Value.ToString();
                first = false;
            }
            if (preserveAspectRatio != null)
            {
                if (first) url += "?preserveAspectRatio=" + preserveAspectRatio.Value.ToString();
                else url += "&preserveAspectRatio=" + preserveAspectRatio.Value.ToString();
                first = false;
            }

            byte[] data;
            try
            {
                data = GetBytesAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 500:
                        throw new HttpException(e.WebEventCode, "Processing error occurred accessing the avatar image", e);
                    default:
                        throw;
                }
            }

            return data;
        }

        /// <summary>
        /// Return the personal blog for the specified user.
        /// </summary>
        /// <param name="personID">ID of the user for which to return a personal blog</param>
        /// <param name="fields">Fields to be returned (default is @all)</param>
        /// <returns>Blog object containing the person's blog</returns>
        public Blog GetBlog(int personID, List<string> fields = null)
        {
            //constucts the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/blog";
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
                        throw new HttpException(e.WebEventCode, "Any input field was malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to retrieve the blog for this user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user or blog does not exist", e);
                    default:
                        throw;
                }
            }

            JObject Json = JObject.Parse(json);
            return Json.ToObject<Blog>();
        }

        /// <summary>
        /// Return a paginated list of Person objects about colleagues of the specified person (i.e. those who report to the same manager that this person does).
        /// </summary>
        /// <param name="personID">ID of the specified Jive user</param>
        /// <param name="count">Maximum number of instances to be returned per Jive HTTP request (i.e. the page size)</param>
        /// <param name="startIndex">Zero-relative index of the first instance to be returned</param>
        /// <param name="fields">Fields to be returned (or null the summary fields)</param>
        /// <returns>a list of Person objects listing the colleagues of the specified user</returns>
        public List<Person> GetColleagues(int personID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> peopleList = new List<Person>();

            //constructs the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/@colleagues";
            url += "?count=" + (count > 100 ? 100 : count).ToString();
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

            //this loop repeats as many times as necessary to retrieve all of the objects
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorized to retrieve this user information", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Specified user cannot be found", e);
                        case 410:
                            throw new HttpException(e.WebEventCode, "Organization Chart relationships are not supported by this Jive instance", e);
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
        /// Return a paginated list of Persons about bidirectionally related users that you and the specified person have in common
        /// (i.e. any person whom you and the specified person are both following, and who is following both you and specified person).
        /// </summary>
        /// <param name="personID">ID of the specified Jive user</param>
        /// <param name="count">Maximum number of instances to be returned per Jive HTTP request (i.e. the page size)</param>
        /// <param name="startIndex">Zero-relative index of the first element returned</param>
        /// <param name="fields">Fields to be returned (or null for summary fields)</param>
        /// <returns>a list of Person objects listing the common bidirectionally related users</returns>
        public List<Person> GetCommonBidirectional(int personID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> peopleList = new List<Person>();

            //constructs the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/@common";
            url += "?count=" + (count > 100 ? 100 : count).ToString();
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

            //this loop repeats as many times as necessary to retrieve all of the objects
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorized to retrieve this user information", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Specified user cannot be found", e);
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

        //GetExtProps()
        //GetExtPropsForAddOn()

        /// <summary>
        /// Return a list of featured content objects for the specified person.
        /// </summary>
        /// <param name="personID">ID of the person for which to retrieve featured content</param>
        /// <param name="types">one or more object types of desired contained content objects (document, discussion, post, poll)</param>
        /// <param name="fields">Fields to be returned on each content</param>
        /// <param name="abridged">Flag indicating that if content.text is requested, it will be abridged (length shortened, HTML tags removed)</param>
        /// <returns>a list of GenericContent objects containing the matched content objects</returns>
        public List<GenericContent> GetFeaturedContent(int personID, List<string> types = null, List<string> fields = null, bool abridged = false)
        {
            List<GenericContent> contentList = new List<GenericContent>();
            var filter = new List<string>();

            if (types != null)
            {
                string typeFilter = "type(";
                foreach (var type in types)
                {
                    typeFilter += type + ",";
                }
                // remove last comma
                typeFilter = typeFilter.Remove(typeFilter.Length - 1);
                typeFilter += ")";
                filter.Add(typeFilter);
            }

            string url = peopleUrl + "/" + personID.ToString() + "/@featuredContent";
            url += "?abridged=" + abridged.ToString();
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

            //this loop repeats as many times as necessary to retrieve all of the objects
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
                            throw new HttpException(e.WebEventCode, "An input field was malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the specified content object", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Specified person does not exist, or their container is missing", e);
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

        /// <summary>
        /// Return the set of fields that can be used for filtering users in this Jive instance.
        /// </summary>
        /// <returns>a list of strings</returns>
        public List<string> GetFilterableFields()
        {
            string url = peopleUrl + "/@filterableFields";

            string json;
            try
            {
                json = GetAbsolute(url);
            }
            catch (HttpException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            json = "{result: " + json + "}";
            JObject results = JObject.Parse(json);
            return results["result"].ToObject<List<string>>();
        }

        /// <summary>
        /// Return a paginated list of Persons about people who are following the specified person.
        /// </summary>
        /// <param name="personID">ID of the specified person</param>
        /// <param name="count">Maximum number of instances to be returned (i.e. the page size)</param>
        /// <param name="startIndex">Zero-relative index of the first instance to be returned</param>
        /// <param name="fields">Fields to be returned (or null for summary fields)</param>
        /// <returns>a list of Person objects listing the people following the specified person</returns>
        public List<Person> GetFollowers(int personID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> peopleList = new List<Person>();

            //constructs the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/@followers";
            url += "?count=" + (count > 100 ? 100 : count).ToString();
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

            //this loop repeats as many times as necessary to retrieve all of the objects
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorized to retrieve this user information", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Specified user cannot be found", e);
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
        /// Return a paginated list of Persons about people the specified person is following.
        /// </summary>
        /// <param name="personID">ID of the specified person</param>
        /// <param name="count">Maximum number of instances to be returned (i.e. the page size)</param>
        /// <param name="startIndex">Zero-relative index of the first instance to be returned</param>
        /// <param name="fields">Fields to be returned (or null for summary fields)</param>
        /// <returns>a list of Person objects listing the people the specified person is following</returns>
        public List<Person> GetFollowing(int personID, int count = 25, int startIndex = 0, List<string> fields = null)
        {
            List<Person> peopleList = new List<Person>();

            //constructs the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/@following";
            url += "?count=" + (count > 100 ? 100 : count).ToString();
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

            //this loop repeats as many times as necessary to retrieve all of the objects
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorized to retrieve this user information", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Specified user cannot be found", e);
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

        //GetFollowingIn()

        /// <summary>
        /// Return a Person describing the followed person, if a following relationship from the specified person exists.
        /// </summary>
        /// <param name="personID">ID of the specified person</param>
        /// <param name="followedPersonID">ID of the followed person (if any)</param>
        /// <param name="fields">Fields to be returned (or null for all fields)</param>
        /// <returns>Person object describing the followed person</returns>
        public Person GetFollowingPerson(int personID, int followedPersonID, List<string> fields = null)
        {
            //constructs the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/@following/" + followedPersonID.ToString();
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
                        throw new HttpException(e.WebEventCode, "Following relationship does not exist between these two users", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to retrieve this user information", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "One or both of the specified users cannot be found", e);
                    default:
                        throw;
                }
            }

            JObject result = JObject.Parse(json);
            return result.ToObject<Person>();
        }

        /// <summary>
        /// Return a Person describing the manager of the specified person.
        /// </summary>
        /// <param name="personID">ID of the specified Jive use</param>
        /// <param name="fields">Fields to be returned (or null for all fields)</param>
        /// <returns>Person object describing the manager of the specified user</returns>
        public Person GetManager(int personID, List<string> fields = null)
        {
            //constructs the url for the HTTP request based on the user specifications
            string url = peopleUrl + "/" + personID.ToString() + "/@manager";
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
                        throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not allowed to retrieve this user information", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user cannot be found", e);
                    case 410:
                        throw new HttpException(e.WebEventCode, "Organization Chart relationships are not supported by this Jive instance", e);
                    default:
                        throw;
                }
            }

            JObject result = JObject.Parse(json);
            return result.ToObject<Person>();
        }

        //GetMetadata()
        //GetNews()

        /// <summary>
        /// Return a list of pages(currently only one) Page that a user has created with parent as user. currently you can only create one page with user as parent
        /// </summary>
        /// <param name="personID">Authenticated user. Use @me or the ID of the authenticated user.</param>
        /// <returns>Page object for the authenticated user</returns>
        public Page GetPages(int personID)
        {
            string url = peopleUrl + "/" + personID.ToString() + "/pages";

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
                        throw new HttpException(e.WebEventCode, "An input field was malformed");
                    case 403:
                        throw new HttpException(e.WebEventCode, "Specified user is not the authenticated user");
                    case 404:
                        throw new HttpException(e.WebEventCode, "Specified user does not exist");
                    default:
                        throw;
                }
            }

            JObject result = JObject.Parse(json);
            return result.ToObject<Page>();
        }

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
        /// <param name="department">Single value to match against the Department profile field.</param>
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
            string sort = "firstNameAsc", string company = null, string department = null, Tuple<DateTime, DateTime?> hireDate = null, bool includeDisabled = false,
            bool includeExternal = false, bool includeOnline = false, bool includePartner = true, Tuple<DateTime, DateTime?> lastProfileUpdate = null, string location = null, bool nameonly = false,
            Tuple<DateTime, DateTime?> published = null, List<string> search = null, List<string> tag = null, string title = null, Tuple<DateTime, DateTime?> updated = null)
        {
            List<Person> peopleList = new List<Person>();

            List<string> filter = new List<string>();
            if (company != null)
            {
                filter.Add("company(" + company + ")");
            }
            if (department != null)
            {
                filter.Add("department(" + department + ")");
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
                            throw new HttpException(e.WebEventCode, "Request criteria are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user information", e);
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
                        throw new HttpException(e.WebEventCode, "Specified ID is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "ID does not identify a valid user", e);
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
                        throw new HttpException(e.WebEventCode, "Specified email address is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "Email address does not identify a valid user", e);
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<Person>();
        }

        //GetPersonByExternalIdentity()

        /// <summary>
        /// Return a Person object describing the requested Jive user by username.
        /// </summary>
        /// <param name="username">Username of the requested Jive user</param>
        /// <param name="fields">Field names to be returned (default is all)</param>
        /// <returns>a Person object representing the requested user</returns>
        public Person GetPersonByUsername(string username, List<string> fields = null)
        {
            string url = peopleUrl + "/username/" + username;
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
                        throw new HttpException(e.WebEventCode, "Specified username is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "Requesting user is not authorize to retrieve this user", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "username does not identify a valid user", e);
                    default:
                        throw;
                }
            }

            JObject results = JObject.Parse(json);

            return results.ToObject<Person>();
        }

        /// <summary>
        /// Return a list of Streams for the specified user. Because the number of streams will generally be very small, pagination is not supported.
        /// </summary>
        /// <param name="personID">ID of the user for whom to return custom streams</param>
        /// <param name="fields">Fields to be returned (default value is "@owned")</param>
        /// <returns>Stream[]</returns>
        public List<Stream> GetStreams(int personID, List<string> fields = null)
        {
            List<Stream> streamList = new List<Stream>();
            
            string url = peopleUrl + "/" + personID + "/streams";
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
                        case 403:
                            throw new HttpException(e.WebEventCode, "Requester is not allowed to view custom streams for the owning user", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "Specified user cannot be found", e);
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                streamList.AddRange(results["list"].ToObject<List<Stream>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return streamList;
        }

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
