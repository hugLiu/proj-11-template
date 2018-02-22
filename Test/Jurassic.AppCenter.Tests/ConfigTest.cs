using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.AppCenter.Config;
using System.Windows.Forms;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void UIContextSaveTest()
        {
            TestClass tc = new TestClass
            {
                Id = 1,
                Name = "Good",
                Remark = "Hello"
            };

            UIContext context = new UIContext(tc);
            context.LoadContext("Name=GG;Remark=HH");
            Assert.AreEqual(tc.Name, "GG");
            Assert.AreEqual(tc.Remark, "HH");
            context.Save();

            TestStringId tsi = new TestStringId
            {
                MyList = { "GGG", "HHH", "IIII" },
                SubObject = new TestClass { Name = "ThisSub" },

                MyPos = new System.Drawing.Point(1, 2),
                MySize = new System.Drawing.Size(200, 300)
            };

            UIContext context1 = new UIContext(tsi);
            context1.LoadContext("MyList;SubObject.Name;MySize;MyPos");
            tsi.SubObject.Name = "ThatSub";
            context1.Save(true);
        }

        [TestMethod]
        public void UIContextLoadTest()
        {
            TestClass tc = new TestClass();

            UIContext context = new UIContext(tc);
            context.LoadContext("Name;Remark");

            Assert.AreEqual(tc.Name, "GG");

            TestStringId tsi = new TestStringId();
            context = new UIContext(tsi);
            context.LoadContext("MyList;SubObject.Name;MySize;MyPos");

            Assert.AreEqual(tsi.MyList[0], "GGG");
            Assert.AreEqual(tsi.SubObject.Name, "ThatSub");
            // Assert.AreEqual(tsi.MySize.Height, 300);
            Assert.AreEqual(tsi.MyPos, new System.Drawing.Point(1, 2));
        }

    }
}
