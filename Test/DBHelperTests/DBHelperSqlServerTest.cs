using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Com.DB;
using System.Data;
using System.Collections.Generic;
using Jurassic.Com.Tools;

namespace DBHelperTests
{
    /// <summary>
    /// SqlServer的数据访问测试
    /// </summary>
    [TestClass]
    public class DBHelperSqlServerTest
    {
        [TestMethod]
        public void TestImportFromDataTable()
        {
            DataTable dt = TestObjects.MakeCustomerTable();
            DBHelper helper = new DBHelper("DefaultConnection");

            //先清除表的原数据，这不是必需的
            helper.ExecNonQuery("DELETE FROM Biz_Order;DELETE FROM Biz_Customer");

            //将前面生成的DataTable导入到表中
            helper.Import(dt, null, true, 10, null);

            //count一下验证是否导入成功
            object cnt = helper.ExecGetObject("SELECT COUNT(*) FROM Biz_Customer");

            Assert.AreEqual(dt.Rows.Count, cnt);
        }

        [TestMethod]
        public void TestPageReader()
        {
            TestImportFromDataTable();
            DBPagerInfo pageInfo = new DBPagerInfo { Query = "SELECT * FROM Biz_Customer", KeyId = "ID", PageIndex = 1, PageSize = 2, OrderBy = "ID" };
            DBHelper helper = new DBHelper("DefaultConnection");
            var reader = helper.ExecPageReader(pageInfo);
            int cnt = 0;
            while (reader.Read()) cnt++;
            Assert.AreEqual(cnt, 1);
        }


        [TestMethod]
        public virtual void TestExecDataTable()
        {
            DBHelper helper = new DBHelper("DefaultConnection");

            DataTable dt = helper.ExecDataTable("SELECT * FROM Biz_Customer");
            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public virtual void TestExecNonQuery()
        {
            DBHelper helper = new DBHelper("DefaultConnection");
            int customerId = 0;
            while (true)
            {
                customerId = CommOp.ToInt(helper.ExecGetObject("SELECT TOP 1 Id FROM Biz_Customer"));
                if (customerId > 0) break;
                TestImportFromDataTable();
            }

            int r = helper.ExecNonQuery(@"INSERT INTO [Biz_Order]
           ([Name]
           ,[ProductName]
           ,[Amount]
           ,[DelierDate]
           ,[CustomerId]
           ,[ManagerId])
     VALUES
           (@Name
           ,@ProductName
           ,@Amount
           ,@DelierDate
           ,@CustomerId
           ,@ManagerId
)",
                       helper.CreateParameter("Name", "Computer")
                       , helper.CreateParameter("ProductName", "Dell")
                       , helper.CreateParameter("Amount", 101)
                       , helper.CreateParameter("DelierDate", DateTime.Today)
                       , helper.CreateParameter("CustomerId", customerId)
                       , helper.CreateParameter("ManagerId", 1)
                       );

            Assert.AreEqual(r, 1);
        }

        [TestMethod]
        public virtual void TestExecDataSet()
        {
            DBHelper helper = new DBHelper("DefaultConnection");

            //sqlserver可以连续执行多条查询返回DataSet
            DataSet ds = helper.ExecDataSet("SELECT * FROM Biz_Customer;SELECT * FROM Biz_Order");

            Assert.AreEqual(ds.Tables.Count, 2);
        }


        [TestMethod]
        public void TestGetListT()
        {
            DBHelper helper = new DBHelper("DefaultConnection");
            List<Biz_Customer> listT = helper.GetList<Biz_Customer>("SELECT * FROM Biz_Customer");

            Assert.IsNotNull(listT);
        }

    }
}
