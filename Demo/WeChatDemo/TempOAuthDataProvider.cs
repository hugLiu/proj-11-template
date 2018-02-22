using Jurassic.AppCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChatDemo
{
    /// <summary>
    /// 这个是为了避免在微信中出现用户验证界面，让用户能直接进内容页
    /// 在实际要求用户认证场合应该用系统自带的
    /// </summary>
    public class TempOAuthDataProvider : IOAuthDataProvider
    {
        public AppUser GetLocalUser(string oauthProvider, string oauthUserId)
        {
            return AppManager.Instance.UserManager.GetAll().First();
        }

        public IList<string> GetOAuthUserId(string oauthProvider, IEnumerable<int> localUsersId)
        {
            return AppManager.Instance.UserManager.GetAll().Select(u => u.Id).ToList();
        }

        public void SaveOAuthUser(string oauthProvider, string userId, int currentUserId)
        {
            
        }
    }
}