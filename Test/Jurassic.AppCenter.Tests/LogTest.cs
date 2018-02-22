using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.AppCenter.Logs;
using log4net;
using log4net.Core;
using Jurassic.Log4;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class LogTest
    {
        [TestMethod]
        public void TestWirteJLog()
        {

            //private static readonly IEventIDLog log = EventIDLogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            // var type = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            var log = new JLogManager().GetLogger("MyLog");

            log.Write(new JLogInfo
            {
                ActionName = "Action1",
                Browser = "IE",
                BrowserVersion = 10.0M,
                ModuleName = "Module1",
                ObjectId = 100,
                CatalogId = 1,
                Platform = "Windows 8.1",
                UserName = "ggg",
            }, JLogType.Info, null);

            log.Write(new JLogInfo
            {
                ActionName = "Action2",
                Browser = "IE",
                BrowserVersion = 12.0M,
                ModuleName = "Module2",
                ObjectId = 100,
                CatalogId = 1,
                Platform = "Windows 8.1",
                UserName = "123",
            }, JLogType.Info, null);
        }

        [TestMethod]
        public void TestLogHelper()
        {
            LogHelper.Init(new JLogManager(), "MyLog");
            LogHelper.Write(new JLogInfo
            {
                ActionName = "LogHelper",
                Browser = "IE",
                BrowserVersion = 10.0M,
                ModuleName = "Module1",
                ObjectId = 100,
                CatalogId = 1,
                Platform = "Windows 8.1",
                UserName = "456",
            });

            LogHelper.Write(new JLogInfo
            {
                ActionName = "LogHelper",
                Browser = "IE",
                BrowserVersion = 12.0M,
                ModuleName = "Module2",
                ObjectId = 100,
                CatalogId = 1,
                Platform = "Windows 8.1",
                UserName = "abc",
            });
        }
    }
}
