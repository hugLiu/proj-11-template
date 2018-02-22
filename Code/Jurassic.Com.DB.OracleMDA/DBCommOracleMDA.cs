using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess;
using System.Linq;
using System.Text;
using System.Data.Common;
using Jurassic.Com.Tools;
using Oracle.ManagedDataAccess.Client;

namespace Jurassic.Com.DB.OracleMDA
{
    /// <summary>
    /// Oracle的DBComm接口实现，基于Oracle提供的MDA
    /// MDA的特点是不需要额外安装客户端
    /// </summary>
    public class DBCommOracleMDA : IDBComm
    {
        public DBHelper Helper
        {
            get;
            set;
        }

        public DbConnection CreateConnection()
        {
            return new OracleConnection(Helper.ConnStr);
        }

        public DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return new OracleDataAdapter((OracleCommand)command);
        }

        public IDataParameter CreateParameter(string parameterName, object value)
        {
            if (value is byte[])
            {
                return CreateImageParameter(parameterName, (byte[])value);
            }
            var p = new OracleParameter(parameterName, CommOp.TestNull(value));
            return p;
        }

        public IDataParameter CreateImageParameter(string parameterName, byte[] value)
        {
            //var p = new OracleParameter(parameterName, DbType.Object);
            //p.Value = CommOp.TestNull(value);
            //return p;
            //张群龙改：
            var pp = new OracleParameter(parameterName, OracleDbType.Blob, value, ParameterDirection.InputOutput);
            return pp;
        }

        public IDataReader ExecPageReader(DBPagerInfo pager, params IDataParameter[] sp)
        {
            string sql = "SELECT COUNT(*) FROM (" + pager.Query + ")c";
            pager.RecordCount = CommOp.ToInt(Helper.ExecGetObject(sql, sp));
            sql = String.Format(@"SELECT * FROM(SELECT A.*, rownum r FROM({0} ORDER BY {1})A WHERE rownum <= {2})B WHERE r>{3}",
                pager.Query, pager.OrderBy, pager.StartIndex + pager.PageSize, pager.StartIndex);
            return Helper.ExecReader(sql, sp);
        }

        /*sqlserver的分页对比参考
        public IDataReader ExecPageReader(DBPagerInfo pager, params IDataParameter[] sp)
        {
            string sql = "SELECT COUNT(*) FROM (" + pager.Query + ")c";

            pager.RecordCount = (int)Helper.ExecGetObject(sql, sp);
            int startRow = pager.PageSize * (pager.PageIndex - 1) + 1;

            sql = String.Format(@"WITH PAGED AS ( 
SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS rowNum, 
* FROM ({1})a)
SELECT TT.*  FROM PAGED P INNER JOIN ({1})TT 
ON P.{2} = TT.{2}  WHERE ROWNUM BETWEEN {3} AND {4}
ORDER BY {0}",
            pager.OrderBy, pager.Query, pager.KeyId, startRow, startRow + pager.PageSize - 1);
            return Helper.ExecReader(sql, sp);
        }
        */

        public double GetDBSize()
        {
            throw new NotImplementedException();
        }

        public void ShrinkDB()
        {
            throw new NotImplementedException();
        }

        public void CreateTable(DataTable dt, string tableName)
        {
            if (tableName.IsEmpty())
            {
                tableName = dt.TableName;
            }

            string strSql = String.Format("create table {0}(", tableName);
            foreach (DataColumn c in dt.Columns)
            {
                strSql += string.Format("[{0}] {1},", c.ColumnName, TypeMappingName(c.DataType, c.MaxLength < 0 ? 255 : c.MaxLength));
            }
            strSql = strSql.Trim(',') + ")";

            Helper.ExecNonQuery(strSql);
        }

        public string TypeMappingName(Type type, int maxLength)
        {
            switch (type.Name.ToLower())
            {
                case "int32": return "number";
                case "int64": return "number";
                case "bool": return "number";
                case "byte[]": return "blob";
                case "float": return "number";
                case "double": return "number";
                case "decimal": return "numeric(18,4)";
            }
            return "nvarchar(" + maxLength + ")";
        }

        public bool TableExists(string tableName)
        {
            object exist = Helper.ExecGetObject("SELECT count(*) FROM user_tables WHERE table_name=@tableName", Helper.CreateParameter("tableName", tableName.ToUpper()));
            return CommOp.ToInt(exist) == 1;
        }

        public DbCommandBuilder CreateCommandBuilder(DbDataAdapter sda)
        {
            return new OracleCommandBuilder((OracleDataAdapter)sda);
        }

        public string ParamPrefix
        {
            get { return ":"; }
        }

        public string FieldPrefix
        {
            get { return "\""; }
        }

        public string FieldSuffix
        {
            get { return "\""; }
        }
    }
}
