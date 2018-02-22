using Jurassic.AppCenter;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WeChatDemo
{
    public class MvcApplication : Jurassic.WebFrame.MvcApplication
    {
        protected override void AddBindings(IKernel kernel)
        {
            base.AddBindings(kernel);

            kernel.Rebind<IOAuthDataProvider>().To<TempOAuthDataProvider>();
        }
    }
}
