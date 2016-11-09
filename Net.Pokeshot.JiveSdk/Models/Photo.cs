using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Photo : Content
    {
        public int abuseCount { get; set; }
        public List<Person> authors { get; set; }
        public string authorship { get; set; }
        public ContentBanner banner { get; set; }
        public string binaryURL { get; set; }
        public Person editingBy { get; set; }
        public int favoriteCount { get; set; }
        public bool followed { get; set; }
        public string name { get; set; }
        public OnBehalfOf onBehalfOn { get; set; }
        public bool promotedResult { get; set; }
        public bool restrictComments { get; set; }
        public int size { get; set; }
        public OnBehalfOf updatedOnBehalfOf { get; set; }
        public Person updater { get; set; }
        public List<Person> users { get; set; }
        public int version { get; set; }
        public Via via { get; set; }
    }
}
