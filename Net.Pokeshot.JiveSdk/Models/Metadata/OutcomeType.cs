using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Pokeshot.JiveSdk.Models;

namespace Net.Pokeshot.JiveSdk.Models.Metadata
{
    public class OutcomeType
    {
        public string alternativeGrouping { get; set; }
        public string communityAudience { get; set; }
        public string confirmContentEdit { get; set; }
        public bool confirmExclusion { get; set; }
        public bool confirmUnmark { get; set; }
        public List<Metadata.Field> fields { get; set; }
        public bool generalNote { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool noteRequired { get; set; }
        public Resources.Resources resources { get; set; }
        public bool shareable { get; set; }
        public string type { get; set; }
        public bool urlAllowed { get; set; }
    }
}
