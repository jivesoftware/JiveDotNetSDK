using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Pokeshot.JiveSdk.Models
{
    public class Discussion : Content
    {
        public string answer { get; set; }
        public List<string> helpful { get; set; }
        public OnBehalfOf onBehalfOf { get; set; }
        public bool question { get; set; }
        public string resolved { get; set; }
        public bool restrictReplies { get; set; }


        public static explicit operator Discussion(GenericContent gc)
        {
            return new Discussion {
                answer = gc.answer,
                attachments = gc.attachments,
                author = gc.author,
                categories = gc.categories,
                content = gc.content,
                contentID = gc.contentID,
                contentImages = gc.contentImages,
                contentVideos = gc.contentVideos,
                extendedAuthors = gc.extendedAuthors,
                followerCount = gc.followerCount,
                helpful = gc.helpful,
                highlightBody = gc.highlightBody,
                highlightSubject = gc.highlightSubject,
                highlightTags = gc.highlightTags,
                iconCss = gc.iconCss,
                id = gc.id,
                likeCount = gc.likeCount,
                onBehalfOf = gc.onBehalfOf,
                outcomeCounts = gc.outcomeCounts,
                outcomeTypeNames = gc.outcomeTypeNames,
                outcomeTypes = gc.outcomeTypes,
                parent = gc.parent,
                parentContent = gc.parentContent,
                parentContentVisible = gc.parentContentVisible,
                parentPlace = gc.parentPlace,
                parentVisible = gc.parentVisible,
                published = gc.published,
                question = gc.question,
                replyCount = gc.replyCount,
                resolved = gc.resolved,
                resources = gc.resources,
                restrictReplies = gc.restrictReplies,
                status = gc.status,
                subject = gc.subject,
                tags = gc.tags,
                type = gc.type,
                updated = gc.updated,
                viewCount = gc.viewCount,
                visibility = gc.visibility,
                visibleToExternalContributors = gc.visibleToExternalContributors
            };
        }
    }
}
