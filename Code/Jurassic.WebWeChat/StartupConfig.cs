using Jurassic.WebFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Jurassic.CommonModels;
using Ninject;
namespace Jurassic.WebWeChat
{
    public class StartupConfig : IStartupConfig
    {
        public void Config(IAppBuilder app)
        {
            if (ApiHelper.AccessToken == null)
            {
                //强制获取一下accesstoken
            }
            SiteManager.Kernel.Bind<IStateUrlService>().To<DefaultStateUrlService>().InSingletonScope();
            SiteManager.Message.Register(CommonModels.Messages.SendChannel.Custom, new WeChatMessageSender());
        }
    }
}