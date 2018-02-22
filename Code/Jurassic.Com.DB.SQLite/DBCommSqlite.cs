using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Data.Common;
using Jurassic.Com.Tools;

namespace Jurassic.Com.DB.SQLite
{
    /// <summary>
    /// SQLite的DBComm接口实现
    /// </summary>
    public class DBCommSQLite : IDBComm
    {
        public DBHelper Helper
        {
            get;
            set;
        }

        public DbConnection CreateConnection()
        {
            return new SQLiteConnection(Helper.ConnStr);
        }

        public DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return new SQLiteDataAdapter((SQLiteCommand)command);
        }

        public IDataParameter CreateParameter(string parameterName, object value)
        {
            if (value is byte[])
            {
                return CreateImageParameter(parameterName, (byte[])value);
            }
            var p = new SQLiteParameter(parameterName, CommOp.TestNull(value));
            return p;
        }

        public IDataParameter CreateImageParameter(string parameterName, byte[] value)
        {
            var p = new SQLiteParameter(parameterName, DbType.Object);
            p.Value = CommOp.TestNull(value);
            return p;
        }

        public IDataReader ExecPageReader(DBPagerInfo pager, params IDataParameter[] sp)
        {
            if (pager.OrderBy.IsEmpty())
            {
                throw new ArgumentException("pager.OrderBy");
            }
            string sql = "SELECT COUNT(*) FROM (" + pager.Query + ")c";

            pager.RecordCount = CommOp.ToInt(Helper.ExecGetObject(sql, sp));

            sql = String.Format(@"{0} ORDER BY {1} LIMIT {2} OFFSET {3}",
             pager.Query, pager.OrderBy, pager.PageSize, pager.StartIndex);
            return Helper.ExecReader(sql, sp);
        }

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
                case "int32": return "int";
                case "int64": return "bigint";
                case "bool": return "bit";
                case "byte[]": return "image";
                case "float": return "real";
                case "double": return "float";
                case "decimal": return "numeric(18,4)";
            }
            return "nvarchar(" + maxLength + ")";
        }

        public bool TableExists(string tableName)
        {
            object exist = Helper.ExecGetObject("SELECT count(*) FROM sqlite_master WHERE type='table' AND name=@tableName", Helper.CreateParameter("tableName", tableName));
            return CommOp.ToInt(exist) == 1;
        }

        public DbCommandBuilder CreateCommandBuilder(DbDataAdapter sda)
        {
            return new SQLiteCommandBuilder((SQLiteDataAdapter)sda);
        }

        public string ParamPrefix
        {
            get { return "@"; }
        }

        public string FieldPrefix
        {
            get { return "["; }
        }

        public string FieldSuffix
        {
            get { return "]"; }
        }
    }
}
