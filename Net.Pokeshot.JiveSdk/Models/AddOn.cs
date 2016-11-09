using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class AddOn
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        //public ??? config { get; set; }
        public string descriptionLocalized { get; set; }
        public string displayNameLocalized { get; set; }
        public string id { get; set; }
        public DateTime published { get; set; }
        public Resources.Resources resources { get; set; }
        public DateTime updated { get; set; }
        public string uuid { get; set; }
    }
}
