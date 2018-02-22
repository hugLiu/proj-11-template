using Jurassic.AppCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jurassic.CommonModels.EFProvider
{
    public class OAuthDataProvider : IOAuthDataProvider
    {
        public AppUser GetLocalUser(string oauthProvider, string oauthUserId)
        {
            using (var context = SiteManager.Get<ModelContext>())
            {
                var member = context.OAuthMemberShips.FirstOrDefault(m => m.Provider == oauthProvider
                 && m.ProviderUserId == oauthUserId);
                if (member == null)
                {
                    return null;
                }
                var user = AppManager.Instance.UserManager.GetById(member.UserId.ToString());
                return user;
            }
        }

        public void SaveOAuthUser(string oauthProvider, string oauthUserId, int localUserId)
        {
            using (var context = SiteManager.Get<ModelContext>())
            {
                var member = context.OAuthMemberShips.FirstOrDefault(m => m.Provider == oauthProvider
                 && m.ProviderUserId == oauthUserId);
                if (member != null) return;

                member = new OAuthMemberShip
                {
                    UserId = localUserId,
                    ProviderUserId = oauthUserId,
                    Provider = oauthProvider
                };
                context.OAuthMemberShips.Add(member);
                context.SaveChanges();
            }
        }

        public IList<string> GetOAuthUserId(string oauthProvider, IEnumerable<int> localUsersId)
        {
            using (var context = SiteManager.Get<ModelContext>())
            {
                var users = context.OAuthMemberShips.Where(m => localUsersId.Contains(m.UserId) && m.Provider == oauthProvider)
                    .Select(m=>m.ProviderUserId).ToList();
                return users;
            }
        }
    }
}
