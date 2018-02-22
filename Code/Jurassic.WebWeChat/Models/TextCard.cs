using Newtonsoft.Json;

namespace Jurassic.WebWeChat.Models
{
    public class TextCard
    {
        //"textcard" : {
        //         "title" : "领奖通知",
        //         "description" : "<div class=\"gray\">2016年9月26日</div> <div class=\"normal\">恭喜你抽中iPhone 7一台，领奖码：xxxx</div><div class=\"highlight\">请于2016年10月10日前联系行政同事领取</div>",
        //         "url" : "URL",
        //         "btntxt":"更多"
        //}

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("btntxt")]
        public string ButtonText { get; set; }

    }
}