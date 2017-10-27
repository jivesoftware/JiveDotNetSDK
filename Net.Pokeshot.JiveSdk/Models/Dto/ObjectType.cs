using System.Collections;
using System.Linq;

namespace Net.Pokeshot.JiveSdk.Models.Dto
{
    public static class ObjectTypeExtensions
    {
        private readonly static ObjectType[] _contentTypes = {ObjectType.THREAD, ObjectType.DOCUMENT, ObjectType.QUESTION, ObjectType.POLL, ObjectType.TASK,
            ObjectType.FAVORITE, ObjectType.VIDEO, ObjectType.IDEA, ObjectType.DIRECTMESSAGE, ObjectType.BLOGPOST, ObjectType.EVENT, ObjectType.WALLENTRY};
        private readonly static ObjectType[] _placeTypes = { ObjectType.COMMUNITY, ObjectType.BLOG, ObjectType.PROJECT, ObjectType.SOCIAL_GROUP};
        private readonly static ObjectType[] _personTypes = { ObjectType.USER };

        public static bool IsContent(this ObjectType type)
        {
            return _contentTypes.Contains(type);
        }

        public static bool IsPerson(this ObjectType type)
        {
            return _personTypes.Contains(type);
        }

        public static bool IsPlace(this ObjectType type)
        {
            return _placeTypes.Contains(type);
        }

        /// <summary>
        /// Returns the name the type is known as in the Jive API.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAlias(this ObjectType type)
        {
            switch (type)
            {
                case ObjectType.THREAD:
                    return "discussion";
                case ObjectType.MESSAGE:
                    return "message";
                case ObjectType.USER:
                    return "person";
                case ObjectType.GROUP:
                    goto default;
                case ObjectType.THREAD_NAME:
                    goto default;
                case ObjectType.MESSAGE_SUBJECT:
                    goto default;
                case ObjectType.MESSAGE_BODY:
                    goto default;
                case ObjectType.CREATION_DATE:
                    goto default;
                case ObjectType.MODIFICATION_DATE:
                    goto default;
                case ObjectType.EXTENDED_PROPERTY:
                    goto default;
                case ObjectType.ANONYMOUS:
                    goto default;
                case ObjectType.REGISTERED_USERS:
                    goto default;
                case ObjectType.ATTACHMENT:
                    goto default;
                case ObjectType.COMMUNITY:
                    return "space";
                case ObjectType.COMMUNITY_NAME:
                    goto default;
                case ObjectType.SYSTEM:
                    goto default;
                case ObjectType.POLL:
                    return "poll";
                case ObjectType.COMMUNITY_SEARCH_QUERY:
                    goto default;
                case ObjectType.PRIVATE_MESSAGE:
                    goto default;
                case ObjectType.PRIVATE_MESSAGE_FOLDER:
                    goto default;
                case ObjectType.ANNOUNCEMENT:
                    goto default;
                case ObjectType.SEARCH:
                    goto default;
                case ObjectType.CRONTASK:
                    goto default;
                case ObjectType.STATUS_LEVEL:
                    goto default;
                case ObjectType.AVATAR:
                    goto default;
                case ObjectType.QUESTION:
                    goto default;
                case ObjectType.COMMUNITY_QUESTION:
                    goto default;
                case ObjectType.SYSTEM_QUESTION:
                    goto default;
                case ObjectType.GATEWAY:
                    goto default;
                case ObjectType.BAN:
                    goto default;
                case ObjectType.ABUSE:
                    goto default;
                case ObjectType.WATCH_SETTINGS:
                    goto default;
                case ObjectType.DRAFT:
                    goto default;
                case ObjectType.BLOG:
                    return "blog";
                case ObjectType.BLOGPOST:
                    return "post";
                case ObjectType.TRACKBACK:
                    goto default;
                case ObjectType.TAG:
                    goto default;
                case ObjectType.TAG_SET:
                    goto default;
                case ObjectType.CONTENT_OBJECT:
                    goto default;
                case ObjectType.STATUS_LEVEL_POINT:
                    goto default;
                case ObjectType.STATUS_LEVEL_SCENARIO:
                    goto default;
                case ObjectType.WATCHABLE_OBJECT:
                    goto default;
                case ObjectType.SEARCH_ENGINE:
                    goto default;
                case ObjectType.USER_STATUS:
                    goto default;
                case ObjectType.USER_RELATIONSHIP:
                    goto default;
                case ObjectType.USER_RELATIONSHIP_GRAPH:
                    goto default;
                case ObjectType.PROJECT_QUESTION:
                    goto default;
                case ObjectType.SOCIAL_GROUP_QUESTION:
                    goto default;
                case ObjectType.USER_RELATIONSHIP_LIST:
                    goto default;
                case ObjectType.PLUGIN:
                    goto default;
                case ObjectType.DOCUMENT:
                    return "document";
                case ObjectType.DOCUMENT_TITLE:
                    goto default;
                case ObjectType.DOCUMENT_FIELD:
                    goto default;
                case ObjectType.COMMENT:
                    goto default;
                case ObjectType.EXPIRATION_DATE:
                    goto default;
                case ObjectType.RATING:
                    goto default;
                case ObjectType.DOCUMENT_TYPE:
                    goto default;
                case ObjectType.SEARCH_QUERY:
                    goto default;
                case ObjectType.DOCUMENT_BODY:
                    goto default;
                case ObjectType.IMAGE:
                    goto default;
                case ObjectType.DOCUMENT_PRESENTER:
                    goto default;
                case ObjectType.DOCUMENT_STATE:
                    goto default;
                case ObjectType.DOCUMENT_FIELD_OPTION:
                    goto default;
                case ObjectType.DOCUMENT_VERSION:
                    goto default;
                case ObjectType.DOCUMENT_VERSION_COMMENT:
                    goto default;
                case ObjectType.DOCUMENT_FIELD_TYPE:
                    goto default;
                case ObjectType.DOCUMENT_ID:
                    goto default;
                case ObjectType.DOCUMENT_TYPE_ELEMENT:
                    goto default;
                case ObjectType.TEMPORARY_DOCUMENT_ID:
                    goto default;
                case ObjectType.DOCUMENT_BACKCHANNEL:
                    goto default;
                case ObjectType.READ_STAT_SESSION:
                    goto default;
                case ObjectType.READ_STAT:
                    goto default;
                case ObjectType.WORKGROUP_AGENT:
                    goto default;
                case ObjectType.WORKGROUP_QUEUE:
                    goto default;
                case ObjectType.WORKGROUP_GROUP:
                    goto default;
                case ObjectType.ACTIVITY:
                    goto default;
                case ObjectType.POPULARITY:
                    goto default;
                case ObjectType.IMPORT:
                    goto default;
                case ObjectType.I18N_TEXT:
                    goto default;
                case ObjectType.WIDGET:
                    goto default;
                case ObjectType.WIDGET_FRAME:
                    goto default;
                case ObjectType.WIDGET_LAYOUT:
                    goto default;
                case ObjectType.INVITATION:
                    goto default;
                case ObjectType.ENTITLEMENT:
                    goto default;
                case ObjectType.ROSTER:
                    goto default;
                case ObjectType.OFFLINE:
                    goto default;
                case ObjectType.PROFILE_FIELD:
                    goto default;
                case ObjectType.PROJECT:
                    return "project";
                case ObjectType.CHECKPOINT:
                    goto default;
                case ObjectType.TASK:
                    goto default;
                case ObjectType.DUE_DATE:
                    goto default;
                case ObjectType.PROJECT_STATUS:
                    goto default;
                case ObjectType.SOCIAL_GROUP:
                    return "group";
                case ObjectType.SOCIAL_GROUP_MEMBER:
                    goto default;
                case ObjectType.FAVORITE:
                    goto default;
                case ObjectType.EXTERNAL_URL:
                    goto default;
                case ObjectType.LABEL:
                    goto default;
                case ObjectType.BRIDGE:
                    goto default;
                case ObjectType.VIDEO:
                    return "video";
                case ObjectType.VIDEO_WATERMARK:
                    goto default;
                case ObjectType.OSWORKFLOW_ENTRY:
                    goto default;
                case ObjectType.OSWORKFLOW_STEP:
                    goto default;
                case ObjectType.AUDIT_MESSAGE:
                    goto default;
                case ObjectType.REFERENCE:
                    goto default;
                case ObjectType.SYSTEM_CONTAINER:
                    goto default;
                case ObjectType.USER_CONTAINER:
                    goto default;
                case ObjectType.IDEA:
                    return "idea";
                case ObjectType.DIRECTMESSAGE:
                    return "dm";
                case ObjectType.EVENT:
                    return "event";
                case ObjectType.WALLENTRY:
                    return "update";
                default:
                    return type.ToString().ToLower();
            }
        }
    }
    public enum ObjectType
    {
        THREAD = 1,
        MESSAGE,
        USER,
        GROUP,
        THREAD_NAME,
        MESSAGE_SUBJECT,
        MESSAGE_BODY,
        CREATION_DATE,
        MODIFICATION_DATE,
        EXTENDED_PROPERTY,
        ANONYMOUS,
        REGISTERED_USERS,
        ATTACHMENT,
        COMMUNITY,
        COMMUNITY_NAME = 16,
        SYSTEM,
        POLL,
        COMMUNITY_SEARCH_QUERY,
        PRIVATE_MESSAGE,
        PRIVATE_MESSAGE_FOLDER,
        ANNOUNCEMENT,
        SEARCH,
        CRONTASK,
        STATUS_LEVEL,
        AVATAR,
        QUESTION,
        COMMUNITY_QUESTION = 29,
        SYSTEM_QUESTION,
        GATEWAY,
        BAN,
        ABUSE,
        WATCH_SETTINGS = 35,
        DRAFT,
        BLOG,
        BLOGPOST,
        TRACKBACK = 40,
        TAG,
        TAG_SET,
        CONTENT_OBJECT,
        STATUS_LEVEL_POINT,
        STATUS_LEVEL_SCENARIO,
        WATCHABLE_OBJECT,
        SEARCH_ENGINE,
        USER_STATUS,
        USER_RELATIONSHIP,
        USER_RELATIONSHIP_GRAPH,
        PROJECT_QUESTION,
        SOCIAL_GROUP_QUESTION,
        USER_RELATIONSHIP_LIST,
        PLUGIN = 69,
        DOCUMENT = 102,
        DOCUMENT_TITLE,
        DOCUMENT_FIELD,
        COMMENT,
        EXPIRATION_DATE,
        RATING,
        DOCUMENT_TYPE,
        SEARCH_QUERY,
        DOCUMENT_BODY,
        IMAGE,
        DOCUMENT_PRESENTER = 117,
        DOCUMENT_STATE,
        DOCUMENT_FIELD_OPTION,
        DOCUMENT_VERSION,
        DOCUMENT_VERSION_COMMENT,
        DOCUMENT_FIELD_TYPE = 123,
        DOCUMENT_ID,
        DOCUMENT_TYPE_ELEMENT,
        TEMPORARY_DOCUMENT_ID = 127,
        DOCUMENT_BACKCHANNEL = 129,
        READ_STAT_SESSION = 201,
        READ_STAT,
        WORKGROUP_AGENT = 300,
        WORKGROUP_QUEUE,
        WORKGROUP_GROUP,
        ACTIVITY = 310,
        POPULARITY,
        IMPORT = 320,
        I18N_TEXT = 330,
        WIDGET = 340,
        WIDGET_FRAME,
        WIDGET_LAYOUT,
        INVITATION = 350,
        ENTITLEMENT = 360,
        ROSTER = 400,
        OFFLINE,
        PROFILE_FIELD = 500,
        PROJECT = 600,
        CHECKPOINT,
        TASK,
        DUE_DATE,
        PROJECT_STATUS,
        SOCIAL_GROUP = 700,
        SOCIAL_GROUP_MEMBER,
        FAVORITE = 800,
        EXTERNAL_URL,
        LABEL = 900,
        BRIDGE = 1000,
        VIDEO = 1100,
        VIDEO_WATERMARK,
        OSWORKFLOW_ENTRY = 2001,
        OSWORKFLOW_STEP,
        AUDIT_MESSAGE,
        REFERENCE = 2010,
        SYSTEM_CONTAINER = -2,
        USER_CONTAINER = 2020,
        IDEA = 3227383,
        DIRECTMESSAGE = 109016030,
        EVENT = 96891546,
        WALLENTRY = 1464927464
    }
}
