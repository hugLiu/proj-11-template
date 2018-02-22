using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.AppCenter;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Jurassic.AppCenter.Caches;
using System.Threading;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class DataManagerTest
    {
        [TestMethod]
        public void TestDataManagerBase()
        {
            DataManagerBase<TestClass, int> tstMgr = new DataManagerBase<TestClass, int>();
            tstMgr.Provider = new LocalDataProvider<TestClass>();
            tstMgr.Clear();
            tstMgr.Add(new TestClass { Id = 111, Name = "Myname1", Remark = "My remark1" });
            tstMgr.Add(new TestClass { Id = 121, Name = "Myname2", Remark = "My remark2" });
            tstMgr.Add(new TestClass { Id = 21, Name = "Myname3", Remark = "My remark3" });

            Assert.AreEqual(tstMgr.GetAll().Count, 3);

            tstMgr.Save();
        }

        [TestMethod]
        public void TestCachedList()
        {
            CachedList<TestClass> tstList = new CachedList<TestClass>();
            if (tstList.Count != 3)
            {
                TestDataManagerBase();
                // 休眠0.1秒，是为了使得在TestDataManagerBase()中持久化
                // 的对象能有时间同步到tstList中。
                Thread.Sleep(100);
            }

            Assert.AreEqual(tstList.Count, 3);
        }

        [TestMethod]
        public void TestDataManager()
        {
            DataManager<TestStringId> dm = new DataManager<TestStringId>();
            dm.Provider = new LocalDataProvider<TestStringId>();

            dm.Clear();
            var testIdObj = new TestStringId { Id = "hello", Name = "world", MyList = { "abc", "def" } };
            dm.Add(testIdObj);
            dm.Add(new TestStringId { Id = "hello1", Name = "world1", MyList = { "abcd", "defg" } });
            dm.Add(new TestStringId { Id = "hello2", Name = "world2", MyList = { "1abc", "2def" } });

            Assert.AreEqual(dm.GetAll().Count(), 3);

            var obj = dm.GetById("hello1");

            Assert.AreEqual(obj.Name, "world1");
        }
    }

    class TestClass : IIdNameBase<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }
    }

    class TestStringId : IIdName
    {
        public TestStringId()
        {
            MyList = new List<string>();
            SubObject = new TestClass();
        }

        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public Point MyPos { get; set; }

        public Size MySize { get; set; }

        public TestClass SubObject { get; set; }

        public List<string> MyList { get; set; }
    }
}
