using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Action
    {
        public List<ActionEntry> actions { get; set; }
        public string body { get; set; }
        public string entryState { get; set; }
        public string entrySubtype { get; set; }
        public string entryType { get; set; }
        public string id { get; set; }
        public DateTime published { get; set; }
        public string requestMessage { get; set; }
        public Resources.Resources resources { get; set; }
        public string summary { get; set; }
        public Metadata.Object target { get; set; }
        public string title { get; set; }
        public string type { get; set; }
    }
}
