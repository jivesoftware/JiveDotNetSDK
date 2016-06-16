using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class DESClient : JiveClient
    {
        string _desUrl { get { return "https://api.jivesoftware.com/analytics/v2/export"; } }
        string _clientId;
        string _clientSecret;

        public DESClient(string communityUrl, NetworkCredential credentials, string clientId, string clientSecret) : base(communityUrl, credentials) {
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        public void test()
        {
            getAuthorization();
        }

        /// <summary>
        /// Gets the Authorization needed for downloading data from the DES.
        /// </summary>
        /// <returns>string to be used as the authorization header for web request to the des.</returns>
        private string getAuthorization() {
            string url = "https://api.jivesoftware.com/analytics/v1/auth/login?";
            url += "clientId=" + _clientId;
            url += "&clientSecret=" + _clientSecret;

            return PostAbsolute(url, "");
        }
    }
}
