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
    public class SecurityGroupsClient : JiveClient
    {
        string securityGroupsUrl { get { return JiveCommunityUrl + "/api/core/v3/securityGroups"; } }

        public SecurityGroupsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }


        /// <summary>
        /// Return the specified fields in a paginated list of security groups that match the specified selection criteria (or all security groups if no criteria are specified).
        /// This service supports the following filters.Parameters, when used, should be wrapped in parentheses, and multiple values separated by commas. See the examples for clarification.
        /// </summary>
        /// <param name="updated">One or two DateTimes. If one timestamp is specified, all security groups updated since that timestamp will be selected. If two timestamps are specified, all security groups updated in the specified range will be selected.</param>
        /// <param name="count">Maximum number of security groups to be returned (default value is 25)</param>
        /// <param name="startIndex">Zero-relative index of the first matching security group to be returned (default value is 0)</param>
        /// <param name="fields">Fields to be returned for each security group (default value is @standard)</param>
        /// <param name="sort">Optional sort to apply to the search results. Since 3.6.</param>
        /// <returns></returns>
        public List<SecurityGroup> GetSecurityGroups(Tuple<DateTime, DateTime?> updated, int count = 25, int startIndex = 0, List<string> fields = null, string sort = "nameAsc")
        {
            List<SecurityGroup> securityGroupsList = new List<SecurityGroup>();

            List<string> filter = new List<string>();
           
            if (updated != null)
            {
                string dateString = "updated(";
                dateString += jiveDateFormat(updated.Item1);
                dateString += (updated.Item2 != null ? ("," + jiveDateFormat(updated.Item2.Value)) : "") + ")";
                filter.Add(dateString);
            }

            string url = securityGroupsUrl;
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
                            throw new HttpException(e.WebEventCode, "Any of the input fields are malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "The requesting user is not authorized to retrieve security groups", e);
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                securityGroupsList.AddRange(results["list"].ToObject<List<SecurityGroup>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return securityGroupsList;
        }

        public List<Person> GetMembers(int securityGroupID, int startIndex = 0, int count = 25, List<string> fields = null)
        {
            List<Person> memberList = new List<Person>();

            string url = securityGroupsUrl + "/" + securityGroupID + "/members";
            url += "?startIndex=" + startIndex.ToString();
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
                            throw new HttpException(e.WebEventCode, "An input field is missing or malformed", e);
                        case 403:
                            throw new HttpException(e.WebEventCode, "The requesting user is not authorized to retrieve security groups information", e);
                        case 404:
                            throw new HttpException(e.WebEventCode, "The specified security group does not exist", e);
                        default:
                            throw;
                    }
                }

                JObject results = JObject.Parse(json);

                memberList.AddRange(results["list"].ToObject<List<Person>>());

                if (results["links"] == null || results["links"]["next"] == null)
                    break;
                else
                    url = results["links"]["next"].ToString();
            }
            return memberList;
        }
    }
}
