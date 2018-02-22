using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Com.DB;
using System.Data;
using System.Collections.Generic;
using Jurassic.Com.Tools;

namespace DBHelperTests
{
    /// <summary>
    /// Access的数据访问测试
    /// </summary>
    [TestClass]
    public class DBHelperAccessTest
    {
        [TestMethod]
        public void AccessTestImportFromDataTable()
        {
            DataTable dt = TestObjects.MakeCustomerTable();
            DBHelper helper = new DBHelper("AccessConnection");

            //先清除表的原数据，这不是必需的
            helper.ExecNonQuery("DELETE FROM Biz_Order");
            helper.ExecNonQuery("DELETE FROM Biz_Customer");

            //将前面生成的DataTable导入到表中
            helper.Import(dt, null, true, 10, null);

            //count一下验证是否导入成功
            object cnt = helper.ExecGetObject("SELECT COUNT(*) FROM Biz_Customer");

            Assert.AreEqual(dt.Rows.Count, cnt);
        }

        [TestMethod]
        public virtual void AccessTestExecDataTable()
        {
            DBHelper helper = new DBHelper("AccessConnection");

            DataTable dt = helper.ExecDataTable("SELECT * FROM Biz_Customer");
            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public virtual void AccessTestExecNonQuery()
        {
            DBHelper helper = new DBHelper("AccessConnection");
            int customerId = 0;
            while (true)
            {
                customerId = CommOp.ToInt(helper.ExecGetObject("SELECT TOP 1 Id FROM Biz_Customer"));
                if (customerId > 0) break;
                AccessTestImportFromDataTable();
            }

            //注意Access也可以用?号代替@参数名。access的参数顺序不能乱而且不能重复使用
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
                       helper.CreateParameter("Name", "Computer" + DateTime.Now.Second)
                       , helper.CreateParameter("ProductName", "Dell" +DateTime.Now.Millisecond)
                       , helper.CreateParameter("Amount", 101)
                       , helper.CreateParameter("DelierDate", DateTime.Today)
                       , helper.CreateParameter("CustomerId", customerId)
                       , helper.CreateParameter("ManagerId", 1)
                       );

            Assert.AreEqual(r, 1);
        }

        [TestMethod]
        public virtual void AccessTestExecDataSet()
        {
            DBHelper helper = new DBHelper("AccessConnection");

            //access不支持连续执行多条查询返回DataSet
            DataSet ds = helper.ExecDataSet("SELECT * FROM Biz_Customer");
            DataTable dt = helper.ExecDataTable("SELECT * FROM Biz_Order");
            ds.Tables.Add(dt);

            Assert.AreEqual(ds.Tables.Count, 2);
        }


        [TestMethod]
        public void AccessTestGetListT()
        {
            DBHelper helper = new DBHelper("AccessConnection");
            List<Biz_Customer> listT = helper.GetList<Biz_Customer>("SELECT * FROM Biz_Customer");

            Assert.IsNotNull(listT);
        }

    }
}
