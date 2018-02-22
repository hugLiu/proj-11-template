using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jurassic.AppCenter;
using Jurassic.WebFrame;
using Jurassic.AppCenter.Config;
using Jurassic.WebFrame.Models;
using Ninject;
using Jurassic.Com.Tools;
using System.IO;
using System.Text.RegularExpressions;
using Jurassic.AppCenter.Caches;
using Jurassic.CommonModels;
using Jurassic.CommonModels.Organization;
using System.Data.Entity;
using Jurassic.AppCenter.Resources;

namespace Jurassic.WebFrame.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// 框架主页
        /// </summary>。
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            //  ConfigData.ShowTab = true;
            return View(UserConfig);
        }

        #region 用户配置页
        public virtual ActionResult SelfServiceIndex()
        {
            string themePath = Server.MapPath("~/content/theme");
            ViewData["ThemeList"] = Directory.GetDirectories(themePath)
                .Select(dir => new DirectoryInfo(dir).Name);
            ViewBag.ShowSearchBox = false;
            return View(UserConfig);
        }

        [HttpPost]
        public virtual ActionResult SelfServiceIndex(UserConfig user)
        {
            if (!ModelState.IsValid) return JsonTips(user);

            var tUser = AppManager.Instance.UserManager.GetById(user.Id);

            //添加其他程序AppUser子类中的特殊属性
            Request.Form.AssignFormValues(tUser);

            AppManager.Instance.UserManager.Change(tUser);
            return JsonTips("success", FStr.UpdateUserInfoSucceed, FStr.UpdateUserInfoSucceed0, tUser, tUser.Name);
        }
        #endregion

        #region 框架起始页
        /// <summary>
        /// 主页中内框架mainframe中的起始页
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult StartPage()
        {
            string userid = User.Identity.GetUserId();
            if (string.IsNullOrEmpty(userid))
            {
                return RedirectAction("Login", "Account");
            }
            SyncUsePortalData();

            return View(UserConfig.StartPageConfig);
        }

        /// <summary>
        /// 保存用户的起始页布局
        /// </summary>
        /// <param name="layoutIds"></param>
        /// <returns></returns>
        public ActionResult SaveStartPage(string layoutIds)
        {
            string[] ids = layoutIds.Split(',');

            foreach (WidgetModel widget in UserConfig.StartPageConfig.Widgets)
            {
                int idx = Array.IndexOf(ids, widget.Id);
                widget.Visible = idx >= 0;
                widget.Order = idx;
            }
            AppManager.Instance.UserManager.Change(UserConfig);
            return JsonTips("success", JStr.SuccessSaved);
        }

        /// <summary>
        /// 将用户的起始页布局重置为默认布局
        /// </summary>
        /// <returns></returns>
        public ActionResult ResumeStartPage()
        {
            UserConfig.StartPageConfig = null;
            AppManager.Instance.UserManager.Change(UserConfig);
            return JsonTips(new { Url = Url.Action("StartPage") });
        }

        private void SyncUsePortalData()
        {
            var widgetFuncs = AppManager.Instance.GetUserWidgetFunctions(CurrentUser.Name);
            if (UserConfig.StartPageConfig == null)
            {
                UserConfig.StartPageConfig = new UserPortalModel();
            }
            //将功能列表中的全局配置项同步到用户的部件配置中
            foreach (var func in widgetFuncs)
            {
                var userWidget = UserConfig.StartPageConfig.Widgets.Where(w => w.Id == func.Id).FirstOrDefault();

                if (userWidget == null)
                {
                    userWidget = SiteManager.Get<WidgetModel>();
                    UserConfig.StartPageConfig.Widgets.Add(userWidget);
                    userWidget.Order = func.Ord;
                }
                ConvertToWidget(func, userWidget);
            }

            //同步移除在功能列表中已经被移除的项
            foreach (var widget in UserConfig.StartPageConfig.Widgets.ToArray())
            {
                if (AppManager.Instance.FunctionManager.GetById(widget.Id) == null)
                {
                    UserConfig.StartPageConfig.Widgets.Remove(widget);
                }
            }
        }

        private WidgetModel ConvertToWidget(AppFunction func, WidgetModel userWidget)
        {
            userWidget.Id = func.Id;
            userWidget.Title = ResHelper.GetStr(func.Name);
            //将参数赋值给其他可能的属性
            foreach (var p in func.Arguments)
            {
                RefHelper.SetValue(userWidget, p.Key, p.Value);
            }
            return userWidget;
        }
        #endregion

        #region 以下几个Action是DEMO， 为起始页定制提供一些有意义的功能

        /// <summary>
        /// 提供系统信息的子页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SystemInfo()
        {

            return View();
        }

        /// <summary>
        /// 提供服务器信息的子页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ServerInfo()
        {

            return View();
        }

        /// <summary>
        /// 显示欢迎信息的子页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Welcome()
        {
            return View();
        }

        /// <summary>
        /// 显示数据库信息的子页面
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult DbInfo()
        {
            DbContext context = SiteManager.Get<DbContext>();
            if (context.Database.Exists())
            {
                try
                {
                    context.Database.Connection.Open();
                    string version = context.Database.Connection.ServerVersion;
                    ViewBag.ServerVersion = version;

                    var dataTable = context.Database.Connection.GetSchema();
                    context.Dispose();
                    return View(dataTable);
                }
                catch
                {
                    ViewBag.ServerVersion = "未检测到数据库";
                }
            }
            return View();
        }
        #endregion
    }
}
