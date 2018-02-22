using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Jurassic.Com.DB;
using Jurassic.Com.DBManager;

namespace DBHelperTests
{
    [TestClass]
    public class DBManagerTest
    {
        [TestMethod]
        public void TestSchemaDataSet()
        {
            //DBHelper helper = new DBHelper("DefaultConnection");
            DBHelper helper = new DBHelper("Data Source=.;DataBase=LibraryManaStm;Integrated Security=SSPI");
            SchemaReaderSql rdr = new SchemaReaderSql(helper);
            ISchemaDataServices sd = new SchemaDataServices(rdr);


            List<string> list = new List<string>();

            for (int i = 0; i < sd.DBTables.Count - 5; i++)
            {
                list.Add(sd.DBTables[i].Name);
            }
            //var aa = sd.GetDataSet(true, list);
            var tb = sd.GetDataTable(sd.DBTables[0].Name, true);

            helper.ExecNonQuery("delete from " + sd.DBTables[0].Name);

            var tb2 = sd.GetDataTable(sd.DBTables[0].Name, true);
            //测试批量复制的方法
            helper.Import(tb, tb.TableName, true, 1, delegate(int i)
            {

                Console.WriteLine(i + "asdfsad");
            });

            var tb3 = sd.GetDataTable(sd.DBTables[0].Name, true);

            Assert.IsTrue(sd.DBTables.Count > 0);
        }

        [TestMethod]
        public void TestSaveDataTable()
        {
            DBHelper helper = new DBHelper("DefaultConnection");
            SchemaReaderSql rdr = new SchemaReaderSql(helper);
            ISchemaDataServices sd = new SchemaDataServices(rdr);
            var tb = sd.GetDataTable(sd.DBTables[0].Name, true);
            int count1 = tb.Rows.Count;

            DataRow dr = tb.NewRow();
            dr["UserName"] = "Percy";
            dr["Email"] = 32342;
            dr["PhoneNumber"] = 354353543;
            tb.Rows.Add(dr);


            //调用服务端Save
            sd.Savetabel(tb);

            int count2 = tb.Rows.Count;
            Assert.IsTrue(count2 - count1 == 1);
        }


        [TestMethod]
        public void TestSaveDataSet()
        {
            DBHelper helper = new DBHelper("DefaultConnection");
            SchemaReaderSql rdr = new SchemaReaderSql(helper);
            ISchemaDataServices sd = new SchemaDataServices(rdr);
           // var tb = sd.GetDataTable(sd.DBTables[0].Name, true);
            //获取DataTable表的list
            List<string> list = new List<string>();
            for (int i = 0; i < sd.DBTables.Count - 5; i++)
            {
                list.Add(sd.DBTables[i].Name);
            }

            var ds = sd.GetDataSet(true, list);
            var tb = ds.Tables[0];
            int count1 = tb.Rows.Count;

            DataRow dr = tb.NewRow();
            dr["UserName"] = "Percy";
            dr["Email"] = 32342;
            dr["PhoneNumber"] = 354353543;
            tb.Rows.Add(dr);


            
            //调用服务端Save
            sd.SaveDataSet();

            var tb2 = sd.GetDataTable(sd.DBTables[0].Name, true);
            int count2 = tb2.Rows.Count;

            Assert.IsTrue(count2 - count1 == 1);
        }

        [TestMethod]
        public void TestDataType()
        {
            DBHelper helper = new DBHelper("DefaultConnection");

            SchemaReaderSql rdr = new SchemaReaderSql(helper);
            ISchemaDataServices sd = new SchemaDataServices(rdr);

        }


        [TestMethod]
        public void TestSaveFeildDesc()
        {
            //DBHelper helper = new DBHelper("DefaultConnection");

            DBHelper helper = new DBHelper("Data Source=.;DataBase=LibraryManaStm;Integrated Security=SSPI");
            SchemaReaderSql rdr = new SchemaReaderSql(helper);
            ISchemaDataServices sd = new SchemaDataServices(rdr);

            var tables = sd.DBTables;
            sd.SaveDescription(tables);
        }
    }
}
