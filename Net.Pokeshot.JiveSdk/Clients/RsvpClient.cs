using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Net.Pokeshot.JiveSdk.Models;
using System.Web;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class RsvpClient : JiveClient
    {
        string rsvpUrl { get { return JiveCommunityUrl + "/api/core/ext/event-type-plugin/v3/rsvp"; } }

        public RsvpClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        public string Create(int contentID, Person person, RsvpResponse response)
        {
            string json = "" + (int) response;
            string url = rsvpUrl + "/" + contentID.ToString();
            string result;

            try
            {
                result = RunAs(person.id, () => PostAbsolute(url, json));
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
                        throw new HttpException(e.WebEventCode, "The specified parent content object (or comment) cannot be found", e);
                    default:
                        throw;
                }
            }
            return result;
        }
    }

    public enum RsvpResponse
    {
        Yes = 1,
        No = 2,
        Maybe = 3
    }
}
