using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models.Metadata
{
    public class Field
    {
        public bool array { get; set; }
        public string availability { get; set; }
        public string description { get; set; }
        public string displayName { get; set; }
        public bool editable { get; set; }
        public bool filterable { get; set; }
        public string name { get; set; }
        public bool newsEnabled { get; set; }
        public List<string> options { get; set; }
        public bool required { get; set; }
        public string since { get; set; }
        public string type { get; set; }
        public bool unpublished { get; set; }
    }
}
