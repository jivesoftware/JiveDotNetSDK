using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAActor
    {
        public string @class { get; set; }    
        public string objectType {get;set;}
        public int objectId { get; set; }
        public string objectHash { get; set; }
        public bool isDataAvailable { get; set; }
        public List<string> tags { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int creationDate { get; set; }
        public int modificationDate { get; set; }
        public JiveDEAActorProfile profile { get; set; }
        public bool enabled { get; set; }
        public int lastLoggedIn { get; set; }
        public int lastProfileUpdate { get; set; }
        public string type { get; set; }
        public bool federated { get; set; }
        public bool visible { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public object extras { get; set; }

    }
}
