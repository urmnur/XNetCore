using System;
using System.Runtime.Serialization;

namespace XNetCore.DingTalk
{
    [DataContract]
    public class ActionCardMsg : DingTalkMsg
    {
        public ActionCardMsg() : base()
        {
            this.ActionCard = new ActionCardMsgData();
        }
        [DataMember(Name = "msgtype")]
        public string Msgtype { get { return "actionCard"; } }
        [DataMember(Name = "actionCard")]
        public ActionCardMsgData ActionCard { get; set; }
    }
    [DataContract]
    public class ActionCardMsgData
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "text")]
        public string Text { get; set; }
        [DataMember(Name = "singleTitle")]
        public string SingleTitle { get; set; }
        [DataMember(Name = "singleURL")]
        public string SingleURL { get; set; }
        [DataMember(Name = "btnOrientation")]
        public string BtnOrientation { get; set; }
        [DataMember(Name = "btns")]
        public ActionCardBtn[] Btns { get; set; }
    }
    [DataContract]
    public class ActionCardBtn
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "actionURL")]
        public string ActionURL { get; set; }
    }
}
