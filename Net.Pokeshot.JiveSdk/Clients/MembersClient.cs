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
    public class MembersClient : JiveClient
    {
        string membersUrl { get { return JiveCommunityUrl + "/api/core/v3/members"; } }

        public MembersClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
        public MembersClient(IJiveUrlAndCredentials jiveUrlAndCredentials) : base(jiveUrlAndCredentials) { }


        //CreateMember
        //DestroyMember

        /// <summary>
        /// Return the specified membership.
        /// </summary>
        /// <param name="memberID">ID of the membership to be returned.</param>
        /// <param name="fields">Fields to include in the returned Member.</param>
        /// <returns>Member</returns>
        public Member GetMember(int memberID, List<string> fields = null)
        {
            string url = membersUrl + "/" + memberID.ToString();
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
                        throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "You are not allowed to perform this operation", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified member cannot be found", e);
                    default:
                        throw;
                }
            }
            JObject results = JObject.Parse(json);

            return results.ToObject<Member>();
        }


        /// <summary>
        /// Retrieve all memberships of a given group that match the specified criteria.
        /// </summary>
        /// <param name="placeID">Group ID of the social group for which to select memberships.</param>
        /// <param name="state">List of states used to filter the returned results (default is all states). Possible values are: 'banned', 'invited', 'member', 'owner' and 'pending'.</param>
        /// <param name="count">Maximum number of memberships to be returned (default is 25).</param>
        /// <param name="startIndex">Zero-relative offset of the first membership to be returned (default is 0).</param>
        /// <param name="filter">The filter criteria used to select memberships</param>
        /// <param name="fields">Fields to be returned in the selected memberships.</param>
        /// <returns>Member[] of memberships of a given group.</returns>
        public List<Member> GetMembersByGroup(int placeID, List<string> state = null, int count = 25, int startIndex = 0, List<string> filter = null, List<string> fields = null)
        {
            List<Member> memberList = new List<Member>();

            string url = membersUrl + "/places/" + placeID.ToString();

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
            if (filter != null && filter.Count > 0)
            {
                foreach (var item in filter)
                {
                    url += "&filter=" + item;
                }
            }
            if (state != null && state.Count > 0)
            {
                url += "&state=";
                foreach (var field in state)
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
                            throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the requested members", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified place is not found", e);
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                memberList.AddRange(results["list"].ToObject<List<Member>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return memberList;
        }


        /// <summary>
        /// Retrieve all memberships of a given person that match the specified criteria.
        /// </summary>
        /// <param name="personID">ID of the person for which to select memberships.</param>
        /// <param name="state">List of states used to filter the returned results (default is all states). Possible values are: 'banned', 'invited', 'member', 'owner' and 'pending'.</param>
        /// <param name="count">Maximum number of memberships to be returned (default is 25).</param>
        /// <param name="startIndex">Zero-relative offset of the first membership to be returned (default is 0).</param>
        /// <param name="filter">The filter criteria used to select memberships</param>
        /// <param name="fields">Fields to be returned in the selected memberships.</param>
        /// <returns>Member[] of memberships of a given group.</returns>
        public List<Member> GetMembersByPerson(int personID, List<string> state = null, int count = 25, int startIndex = 0, List<string> filter = null, List<string> fields = null)
        {
            List<Member> memberList = new List<Member>();

            string url = membersUrl + "/people/" + personID.ToString();

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
            if (filter != null && filter.Count > 0)
            {
                foreach (var item in filter)
                {
                    url += "&filter=" + item;
                }
            }
            if (state != null && state.Count > 0)
            {
                url += "&state=";
                foreach (var field in state)
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
                            throw new HttpException(e.WebEventCode, "An input field is malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "You are not allowed to access the requested members", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified place is not found", e);
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                memberList.AddRange(results["list"].ToObject<List<Member>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return memberList;
        }

        //UpdateMember
    }
}
