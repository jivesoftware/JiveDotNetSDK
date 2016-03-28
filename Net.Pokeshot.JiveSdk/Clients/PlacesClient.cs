using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Net.Pokeshot.JiveSdk.Models;

namespace Net.Pokeshot.JiveSdk.Clients
{
    public class PlacesClient : JiveClient
    {
        string placeUrl { get { return JiveCommunityUrl + "/api/core/v3/places"; } }
        public PlacesClient(string communityUrl, NetworkCredential credentials) : base(communityUrl, credentials) { }

        //GetActivity()
        //GetAppliedEntitlements()
        //GetContent()
        //GetExtProps()
        //GetExtPropsForAddOn()
        //GetFeaturedContent()
        //GetPages()

        JivePlace
    }
}
