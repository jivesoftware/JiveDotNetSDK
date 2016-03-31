using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Message
    {
        public int abuseCount { get; set; }
        public bool answer { get; set; }
        public int answeredQuestionCount { get; set; }
        public List<Attachment> attachments { get; set; }
        public Person author { get; set; }
        public ContentBody content { get; set; }
        public List<Image> contentImages { get; set; }
        public List<ContentVideo> contentVideos { get; set; }
        public string discussion { get; set; }
        public int favoriteCount { get; set; }
        public int followerCount { get; set; }
        public string fromQuest { get; set; }
        public bool helpful { get; set; }
        public int helpfulCount { get; set; }
        public string highlightBody { get; set; }
        public string highlightSubject { get; set; }
        public string highlightTags { get; set; }
        public string iconCss { get; set; }
        public string id { get; set; }
        public DateTime lastActivityDate { get; set; }
        public int likeCount { get; set; }
        public string messageTarget { get; set; }
        public OnBehalfOf onBehalfOf { get; set; }
        public Object outcomeCounts { get; set; }
        public List<string> outcomeTypeNames { get; set; }
        public List<OutcomeType> outcomeTypes { get; set; }
        public string parent { get; set; }
        public Summary parentContent { get; set; }
        public bool parentContentVisible { get; set; }
        public Summary parentPlace { get; set; }
        public bool parentVisible { get; set; }
        public DateTime published { get; set; }
        public bool question { get; set; }
        public int replyCount { get; set; }
        public Resources.Resources resources { get; set; }
        public Object searchRankings { get; set; }
        public string status { get; set; }
        public string subject { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
        public int unhelpfulCount { get; set; }
        public DateTime updated { get; set; }
        public Via via { get; set; }
        public int viewCount { get; set; }
        public bool visibleToExternalContributors { get; set; }
        public bool canMarkHelpful { get; set; }
        public bool canMarkUnHelpful { get; set; }
        public bool followed { get; set; }
        public bool hasVotedHelpful { get; set; }
        public bool hasVotedUnHelpful { get; set; }
        public bool promotedResult { get; set; }
        public bool unHelpfulEnabled { get; set; }
    }
}
