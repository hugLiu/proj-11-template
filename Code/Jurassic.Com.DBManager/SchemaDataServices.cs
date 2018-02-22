using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic.Com.DBManager.Models;

namespace Jurassic.Com.DBManager
{
    public class SchemaDataServices : ISchemaDataServices
    {
        ISchemaReader _schemaReader;
        public List<DBTable> DBTables { get; set; }

        public DataSet DBDataSet { get; set; }
        public SchemaDataServices(ISchemaReader schemaReader)
        {
            _schemaReader = schemaReader;
            GetAllTables();
        }

        /// <summary>
        /// 获取整个数据库的结构信息（包括表名、字段信息、约束信息）
        /// </summary>
        private void GetAllTables()
        {
            DBTables = _schemaReader.GetSchema();
            DBDataSet = GetDataSet();
        }


        /// <summary>
        ///  获取数据库结构
        /// </summary>
        /// <param name="isGetData">是否要获得表数据</param>
        /// <param name="tablesName">如果要获得数据，需有将获得表的名称集合</param>
        /// <returns></returns>
        public DataSet GetDataSet(bool isGetData = false, List<string> tablesName = null)
        {
            DataSet ds = new DataSet("MyDataSet");

            //仅数据库结构（不包括数据）
            if (!isGetData)
            {
                tablesName = DBTables.Select(t => t.Name).ToList();
            }
            DBDataSet = _schemaReader.GetDataSet(tablesName, isGetData);
            return DBDataSet;
        }


        /// <summary>
        /// 获取数据库中表的结构
        /// </summary>
        /// <param name="tableName">将获得结构信息的表名称</param>
        /// <param name="isGetData">是否同时获取数据</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, bool isGetData = false)
        {
            DataTable dt = new DataTable(tableName);

            dt = _schemaReader.GetDataSet(new List<string> { tableName }, isGetData).Tables[0];

            return dt;
        }


        /// <summary>
        /// 更新DataTable
        /// </summary>
        /// <param name="table"></param>
        public void Savetabel(DataTable table)
        {
            _schemaReader.SaveDataTable(table);
        }


        /// <summary>
        /// 更新DataSet
        /// </summary>
        /// <param name="ds"></param>
        public void SaveDataSet()
        {
            _schemaReader.SaveDataSet(DBDataSet);
        }

        /// <summary>
        /// 更新字段的描述信息
        /// </summary>
        /// <param name="field"></param>
        public void SaveDescription(List<DBTable> tables)
        {
            _schemaReader.SaveDescription(tables);
            GetAllTables();
        }
    }
}
