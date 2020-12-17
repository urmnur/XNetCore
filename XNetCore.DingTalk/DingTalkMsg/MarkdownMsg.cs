using System;
using System.Runtime.Serialization;

namespace XNetCore.DingTalk
{
    [DataContract]
    public class MarkdownMsg : DingTalkMsg
    {
        public MarkdownMsg() : base()
        {
            this.Markdown = new MarkdownMsgData();
        }
        [DataMember(Name = "msgtype")]
        public string Msgtype { get { return "markdown"; } }

        [DataMember(Name = "markdown")]
        public MarkdownMsgData Markdown { get; set; }
    }
    [DataContract]
    public class MarkdownMsgData
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }
    }
}
