﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEAActivityInstance
    {
        public string name { get; set; }
        public long timestamp { get; set; }
        public int seqId { get; set; }
        public string uuid { get; set; }
        public JiveDEAContext context { get; set; }
        public long actorID { get; set; }
        public ObjectType actorType { get; set; }
        public string activityType { get; set; }
        public int actionObjectId { get; set; }
        public ObjectType actionObjectType { get; set; }
        public JiveDEAActivityInstanceElement activity { get; set; }
        public bool isHistoricalReplay { get; set; }
        public int containerId { get; set; }
        public ObjectType containerType { get; set; }
        public object payload { get; set; }
    }
}
