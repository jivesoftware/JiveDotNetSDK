using System;
using System.Collections.Generic;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class SecurityGroup
    {
        public int administratorCount { get; set; }
        public string description { get; set; }
        public bool federated { get; set; }
        public string id { get; set; }
        public int memberCount { get; set; }
        public string name { get; set; }
        public object properties { get; set; }
        public DateTime published { get; set; }
        public Resources.Resources resources { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
    }
}
