using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
     public class Comment
     {
         public int abuseCount { get; set; }
         public Person author { get; set; }
         public ContentBody content { get; set; }
         public List<Image> contentImages { get; set; }
         public List<ContentVideo> contentVideos { get; set; }
         public string externalID {get; set;}
         public int favoriteCount { get; set; }
         public int helpfulCount { get; set; }
         public string highlightBody {get;set;}
         public string highlightSubject {get;set;}
         public string highlightTags {get;set;}
         public string iconCss {get; set;}
         public string id {get;set;}
         public DateTime lastActivityDate { get; set; }
         public int likeCount {get;set;}
         public Object outcomeCounts {get; set;}
         public List<string> outcomeTypeNames {get;set;}
         public List<OutcomeType> outcomeTypes {get;set;}
         public string parent {get;set;}
         public Summary parentContent { get; set; }
         public string parentID { get; set; }
         public Summary parentPlace {get;set;}
         public Boolean parentVisible {get;set;}
         public DateTime published {get;set;}
         public string publishedCalendarDate {get;set;}
         public string publishedTime {get;set;}
         public int replyCount {get;set;}
         public Resources.Resources resources {get;set;}
         public string rootExternalID {get;set;}
         public string rootType {get;set;}
         public string rootURI {get;set;}
         public string status {get;set;}
        public string subject { get; set; }
        public string type {get;set;}
         public DateTime updated {get;set;}
         public int viewCount {get;set;}
     }
}
