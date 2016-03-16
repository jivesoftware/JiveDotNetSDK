using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Net.Pokeshot.JiveSdk
{
    // Singleton class that will use the same credentials and baseUrl for all calls
    public sealed class JiveRetrieval
    {
        private static readonly JiveRetrieval instance = new JiveRetrieval();

        // having a private constructor stops others from instantiating the class
        private JiveRetrieval() { Credential = null; JiveCommunityUrl = null; }

        public static JiveRetrieval Instance
        {
            get
            {
                return instance;
            }
        }


        public NetworkCredential Credential { get; set; }
        public string JiveCommunityUrl { get; set; }


        public string ExecuteAbsolute(string url)
        {
            HttpClientHandler jiveHandler = new HttpClientHandler();

            //Setting credentials for our request. This needs to be done for every request as there are no persistent sessions for the REST Api  
            Credential.Domain = JiveCommunityUrl + "/api/core/v3";
            //Getting our credentials in Base64 encoded format  
            string cre = String.Format("{0}:{1}", Credential.UserName, Credential.Password);
            byte[] bytes = Encoding.UTF8.GetBytes(cre);
            string base64 = Convert.ToBase64String(bytes);
            //Set credentials and make sure we are pre-authenticating our request  
            jiveHandler.Credentials = Credential;
            jiveHandler.PreAuthenticate = true;
            jiveHandler.UseDefaultCredentials = true;

            HttpClient httpClient = new HttpClient(jiveHandler);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);


            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage activityResponse = httpClient.SendAsync(requestMessage).Result;
            String myActivityResponse = activityResponse.Content.ReadAsStringAsync().Result;
            //Remove the string Jive includes in every response from the REST API  
            string cleanResponseActivities = myActivityResponse.Replace("throw 'allowIllegalResourceCall is false.';", "");


            return cleanResponseActivities;
        }
    }
}
