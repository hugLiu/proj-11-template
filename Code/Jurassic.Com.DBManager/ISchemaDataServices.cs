using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic.Com.DBManager.Models;

namespace Jurassic.Com.DBManager
{
    public interface ISchemaDataServices
    {
        List<DBTable> DBTables { get; set; }
        DataSet DBDataSet { get; set; }

        /// <summary>
        ///  获取数据库结构，当传入表名时可获取相应表的数据
        /// </summary>
        /// <param name="isGetData">是否要获得表数据</param>
        /// <param name="tablesName">如果要获得数据，需有将获得表的名称集合</param>
        /// <returns></returns>
        DataSet GetDataSet(bool isGetData = false, List<string> tablesName = null);


        /// <summary>
        /// 获取数据库中表的结构和数据
        /// </summary>
        /// <param name="tableName">将获得结构信息的表名称</param>
        /// <param name="isGetData">是否同时获取数据</param>
        /// <returns></returns>
        DataTable GetDataTable(string tableName, bool isGetData = false);


        /// <summary>
        /// 更新DataTable
        /// </summary>
        /// <param name="table"></param>
        void Savetabel(DataTable table);


        /// <summary>
        /// 更新DataSet
        /// </summary>
        void SaveDataSet();

        /// <summary>
        /// 更新字段的描述信息
        /// </summary>
        /// <param name="field"></param>
        void SaveDescription(List<DBTable> tables);
    }
}
