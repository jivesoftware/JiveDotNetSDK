using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Stream
    {
        public int count { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public DateTime newUpdates { get; set; }
        public Person person { get; set; }
        public DateTime previousUpdates { get; set; }
        public DateTime published { get; set; }
        public bool receiveEmails { get; set; }
        public Resources.Resources resources { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
        public string source { get; set; }
    }
}
