using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jurassic.WebWeChat.Models
{
    /// <summary>
    /// 文本卡片消息
    /// </summary>
    public class TextCardMessage
    {
        //{
        //"touser" : "UserID1|UserID2|UserID3",
        //"toparty" : "PartyID1 | PartyID2",
        //"totag" : "TagID1 | TagID2",
        //"msgtype" : "textcard",
        //"agentid" : 1,
        //"textcard" : {
        //         "title" : "领奖通知",
        //         "description" : "<div class=\"gray\">2016年9月26日</div> <div class=\"normal\">恭喜你抽中iPhone 7一台，领奖码：xxxx</div><div class=\"highlight\">请于2016年10月10日前联系行政同事领取</div>",
        //         "url" : "URL",
        //         "btntxt":"更多"
        //}

        [JsonProperty("touser")]
        public string ToUser { get; set; }

        [JsonProperty("msgtype")]
        public string MessageType { get; set; } = "textcard";

        [JsonProperty("agentid")]
        public string AgentId { get; set; }

        [JsonProperty("textcard")]
        public TextCard TextCard { get; set; }
    }
}
