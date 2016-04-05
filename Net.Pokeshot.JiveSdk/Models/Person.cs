using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Person
    {
        public string displayName { get; set; }
        public int followerCount { get; set; }
        public int followingCount { get; set; }
        public int id { get; set; }
        public string location { get; set; }
        public string status { get; set; }
        public string thumbnailId { get; set; }
        public string thumbnailUrl { get; set; }
        public DateTime updated { get; set; }
        public List<ProfileEntry> emails { get; set; }
        public JivePerson jive { get; set; }
        public Name name { get; set; }
        public List<ProfileEntry> phoneNumbers { get; set; }
        public List<ProfileEntry> photos { get; set; }
        public DateTime published { get; set; }
        public List<string> tags { get; set; }
        public JiveResources resources { get; set; }


    }

    public class JivePerson
    {
        public bool enabled { get; set; }
        public bool external { get; set; }
        public bool externalContributor { get; set; }
        public List<ExternalIdentity> externalIdentities { get; set; }
        public bool federated { get; set; }
        public DateTime lastAuthenticated { get; set; }
        public DateTime lastProfileUpdate { get; set; }
        public Level level { get; set; }
        public string locale { get; set; }
        public string password { get; set; }
        public List<ProfileEntry> profile { get; set; }
        public RoleBadge roleBadge { get; set; }
        public bool sendeable { get; set; }
        public bool termsAndConditionsRequired { get; set; }
        public string timeZone { get; set; }
        public string username { get; set; }
        public bool viewContent { get; set; }
        public bool visible { get; set; }
    }

    public class RoleBadge
    {
        public string groupBadgeRoleName { get; set; }
        public string groupBadgeUri { get; set; }
        public string id { get; set; }
        public Resources.Resources resources { get; set; }
    }

    public class Level
    {
        public string description { get; set; }
        public string imageURI { get; set; }
        public string name { get; set; }
        public int points { get; set; }
    }

    public class ProfileEntry
    {
        public string jive_label { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public bool primary { get; set; }
    }
    public class Name
    {
        public string familyName { get; set; }
        public string formatted { get; set; }
        public string givenName { get; set; }
    }
    public class ExternalIdentity
    {
        public string identity { get; set; }
        public string identityType { get; set; }
    }
}
