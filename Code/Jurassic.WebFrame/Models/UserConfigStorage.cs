using Jurassic.AppCenter;
using Jurassic.AppCenter.Caches;
using Jurassic.CommonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic.Com.Tools;

namespace Jurassic.WebFrame.Models
{
    /// <summary>
    /// 用于存储与用户关联的配置信息
    /// </summary>
    /// <typeparam name="T">继承UserConfig的子类</typeparam>
    public class UserConfigStorage<T> where T : UserConfig
    {
        CachedList<T> usersList = new CachedList<T>();

        public UserConfigStorage()
        {
            AppManager.Instance.UserManager.AfterGetData = AfterGetData;

            AppManager.Instance.UserManager.AfterSaved = user =>
            {
                var oldUser = usersList.FirstOrDefault(u => u.Id == user.Id);
                if (oldUser != null)
                {
                    usersList.Remove(oldUser);
                }
                usersList.Add((T)user);
                usersList.Save();
            };

            usersList.OnMerge += usersList_OnMerge;
        }

        void usersList_OnMerge(object sender, CachedObjectOnMergeEventArgs<List<T>> e)
        {
            foreach (var newUser in e.NewObject)
            {
                var oldUser =AppManager.Instance.UserManager.GetById(newUser.Id);
                if (oldUser != null)
                {
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        if (prop.CanWrite)
                        {
                            prop.SetValue(oldUser, prop.GetValue(newUser, null), null);
                        }
                    }
                }
            }
        }

        private IEnumerable<T> AfterGetData(IEnumerable<AppUser> userList)
        {
            foreach (var user in userList)
            {
                var cacheUser = usersList.FirstOrDefault(u => u.Id == user.Id) ?? SiteManager.Get<T>();
                if (CommOp.ToInt(cacheUser.Id) == 0)
                {
                    cacheUser.IsDefaultRole = true;
                }
                cacheUser.Id = user.Id;
                cacheUser.Name = user.Name;
                cacheUser.Email = user.Email;
                cacheUser.PhoneNumber = user.PhoneNumber;
                cacheUser.RoleIds = user.RoleIds;
                cacheUser.TrueName = user.TrueName;
                yield return cacheUser;
            }
        }
    }
}
