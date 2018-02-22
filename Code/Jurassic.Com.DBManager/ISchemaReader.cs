using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jurassic.Com.DBManager.Models;

namespace Jurassic.Com.DBManager
{
    public interface ISchemaReader
    {

        /// <summary>
        /// 获取数据库中所有的表信息（转化为实体对象的形式）
        /// </summary>
        /// <returns>表集合</returns>
        List<DBTable> GetSchema();


        /// <summary>
        /// 获取数据库结构信息（DataSet的形式）
        /// </summary>
        /// <returns></returns>
        DataSet GetDataSet(List<string> tablesName, bool isGetData = false);


        /// <summary>
        /// 更新DataTable
        /// </summary>
        /// <param name="table"></param>
        void SaveDataTable(DataTable table);


        /// <summary>
        /// 更新DataSet
        /// </summary>
        /// <param name="ds"></param>
        void SaveDataSet(DataSet ds);
       

        /// <summary>
        /// 更新字段的描述信息
        /// </summary>
        /// <param name="field"></param>
        void SaveDescription(List<DBTable> field);
    }
}
