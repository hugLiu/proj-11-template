using Jurassic.AppCenter;
using Jurassic.CommonModels;
using Jurassic.WebFrame;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace MvcApplication1.Tests
{
    [TestClass]
    public class BaseTest
    {
        public BaseTest()
        {
            //SiteManager.Init();
            new ApplicationTest().Start();
        }


    }

    class ApplicationTest : MvcApplication
    {
        public void Start()
        {
            AddBindings(SiteManager.Kernel);
            AppManager.Instance.UserProvider = SiteManager.Kernel.Get<IDataProvider<AppUser>>();
            AppManager.Instance.RoleProvider = SiteManager.Kernel.Get<IDataProvider<AppRole>>();
            AppManager.Instance.StateProvider = SiteManager.Kernel.Get<IStateProvider>();
            SiteManager.Init();
        }
    }
}
