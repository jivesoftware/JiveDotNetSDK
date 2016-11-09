using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class ContentVersion
    {
        public Person author { get; set; }
        public string binaryURL { get; set; }
        public GenericContent content { get; set; }
        public string id { get; set; }
        public bool minorVersion { get; set; }
        public DateTime published { get; set; }
        public Resources.Resources resources { get; set; }
        public string status { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
        public int versionNumber { get; set; }
    }
}
