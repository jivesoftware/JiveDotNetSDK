using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class InboxEntry
    {
        public PersonActivityObject actor { get; set; }
        public string content { get; set; }
        public ActivityObject generator { get; set; }
        public MediaLink icon { get; set; }
        public string id { get; set; }
        public JiveExtension jive { get; set; }
        public ActivityObject @object { get; set; }
        public OpenSocial openSocial { get; set; }
        public MediaLink previewImage { get; set; }
        public ActivityObject provider { get; set; }
        public DateTime published { get; set; }
        public ActivityObject target { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
        public string url { get; set; }
        public string verb { get; set; }
    }

    public class ActionEntry
    {
        public string id { get; set; }
        public string label { get; set; }
        public int sequence { get; set; }
        public string url { get; set; }
    }

    public class Mention
    {
        public string id { get; set; }
        public GenericContent mentionedContent { get; set; }
        public Metadata.Object mentionedObject { get; set; }
        public Resources.Resources resources { get; set; }
        public string type { get; set; }
    }
}
