using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Project : Place
    {
        public Person creator { get; set; }
        public DateTime dueDate { get; set; }
        public string locale { get; set; }
        public string parent { get; set; }
        public string projectStatus { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
    }
}
