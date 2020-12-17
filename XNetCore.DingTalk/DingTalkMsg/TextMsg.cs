using System;
using System.Runtime.Serialization;

namespace XNetCore.DingTalk
{
    [DataContract]
    public class TextMsg : DingTalkMsg
    {
        public TextMsg() : base()
        {
            this.Text = new TextMsgData();
        }
        [DataMember(Name = "msgtype")]
        public string Msgtype { get { return "text"; } }

        [DataMember(Name = "text")]
        public TextMsgData Text { get; set; }
    }
    [DataContract]
    public class TextMsgData
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}
