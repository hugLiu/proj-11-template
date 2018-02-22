using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.AppCenter
{
    /// <summary>
    /// 第三方用户和本地用户的关系数据提供程序接口
    /// </summary>
    public interface IOAuthDataProvider
    {
        /// <summary>
        /// 根据第三方用户ID获取本地用户
        /// </summary>
        /// <param name="oauthProvider"></param>
        /// <param name="oauthUserId"></param>
        /// <returns></returns>
        AppUser GetLocalUser(string oauthProvider, string oauthUserId);

        /// <summary>
        /// 保存本地用户和第三方用户的对应关系
        /// </summary>
        /// <param name="oauthProvider"></param>
        /// <param name="userId"></param>
        /// <param name="currentUserId"></param>
        void SaveOAuthUser(string oauthProvider, string userId, int currentUserId);

        /// <summary>
        /// 获取本地用户对应的第三方用户ID
        /// </summary>
        /// <param name="oauthProvider"></param>
        /// <param name="localUsersId"></param>
        /// <returns></returns>
        IList<string> GetOAuthUserId(string oauthProvider, IEnumerable<int> localUsersId);
    }
}
