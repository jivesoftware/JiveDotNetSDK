using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Web;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class StreamsClient : JiveClient
    {
        private string streamUrl { get { return JiveCommunityUrl + "/api/core/v3/streams"; } }

        public StreamsClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }
        public StreamsClient(IJiveUrlAndCredentials jiveUrlAndCredentials) : base(jiveUrlAndCredentials) { }


        public void CreateAssociations(int streamID, List<Uri> associations)
        {
            if (associations.Count > 200)
                throw new Exception("No more than 200 Uri's can be specified in one CreateAssociations Call.");

            string url = streamUrl + "/" + streamID + "/associations";

            string json = JsonConvert.SerializeObject(associations, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            try
            {
                PostAbsolute(url, json);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "The syntax of the stream ID or one of the object URIs is invalid", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requester is not allowed to manage associations for this custom stream (i.e. not stream owner or a Jive admin)", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified stream or one of the specified objects is not found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Requesting to create an association that already exists", e);
                }
            }
        }

        public void DestroyAssociation(int streamId, string objectType, int objectId)
        {
            var url = streamUrl + "/" + streamId + "/associations/" + objectType + "/" + objectId;

            try
            {
                DeleteAbsolute(url);
            }
            catch (HttpException e)
            {
                switch (e.GetHttpCode())
                {
                    case 400:
                        throw new HttpException(e.WebEventCode, "The syntax of the stream ID, object type, or object ID is invalid", e);
                    case 403:
                        throw new HttpException(e.WebEventCode, "The requester is not allowed to manage associations for this custom stream (i.e. not stream owner or a Jive admin)", e);
                    case 404:
                        throw new HttpException(e.WebEventCode, "The specified stream or one of the specified objects is not found", e);
                    case 409:
                        throw new HttpException(e.WebEventCode, "Requesting to remove an association that does not already exist", e);
                }
            }
        }

        //DestoryStream()
        //GetActivity()
        //GetActivityCount()
        //GetAssociation()
        //GetAssociations()
        //GetConnectionsActivity()
        //GetStream()
        //UpdateStream()
    }
}
