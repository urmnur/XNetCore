using System;
using System.Runtime.Serialization;

namespace XNetCore.DingTalk
{
    [DataContract]
    public class DingTalkMsg
    {
        public DingTalkMsg()
        {
            this.At = new DingTalkAt();
        }
        [DataMember(Name = "at")]
        public DingTalkAt At { get; set; }
    }
    [DataContract]
    public class DingTalkAt
    {
        [DataMember(Name = "atMobiles")]
        public string[] AtMobiles { get; set; }
        [DataMember(Name = "isAtAl")]
        public bool IsAtAll { get; set; }
    }
}
