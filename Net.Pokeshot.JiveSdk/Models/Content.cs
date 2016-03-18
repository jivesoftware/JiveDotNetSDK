using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Content
    {
        public Person author { get; set; }
        public List<Attachment> attachments { get; set; }
        public List<string> categories { get; set; }
        public ContentBody content { get; set; }
        public string contentID { get; set; }
        public List<Image> contentImages { get; set; }
        public List<ContentVideo> contentVideos { get; set; }
        public List<Person> extendendAuthors { get; set; }
        public int followerCount { get; set; }
        public string highlightBody { get; set; }
        public string highlightSubject { get; set; }
        public string highlightTags { get; set; }
        public string iconCss { get; set; }
        public string id { get; set; }
        public int likeCount { get; set; }
        public Object outcomeCounts { get; set; }
        public List<string> outcomeTypeNames { get; set; }
        public List<OutcomeType> outcomeTypes { get; set; }
        public string parent { get; set; }
        public Summary parentContent { get; set; }
        public Boolean parentContentVisible { get; set; }
        public Summary parentPlace { get; set; }
        public Boolean parentVisible { get; set; }
        public DateTime published { get; set; }
        public int replyCount { get; set; }
        public Object recources { get; set; }
        public string status { get; set; }
        public string subject { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
        public DateTime updated { get; set; }
        public int viewCount { get; set; }
        public string visibility { get; set; }
        public Boolean visibleToExternalContributors { get; set; }
    }
}