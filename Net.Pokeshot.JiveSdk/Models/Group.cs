using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Group : Place
    {
        public Person creator { get; set; }
        public string displayName { get; set; }
        public bool extendedAuthorsEnabled { get; set; }
        public string groupType { get; set; }
        public string groupTypeNew { get; set; }
        public int memberCount { get; set; }
        public List<string> tags { get; set; }
    }

    public class Invite
    {
        public string body { get; set; }
        public string email { get; set; }
        public string id { get; set; }
        public Person invitee { get; set; }
        public Person inviter { get; set; }
        public Place place { get; set; }
        public DateTime published { get; set; }
        public Object resources { get; set; }
        public DateTime revokeDate { get; set; }
        public Person revoker { get; set; }
        public DateTime sentDate { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
        public bool followed { get; set; }
    }

    public class Member
    {
        public Group group { get; set; }
        public string id { get; set; }
        public Person person { get; set; }
        public DateTime published { get; set; }
        public Object resources { get; set; }
        public string state { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
    }
}
