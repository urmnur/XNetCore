using System;
using System.Runtime.Serialization;

namespace XNetCore.DingTalk
{
    [DataContract]
    public class FeedCardMsg : DingTalkMsg
    {
        public FeedCardMsg() : base()
        {
            this.FeedCard = new FeedCardMsgData();
        }
        [DataMember(Name = "msgtype")]
        public string Msgtype { get { return "feedCard"; } }
        [DataMember(Name = "feedCard")]
        public FeedCardMsgData FeedCard { get; set; }
    }
    [DataContract]
    public class FeedCardMsgData
    {
        [DataMember(Name = "links")]
        public FeedCardMsgLinksData[] Links { get; set; }
    }
    [DataContract]
    public class FeedCardMsgLinksData
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "messageUrl")]
        public string MessageUrl { get; set; }
        [DataMember(Name = "picUrl")]
        public string PicUrl { get; set; }
    }
}