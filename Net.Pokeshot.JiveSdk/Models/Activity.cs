using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Activity
    {
        public PersonActivityObject actor { get; set; }
        public string content { get; set; }
        public ActivityObject generator { get; set; }
        public MediaLink icon { get; set; }
        public string id { get; set; }
        public JiveExtension jive { get; set; }
        public ActivityObject @object { get; set; }
        public OpenSocial openSocial { get; set; }
        public ActivityObject provider { get; set; }
        public DateTime published { get; set; }
        public ActivityObject target { get; set; }
        public string title { get; set; }
        public DateTime updated { get; set; }
        public string url { get; set; }
        public string verb { get; set; }
        public string prettyDate { get; set; }
    }

    public class ActivityObject
    {
        public ActivityObject author { get; set; }
        public string content { get; set; }
        public string displayName { get; set; }

        public string id { get; set; }
        public MediaLink image { get; set; }
        public string objectType { get; set; }
        public DateTime published { get; set; }
        public string summary { get; set; }
        public DateTime updated { get; set; }
        public string url { get; set; }
    }

    public class MediaLink
    {
        public int duration { get; set; }
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }
    public class JiveExtension
    {
        public string answer { get; set; }
        public bool canComment { get; set; }
        public bool canLike { get; set; }
        public bool canReply { get; set; }
        public string collection { get; set; }
        public bool collectionRead { get; set; }
        public DateTime collectionUpdated { get; set; }
        public string display { get; set; }
        public bool followingInStream { get; set; }
        public string iconCss { get; set; }
        public int likeCount { get; set; }
        public bool liked { get; set; }
        public ActivityObject mentioned { get; set; }
        public int objectId { get; set; }
        public OnBehalfOf onBehalfOf { get; set; }
        public string outcomeComment { get; set; }
        public string outcomeName { get; set; }
        public ActivityObject parent { get; set; }
        public ActivityObject parentActor { get; set; }
        public bool parentLiked { get; set; }
        public OnBehalfOf parentOnBehalfOf { get; set; }
        public int parentReplyCount { get; set; }
        public bool question { get; set; }
        public bool read { get; set; }
        public int replyCount { get; set; }
        public string resolved { get; set; }
        public string state { get; set; }
        public string update { get; set; }
        public string updateCollection { get; set; }
        public Via via { get; set; }
    }
    public class OnBehalfOf
    {
        public string email { get; set; }
        public string name { get; set; }
        public Person person { get; set; }
    }
    public class Via
    {
        public string displayName { get; set; }
        public string url { get; set; }
    }
    public class OpenSocial
    {
        public List<ActionLink> actionLinks { get; set; }
        public List<string> deliverTo { get; set; }
        public Embedded embed { get; set; }
    }
    public class ActionLink
    {
        public string caption { get; set; }
        public string httpVerb { get; set; }
        public string target { get; set; }
    }
    public class Embedded
    {
        public string gadget { get; set; }
        public string previewImage { get; set; }
        public string url { get; set; }
    }
    public class PersonActivityObject
    {
        public string displayName { get; set; }
        public string email { get; set; }
        public string id { get; set; }
        public MediaLink image { get; set; }
        public JivePerson jive { get; set; }
        public Name name { get; set; }
        public string objectType { get; set; }
        public DateTime published { get; set; }
        public DateTime updated { get; set; }
        public string url { get; set; }
    }
}
