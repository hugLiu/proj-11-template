using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Com.DB;
using System.Data;
using System.Collections.Generic;
using Jurassic.Com.Tools;
using Jurassic.Com.DB.SQLite;
using Jurassic.Com.DB.OracleMDA;

namespace DBHelperTests
{
    //使用Oralce ManagedDataAccess的数据访问测试
    [TestClass]
    public class DBHelperOralceMDATest
    {
        DBHelper helper;
        public DBHelperOralceMDATest()
        {
            // 以下的代码是显式创建对象，

            // 可以使用IOC框架在配置中动态创建
            helper = new DBHelper("OracleMDAConnection");

            // 由于Oracle需要引用第三方Oracle类库ODP.net，所以它的接口实现以单独类库的形式提供
            helper.DBComm = new DBCommOracleMDA();

        }

        [TestMethod]
        public void OracelMDATestImportFromDataTable()
        {
            DataTable dt = TestObjects.MakeCustomerTableUpper();

            //先清除表的原数据，这不是必需的
            helper.ExecNonQuery("DELETE FROM BIZ_ORDER");
            helper.ExecNonQuery("DELETE FROM BIZ_CUSTOMER");

            //将前面生成的DataTable导入到表中
            helper.Import(dt, null, true, 10, null);

            //count一下验证是否导入成功
            object cnt = helper.ExecGetObject("SELECT COUNT(*) FROM BIZ_CUSTOMER");

            Assert.AreEqual(dt.Rows.Count, CommOp.ToInt(cnt));
        }

        [TestMethod]
        public void OracleTestPageReader()
        {
            OracelMDATestImportFromDataTable();
            DBPagerInfo pageInfo = new DBPagerInfo { Query = "SELECT * FROM BIZ_CUSTOMER", PageIndex = 1, PageSize = 2, OrderBy = "ID" };
            var reader = helper.ExecPageReader(pageInfo);
            int cnt = 0;
            while (reader.Read()) cnt++;
            Assert.AreEqual(cnt, 1);
        }


        [TestMethod]
        public virtual void OracelMDATestExecDataTable()
        {
            DataTable dt = helper.ExecDataTable("SELECT * FROM BIZ_CUSTOMER");
            Assert.IsNotNull(dt);
        }

        [TestMethod]
        public virtual void OracelMDATestExecNonQuery()
        {
            int customerId = 0;
            while (true)
            {
                customerId = CommOp.ToInt(helper.ExecGetObject("SELECT ID FROM BIZ_CUSTOMER"));
                if (customerId > 0) break;
                OracelMDATestImportFromDataTable();
            }

            int r = helper.ExecNonQuery(@"INSERT INTO BIZ_ORDER
           (NAME
           ,PRODUCTNAME
           ,AMOUNT
           ,DELIERDATE
           ,CUSTOMERID
           ,MANANGERID)
     VALUES
           (@NAME
           ,@PRODUCTNAME
           ,@AMOUNT
           ,@DELIERDATE
           ,@CUSTOMERID
           ,@MANANGERID
)",
                       helper.CreateParameter("NAME", "NAME")
                       , helper.CreateParameter("PRODUCTNAME", "Dell")
                       , helper.CreateParameter("AMOUNT", 101)
                       , helper.CreateParameter("DELIERDATE", DateTime.Today)
                       , helper.CreateParameter("CUSTOMERID", customerId)
                       , helper.CreateParameter("MANANGERID", 1)
                       );

            Assert.AreEqual(r, 1);
        }

        [TestMethod]
        public virtual void OracelMDATestExecDataSet()
        {
            DataSet ds = helper.ExecDataSet("SELECT * FROM USERPROFILE");

            Assert.AreEqual(ds.Tables.Count, 1);
        }


        [TestMethod]
        public void OracelMDATestGetListT()
        {
            List<Biz_Customer> listT = helper.GetList<Biz_Customer>("SELECT * FROM BIZ_CUSTOMER");

            Assert.IsNotNull(listT);
        }

    }
}
