using System;
using System.Runtime.Serialization;

namespace XNetCore.DingTalk
{
    [DataContract]
    public class LinkMsg : DingTalkMsg
    {
        public LinkMsg() : base()
        {
            this.Link = new LinkMsgData();
        }
        [DataMember(Name = "msgtype")]
        public string Msgtype { get { return "link"; } }
        [DataMember(Name = "link")]
        public LinkMsgData Link { get; set; }
    }
    [DataContract]
    public class LinkMsgData
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "messageUrl")]
        public string MessageUrl { get; set; }
        [DataMember(Name = "picUrl")]
        public string PicUrl { get; set; }
    }
}
