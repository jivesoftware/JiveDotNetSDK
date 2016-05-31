using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;


namespace Net.Pokeshot.JiveSdk.Clients
{
    public abstract class JiveClient
    {
        private NetworkCredential _credential;
        protected string JiveCommunityUrl;

        public JiveClient(string communityUrl, NetworkCredential cred)
        {
            JiveCommunityUrl = communityUrl;
            _credential = cred;
        }

        protected string GetAbsolute(string url)
        {
            HttpClientHandler jiveHandler = new HttpClientHandler();

            //Setting credentials for our request. This needs to be done for every request as there are no persistent sessions for the REST Api  
            _credential.Domain = JiveCommunityUrl + "/api/core/v3";
            //Getting our credentials in Base64 encoded format  
            string cre = String.Format("{0}:{1}", _credential.UserName, _credential.Password);
            byte[] bytes = Encoding.UTF8.GetBytes(cre);
            string base64 = Convert.ToBase64String(bytes);
            //Set credentials and make sure we are pre-authenticating our request  
            jiveHandler.Credentials = _credential;
            jiveHandler.PreAuthenticate = true;
            jiveHandler.UseDefaultCredentials = true;

            HttpClient httpClient = new HttpClient(jiveHandler);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);


            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage activityResponse = httpClient.SendAsync(requestMessage).Result;

            if (!activityResponse.IsSuccessStatusCode)
            {
                // Calling methods should handle this exception on a case by case basis.
                // The exception contains the returned status code. Jive documentation describes what to do in the case of each code.
                string message = "Jive Request Failed. Got response " + ((int)activityResponse.StatusCode).ToString() + " " + activityResponse.StatusCode +
                    " when making GET request to " + url + "\nUsername: " + _credential.UserName + "\nPassword: " + _credential.Password;
                throw new HttpException((int)activityResponse.StatusCode, message);
            }
            
            String myActivityResponse = activityResponse.Content.ReadAsStringAsync().Result;
            //Remove the string Jive includes in every response from the REST API  
            string cleanResponseActivities = myActivityResponse.Replace("throw 'allowIllegalResourceCall is false.';", "");


            return cleanResponseActivities;
        }

        //PostAbsolute takes its JSON content as a string, similar to the string returned by the GetAbsolute method
        public string PostAbsolute(string url, string json)
        {
            HttpClientHandler jiveHandler = new HttpClientHandler();

            //Setting credentials for our request. This needs to be done for every request as there are no persistent sessions for the REST Api  
            _credential.Domain = JiveCommunityUrl + "/api/core/v3";
            //Getting our credentials in Base64 encoded format  
            string cre = String.Format("{0}:{1}", _credential.UserName, _credential.Password);
            byte[] bytes = Encoding.UTF8.GetBytes(cre);
            string base64 = Convert.ToBase64String(bytes);
            //Set credentials and make sure we are pre-authenticating our request  
            jiveHandler.Credentials = _credential;
            jiveHandler.PreAuthenticate = true;
            jiveHandler.UseDefaultCredentials = true;

            HttpClient httpClient = new HttpClient(jiveHandler);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage activityResponse = httpClient.SendAsync(requestMessage).Result;

            if (!activityResponse.IsSuccessStatusCode)
            {
                // Calling methods should handle this exception on a case by case basis.
                // The exception contains the returned status code. Jive documentation describes what to do in the case of each code.
                string message = "Jive Request Failed. Got response " + ((int)activityResponse.StatusCode).ToString() + " " + activityResponse.StatusCode +
                    " when making POST request to " + url + "\nUsername: " + _credential.UserName + "\nPassword: " + _credential.Password;
                throw new HttpException((int)activityResponse.StatusCode, message);
            }

            String myActivityResponse = activityResponse.Content.ReadAsStringAsync().Result;
            //Remove the string Jive includes in every response from the REST API  
            string cleanResponseActivities = myActivityResponse.Replace("throw 'allowIllegalResourceCall is false.';", "");

            return cleanResponseActivities;
        }

        protected string PutAbsolute(string url, string json)
        {
            return ""; 
        }

        protected string jiveDateFormat(DateTime time)
        {
            return time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fff") + "%2B0000";
        }
    }
}
