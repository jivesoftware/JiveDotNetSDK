using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAActivityInstance
    {
        public string name { get; set; }
        public int timestamp { get; set; }
        public int seqId { get; set; }
        public string uuid { get; set; }
        public JiveDEAContext context { get; set; }
        public int actorID { get; set; }
        public int actorType { get; set; }
        public string activityType { get; set; }
        public int actionObjectId { get; set; }
        public int actionObjectType { get; set; }
        public JiveDEAActivityInstanceElement activity { get; set; }
        public bool isHistoricalReplay { get; set; }
        public int containerId { get; set; }
        public int containerType { get; set; }
        public object payload { get; set; }
    }
}
