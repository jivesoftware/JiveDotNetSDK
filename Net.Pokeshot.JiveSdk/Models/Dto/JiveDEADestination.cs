using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public class JiveDEADestination
    {
        public string @class { get; set; }
        public string objectType { get; set; }
        public int objectId { get; set; }
        public  string objectHash {get;set;}
        public bool isDataAvailable {get;set;}
        public List<string> tags {get;set;}
        public string name {get;set;}
        public string displayName {get;set;}
        public string description {get;set;}
        public int creationDate {get;set;}
        public int modificationDate {get;set;}
        public string url {get;set;}
        public string status {get;set;}
        public bool isVisibleToPartner {get;set;}
        public string parentId {get;set;}
        public string parentType {get;set;}
        public string fullPath {get;set;}
        public string normalizedPath{get;set;}
        public string combinedObjectTypeId {get;set;}
        public object extras { get; set; }
    }
}
