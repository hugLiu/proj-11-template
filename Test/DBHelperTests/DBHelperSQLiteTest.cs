using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Com.DB;
using System.Data;
using System.Collections.Generic;
using Jurassic.Com.Tools;
using Jurassic.Com.DB.SQLite;

namespace DBHelperTests
{
    //SQLite的数据访问测试
    [TestClass]
    public class DBHelperSQLiteTest
    {
        DBHelper helper;
        public DBHelperSQLiteTest()
        {
            // 以下的代码是显式创建对象，

            // 可以使用IOC框架在配置中动态创建
            helper = new DBHelper("SqliteConnection");

            // 由于SQLite需要引用第三方类库，所以它的接口实现以单独类库的形式提供
            helper.DBComm = new DBCommSQLite();
        }

        [TestMethod]
        public void SQLiteTestImportFromDataTable()
        {
            DataTable dt = TestObjects.MakeCustomerTable();

            //先清除表的原数据，这不是必需的
            helper.ExecNonQuery("DELETE FROM Biz_Order;DELETE FROM Biz_Customer");

            //将前面生成的DataTable导入到表中
            helper.Import(dt, null, true, 10, null);

            //count一下验证是否导入成功
            object cnt = helper.ExecGetObject("SELECT COUNT(*) FROM Biz_Customer");

            Assert.AreEqual(dt.Rows.Count, CommOp.ToInt(cnt));
        }

        [TestMethod]
        public void SQLiteTestPageReader()
        {
            SQLiteTestImportFromDataTable();
            DBPagerInfo pageInfo = new DBPagerInfo { Query="SELECT * FROM Biz_Customer", PageIndex = 1, PageSize = 2, OrderBy = "ID" };
            var reader = helper.ExecPageReader(pageInfo);
            int cnt =0;
            while (reader.Read()) cnt++;
            Assert.AreEqual(cnt, 1);
        }

        [TestMethod]
        public virtual void SQLiteTestExecDataTable()
        {
            DataTable dt = helper.ExecDataTable("SELECT * FROM Biz_Customer");
            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public virtual void SQLiteTestExecNonQuery()
        {
            int customerId = 0;
            while (true)
            {
                customerId = CommOp.ToInt(helper.ExecGetObject("SELECT Id FROM Biz_Customer LIMIT 1"));
                if (customerId > 0) break;
                SQLiteTestImportFromDataTable();
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
        public virtual void SQLiteTestTransNonQuery()
        {
            int customerId = 0;
            while (true)
            {
                customerId = CommOp.ToInt(helper.ExecGetObject("SELECT Id FROM Biz_Customer LIMIT 1"));
                if (customerId > 0) break;
                SQLiteTestImportFromDataTable();
            }

            helper.BeginTrans();
            int r = helper.TransNonQuery(@"INSERT INTO [Biz_Order]
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
            helper.EndTrans();
            Assert.AreEqual(r, 1);
        }

        [TestMethod]
        public virtual void SQLiteTestExecDataSet()
        {
            //sqlite可以连续执行多条查询返回DataSet
            DataSet ds = helper.ExecDataSet("SELECT * FROM Biz_Customer;SELECT * FROM Biz_Order");

            Assert.AreEqual(ds.Tables.Count, 2);
        }


        [TestMethod]
        public void SQLiteTestGetListT()
        {
            List<Biz_Customer> listT = helper.GetList<Biz_Customer>("SELECT * FROM Biz_Customer");

            Assert.IsNotNull(listT);
        }

    }
}
