using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    /// <summary>
    /// GenericPlace can be used for any of the Place types. It has all of the properties.
    /// </summary>
    public class GenericPlace : Place
    {
        // Properties from Space:
        public int childCount { get; set; }
        public string displayName { get; set; }
        public string locale { get; set; }
        public string parent { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }

        // Properties from Project:
        public Person creator { get; set; }
        public DateTime dueDate { get; set; }
        //public string locale { get; set; }
        //public string parent { get; set; }
        public string projectStatus { get; set; }
        //public List<string> tags { get; set; }
        //public string type { get; set; }

        // Properties from Blog:
        //public string displayName { get; set; }
        //public string type { get; set; }

        // Properties from Carousel:
        //public string displayName { get; set; }
        //public string parent { get; set; }

        // Properties from Group:
        //public Person creator { get; set; }
        //public string displayName { get; set; }
        public bool extendedAuthorsEnabled { get; set; }
        public string groupType { get; set; }
        public string groupTypeNew { get; set; }
        public int memberCount { get; set; }
        //public List<string> tags { get; set; }
    }
}
