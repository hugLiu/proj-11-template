using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Jurassic.Com.OfficeLib;
using System.Data;
using Jurassic.Com.Tools;

namespace Jurassic.OfficialSite.Tests
{
    [TestClass]
    public class OfficeLibTest
    {
        /// <summary>
        /// 测试07以下版本  +  DataTable  +  false
        /// </summary>
        [TestMethod]
        public void TestReadXls0()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2014年长庆油流向表油.xls");
          
            ExcelHelper helper = new ExcelHelper(path);
            DataTable dt = helper.ExcelToDataTable("2014年分年度", false);
            Assert.AreEqual(132.486469471686,CommOp.ToDouble(dt.Rows[17]["K"]));
        }

        /// <summary>
        /// 测试07以上版本  +  DataTable  +  false
        /// </summary>
        [TestMethod]
        public void TestReadXlsx0()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");

            ExcelHelper helper = new ExcelHelper(path);
            DataTable dt = helper.ExcelToDataTable("参数表", false);
            Assert.AreEqual( 3589.37,CommOp.ToDouble(dt.Rows[7]["C"]));
        }


        /// <summary>
        /// 下 + DataTable  +  true
        /// </summary>
        [TestMethod]
        public void TestReadXls1()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2014年长庆油流向表油.xls");

            ExcelHelper helper = new ExcelHelper(path);
            DataTable dt = helper.ExcelToDataTable("区块数", true);
            Assert.AreEqual(5, CommOp.ToDouble(dt.Rows[4]["序号"]));
        }

        /// <summary>
        /// 上 + DataTable  +  true
        /// </summary>
        [TestMethod]
        public void TestReadXlsx1()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");

            ExcelHelper helper = new ExcelHelper(path);
            DataTable dt = helper.ExcelToDataTable("类比表", true);
            Assert.AreEqual(3156.23, CommOp.ToDouble(dt.Rows[4]["类比气藏"]));
        }

        /// <summary>
        /// 上 + DataSet  +  false
        /// </summary>
        [TestMethod]
        public void TestGetDataSetReadXlsx()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");

            ExcelHelper helper = new ExcelHelper(path);
            DataSet ds = helper.ExcelToDataSet(false);
            Assert.AreEqual(3589.37, CommOp.ToDouble(ds.Tables["参数表"].Rows[7]["C"]));
        }

        /// <summary>
        /// 上 + DataSet  +  true
        /// </summary>
        [TestMethod]
        public void TestGetDataSetReadXlsx1()  
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");

            ExcelHelper helper = new ExcelHelper(path);
            DataSet ds = helper.ExcelToDataSet(true);
            Assert.AreEqual(3156.23, CommOp.ToDouble(ds.Tables["类比表"].Rows[4]["类比气藏"]));
        }

        /// <summary>
        /// 下 + DataSet  +  false
        /// </summary>
        [TestMethod]
        public void TestGetDataSetReadXls() 
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2014年长庆油流向表油.xls");

            ExcelHelper helper = new ExcelHelper(path);
            DataSet ds = helper.ExcelToDataSet(false);
            Assert.AreEqual(4, CommOp.ToInt(ds.Tables["区块数"].Rows[4]["A"]));
        }

        /// <summary>
        /// 下 + DataSet  +  true
        /// </summary>
        [TestMethod]
        public void TestGetDataSetReadXls1()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2014年长庆油流向表油.xls");
           
            ExcelHelper helper = new ExcelHelper(path);
            DataSet ds = helper.ExcelToDataSet(true);
            Assert.AreEqual(5, CommOp.ToInt(ds.Tables["区块数"].Rows[4]["序号"]));
        }

        /// <summary>
        /// DataTableToExcel + 下
        /// </summary>
        [TestMethod]
        public void TestDataTableToExcel()
        {
            int num;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "新建表--07--下.xls");
            using (ExcelHelper helper = new ExcelHelper(path))
            {
                //获得DataTable的数据
                string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");
                ExcelHelper helper2 = new ExcelHelper(path2);
                DataTable dt = helper2.ExcelToDataTable("类比表", true);

                num = helper.DataTableToExcel(dt, "类比表", true);
            }
            Assert.AreNotEqual(-1, num);//不等于-1说明写入成功
        }

        /// <summary>
        /// DataTableToExcel + 上
        /// </summary>
        [TestMethod]
        public void TestDataTableToExcel1()
        {
            int num;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "新建表--07--上.xlsx");
            using (ExcelHelper helper = new ExcelHelper(path))
            {
                //获得DataTable的数据
                string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");
                ExcelHelper helper2 = new ExcelHelper(path2);
                DataTable dt = helper2.ExcelToDataTable("类比表", true);

                num = helper.DataTableToExcel(dt, "类比表", true);
            }
            Assert.AreNotEqual(-1, num);//不等于-1说明写入成功
        }

        /// <summary>
        /// 测试07以下版本  + Stream --> DataTable  +  false
        /// </summary>
        [TestMethod]
        public void TestReadXlsStream0()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2014年长庆油流向表油.xls");

            Stream _innerStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            ExcelHelper helper = new ExcelHelper(_innerStream);
            DataTable dt = helper.ExcelToDataTable("2014年分年度", false);
            Assert.AreEqual(132.486469471686, CommOp.ToDouble(dt.Rows[17]["K"]));
        }

        /// <summary>
        /// 测试07以上版本  + Stream --> DataTable  +  false
        /// </summary>
        [TestMethod]
        public void TestReadXlsxStream0()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "2013年苏里格气田扩边新发现table.xlsx");

            Stream _innerStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            ExcelHelper helper = new ExcelHelper(_innerStream);
            DataTable dt = helper.ExcelToDataTable("参数表", false);
            Assert.AreEqual(3589.37, CommOp.ToDouble(dt.Rows[7]["C"]));
        }
    }
}
