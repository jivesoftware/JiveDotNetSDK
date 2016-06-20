using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class DeletedObject
    {
        public int containerId { get; set; }
        public string containerType { get; set; }
        public string containerUri { get; set; }
        public DateTime eventDate { get; set; }
        public string id { get; set; }
        public int objectId { get; set; }
        public string objectType { get; set; }
        public Resources.Resources resources { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }
}
