using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Jurassic.WebWeChat.Models
{
    public class UserInfo
    {
        //   "errcode": 0,

        [JsonProperty("errcode")]
        public string ErrCode { get; set; }
        //   "errmsg": "ok",

        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
     
        //   "UserId":"USERID",
        public string UserId { get; set; }
 
        public string OpenId { get; set; }
        //   "DeviceId":"DEVICEID",
        public string DeviceId { get; set; }

        //   "user_ticket": "USER_TICKET"，
        [JsonProperty("user_ticket")]
        public string UserTicket { get; set; }


        //   "expires_in":7200
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class UserDetail
    {
        //"userid":"lisi",
        [JsonProperty("userid")]
        public string UserId { get; set; }

        //"name":"李四",
        [JsonProperty("name")]
        public string Name { get; set; }

        //"department":[3],
        [JsonProperty("department")]
        public string Department { get; set; }

        //"position": "后台工程师",
        [JsonProperty("position")]
        public string Position { get; set; }

        //"mobile":"15050495892",
        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        //"gender":1,
        [JsonProperty("gender")]
        public int Gender { get; set; }

        //"email":"xxx@xx.com",
        [JsonProperty("email")]
        public string Email { get; set; }

        //"avatar":"http://shp.qpic.cn/bizmp/xxxxxxxxxxx/0"
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }
}