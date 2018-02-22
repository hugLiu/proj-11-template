using Jurassic.AppCenter;
using Jurassic.AppCenter.Caches;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Jurassic.Com.Tools;
using System.Web.Caching;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections;
using Jurassic.WebWeChat.Models;
using Jurassic.CommonModels.Messages;
using Jurassic.CommonModels;

namespace Jurassic.WebWeChat
{
    public class ApiHelper
    {
        public static string CreateOAuthUrl(string state)
        {
            string url = "https://open.weixin.qq.com/connect/oauth2/authorize?"
            + string.Format("appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_userinfo&agentid={2}&state={3}#wechat_redirect"
                , CorpId, HttpUtility.UrlEncode(CorpLoginUrl), AgentId, HttpUtility.UrlEncode(state));
            return url;
        }

        const string accessTokenUrl = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";
        const string userInfoUrl = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}";
        const string userDetailUrl = "https://qyapi.weixin.qq.com/cgi-bin/user/getuserdetail?access_token={0}";
        const string messageUrl = "https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
        static System.Timers.Timer _timer = new System.Timers.Timer(600 * 1000);

        public const string ProviderName = "WeChat";

        static ApiHelper()
        {
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        static private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GetAccessToken();
        }

        static void GetAccessToken()
        {
            string url = String.Format(accessTokenUrl, CorpId, Secret);
            string result = GetJson(url);
            //{
            //   "errcode":0，
            //   "errmsg":""，
            //   "access_token": "accesstoken000001",
            //   "expires_in": 7200
            //}
            var dict = JObject.Parse(result);
            _accessToken = (string)dict["access_token"];
        }

        static public UserInfo GetUserInfo(string code)
        {
            string url = String.Format(userInfoUrl, AccessToken, code);
            string result = GetJson(url);
            // {
            //   "errcode": 0,
            //   "errmsg": "ok",
            //   "UserId":"USERID",
            //   "DeviceId":"DEVICEID",
            //   "user_ticket": "USER_TICKET"，
            //   "expires_in":7200
            //}
            var userInfo = JsonHelper.FromJson<UserInfo>(result);
            return userInfo;
        }

        static public UserDetail GetUserDetail(string ticket)
        {
            string url = String.Format(userDetailUrl, AccessToken);
            string result = PostJson(url, new { user_ticket = ticket });
            var userDetail = JsonHelper.FromJson<UserDetail>(result);
            return userDetail;
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public SendResult SendMessage(JMessage msg)
        {
            var oauthDataProivder = SiteManager.Get<IOAuthDataProvider>();
            string url = String.Format(messageUrl, AccessToken);
            var users = oauthDataProivder.GetOAuthUserId(ApiHelper.ProviderName, msg.SendToIds);
            var textCardMsg = new TextCardMessage
            {
                AgentId = AgentId,

                TextCard = new TextCard
                {
                    Title = msg.Title,
                    ButtonText = "查看更多",
                    Description = String.IsNullOrWhiteSpace(msg.Content) ? msg.Title : msg.Content,
                    Url = CreateOAuthUrl(msg.Url),
                },
                ToUser = String.Join("|", users),
            };
            string result = PostJson(url, textCardMsg);
            return new SendResult
            {
                MessageId = msg.Id
            };
        }

        /// <summary>
        /// 向所有用户群发消息 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public SendResult BroadCastMessage(JMessage msg)
        {
            string url = String.Format(messageUrl, AccessToken);

            string result = PostJson(url, new TextCardMessage
            {
                AgentId = AgentId,

                TextCard = new TextCard
                {
                    Title = msg.Title,
                    ButtonText = "查看更多",
                    Description = msg.Content,
                    Url = CreateOAuthUrl(msg.Url),
                },
                ToUser = "@all",
            });
            return new SendResult
            {
                MessageId = msg.Id
            };
        }
        private static string _accessToken;
        static public string AccessToken
        {
            get
            {
                if (_accessToken == null)
                {
                    GetAccessToken();
                }
                return _accessToken;
            }
        }

        static public string CorpId
        {
            get
            {
                return ConfigurationManager.AppSettings["CorpId"];
            }
        }

        static public string Secret
        {
            get
            {
                return ConfigurationManager.AppSettings["Secret"];
            }
        }

        static public string AgentId
        {
            get
            {
                return ConfigurationManager.AppSettings["AgentId"];
            }
        }

        /// <summary>
        /// 网站进行微信身份验证的入口地址，在进入CorpLogin时自动赋值
        /// </summary>
        public static string CorpLoginUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["CorpLoginUrl"];
            }
        }

        public static string GetJson(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //设定要响应的数据格式  
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                // 为JSON格式添加一个Accept报头
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = httpClient.GetAsync(new Uri(url)).Result;

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //获取响应结果
                    string res = result.Content.ReadAsStringAsync().Result;
                    return res;
                }
                else
                {
                    throw new JException(result.StatusCode.ToString());
                }
            }

        }
        public static string PostJson(string url, object req)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                //设定要响应的数据格式  
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                // 为JSON格式添加一个Accept报头
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string strReq = JsonConvert.SerializeObject(req, Formatting.Indented, jSetting);
                HttpContent content = new StringContent(strReq);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = httpClient.PostAsync(new Uri(url), content).Result;

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //获取响应结果
                    string res = result.Content.ReadAsStringAsync().Result;

                    return res;
                }
                else
                {
                    throw new JException(result.StatusCode.ToString());
                }
            }

        }
    }

}