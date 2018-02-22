using Jurassic.AppCenter;
using Jurassic.AppCenter.Models;
using Jurassic.Com.Tools;
using Jurassic.WebFrame;
using Jurassic.WebWeChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Jurassic.WebWeChat.Controllers
{
    /// <summary>
    /// 识别微信企业号身份的登录器
    /// </summary>
    public class CorpLoginController : BaseController
    {
        IStateUrlService _stateUrlService;
        IOAuthDataProvider _oauthDataProvider;
        const string WeChatUserInfoKey = "WeChatUserInfo";
        public CorpLoginController(IStateUrlService stateUrlService, IOAuthDataProvider oauthDataProvier)
        {
            _stateUrlService = stateUrlService;
            _oauthDataProvider = oauthDataProvier;
        }

        // GET: CorpLogin
        [HttpGet]
        public ActionResult Index(string code, string state)
        {
            if (ApiHelper.AccessToken == null)
            {
                if (ApiHelper.AccessToken == null)
                {
                    throw new JException("AccessToken无法获取");
                }
            }
            UserInfo userInfo = Session[WeChatUserInfoKey] as UserInfo;
            if (userInfo == null)
            {
                userInfo = ApiHelper.GetUserInfo(code);
                Session[WeChatUserInfoKey] = userInfo;
                //var userdetail = ApiHelper.GetUserDetail(userInfo.UserTicket);
            }
            if (userInfo.UserId.IsEmpty())
            {
                Session.Remove(WeChatUserInfoKey);
                return null;
                //throw new JException("微信用户接口失败：" + userInfo.ErrorMessage);
            }
            var user = _oauthDataProvider.GetLocalUser(ApiHelper.ProviderName, userInfo.UserId);
            if (!User.Identity.IsAuthenticated)
            {
                if (user == null)
                {
                    //找不到本地用户则进入账号密码验证页面
                    return View();
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(user.Name, true);
                }
            }
            else
            {
                if (user == null)
                {
                    _oauthDataProvider.SaveOAuthUser(ApiHelper.ProviderName, userInfo.UserId, CurrentUserId.ToInt());
                }
                else if (user.Name != User.Identity.Name)
                {
                    throw new Exception("用户身份不符");
                }

            }
         //   Session.Remove(WeChatUserInfoKey);
            
            if (state.IsEmpty())
            {
                return Redirect(Url.Content("~/"));
            }
            else
            {
                string startPage = Url.Content(_stateUrlService.GetUrl(state));
                return Redirect(Url.Content("~/?startPage=" + Url.Encode(startPage)));
            }
        }

        /// <summary>
        /// 当微信端打开的页面找不到本地用户时，让用户输入账号密码确认身份
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            if (AppManager.Instance.Login(model) == LoginState.OK)
            {
                return JsonTips("success", "身份验证成功", new { Url = Request.Url.AbsoluteUri });
            }
            else
            {
                return JsonTips("error", "身份验证失败，用户名密码不符");
            }
        }

    }
}