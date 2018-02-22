using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jurassic.Com.DB;
using Jurassic.Com.DBManager.Models;
using Jurassic.Com.Tools;

namespace Jurassic.Com.DBManager
{
    public class SchemaReaderSql : ISchemaReader
    {
        private DBHelper _helper;
        private List<DBTable> tables;
       
        public SchemaReaderSql(DBHelper helper)
        {
            _helper = helper;
        }


        /// <summary>
        /// 获取数据库中所有的表信息（转化为实体对象的形式）
        /// </summary>
        /// <returns>表集合</returns>
        public List<DBTable> GetSchema()
        {
            tables = new List<DBTable>();
            string sqlstr = String.Format(@"
                     SELECT id=d.id,name = d.name, 
                         description = case when a.colorder = 1 then isnull(f.value, '') else '' end
                     FROM syscolumns a 
                     inner join sysobjects d 
                     on a.id = d.id  and d.xtype = 'U'  and d.name <> 'sysdiagrams'
                     left join sys.extended_properties   f 
                     on a.id = f.major_id and f.minor_id = 0
                     Where (case when a.colorder = 1 then d.name else '' end) <>''");
            //查找所有的用户表(获取到表的id以及name)
            var tb = _helper.ExecDataTable(sqlstr);
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                DBTable table = new DBTable
                {
                    Id = CommOp.ToInt(tb.Rows[i]["id"]),
                    Name = CommOp.ToStr(tb.Rows[i]["name"]),
                    Description = CommOp.ToStr(tb.Rows[i]["description"]),
                    Field = new List<DBField>(),
                    Reletion = new List<DBRelation>()
                };

                //获取用户表的所有字段
                var attrs = GetAllFields();
                foreach (DBField t in attrs.Where(t => t.DBTableId == table.Id))
                {
                    table.Field.Add(t);
                }

                //获取用户表的关系
                var relas = GetTableRelation();
                foreach (DBRelation t in relas.Where(t => t.PrimaryTable == table.Name || t.ForeignTable == table.Name))
                {
                    table.Reletion.Add(t);
                }

                tables.Add(table);
            }
            AddDescription(tables);
            return tables;
        }


        /// <summary>
        /// 获取表的所有字段
        /// </summary>
        /// <returns>字段集合</returns>
        private IEnumerable<DBField> GetAllFields()
        {
            List<DBField> tableAttrs = new List<DBField>();
            string sqlstr = String.Format(@"select tb1.*,tb2.isPrimaryKey from
                        (select c.Id as tableId ,object_name(c.id) as tableName,c.colid as fieldid,c.name as filedName,
	                	t.name as type,c.isnullable as isNullable,c.prec as length ,m.text as deaulftValue,ex.value as description,
                        (select COLUMNPROPERTY( c.id,c.name,'IsIdentity')) as isIdentifyField
                        from syscolumns c  
                        inner join systypes t  on c.xusertype=t.xusertype  
                        left join syscomments m  on c.cdefault=m.id
                        left join sys.extended_properties ex ON ex.major_id = c.id AND ex.minor_id = c.colid AND ex.name = 'MS_Description' 
						WHERE OBJECTPROPERTY(c.id, 'IsMsShipped')=0 and c.id in(select id from sysobjects where type='U' and name!='sysdiagrams'))tb1
                        left join
                        (SELECT TABLE_NAME,COLUMN_NAME,1 as isPrimaryKey FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
	                        where CONSTRAINT_NAME like 'PK_%')tb2 
                        on tb1.tableName=tb2.TABLE_NAME and tb1.filedName=tb2.COLUMN_NAME");
            var tb = _helper.ExecDataTable(sqlstr);
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                DBField tableAttr = new DBField
                {
                    Id = i + 1,
                    DBTableId = CommOp.ToInt(tb.Rows[i]["tableId"]),//所在表ID
                    DBTableName = CommOp.ToStr(tb.Rows[i]["tableName"]),
                    FieldName = CommOp.ToStr(tb.Rows[i]["filedName"]),//属性的名称
                    //DataType = SqlTypeStrToCsharpType(tb.Rows[i]["type"].ToString()),
                    DataType = tb.Rows[i]["type"].ToString(),
                    AllowNull = CommOp.ToInt(tb.Rows[i]["isNullable"]) != 0,
                    MaxLength = CommOp.ToInt(tb.Rows[i]["length"]),
                    DefaultValue = CommOp.ToStr(tb.Rows[i]["deaulftValue"]),
                    Description = CommOp.ToStr(tb.Rows[i]["description"]),
                    IsPrimaryKey = CommOp.ToInt(tb.Rows[i]["isPrimaryKey"]) != 0,
                    IsIdentifyField = CommOp.ToInt(tb.Rows[i]["isIdentifyField"]) != 0,
                };
                tableAttrs.Add(tableAttr);
            }
            return tableAttrs;
        }


        /// <summary>
        /// 根据表名获取与表相关的约束（主外键关系）
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private IEnumerable<DBRelation> GetTableRelation()
        {
            List<DBRelation> tableRelas = new List<DBRelation>();
            const string sqlstr = @"SELECT ConstrName=OBJECT_NAME(b.constid)
                                     ,ForeignTableId=b.fkeyid
	                                 ,ForeignTableName=object_name(b.fkeyid)
	                                 ,ForeignKey=(SELECT name FROM syscolumns WHERE colid=b.fkey AND id=b.fkeyid)
	                                 ,PrimaryTableId=b.rkeyid
	                                 ,PrimaryTableName=object_name(b.rkeyid)
	                                 ,PrimaryKey=(SELECT name FROM syscolumns WHERE colid=b.rkey AND id=b.rkeyid)
	                                 ,IsUpdateCascade=ObjectProperty(a.id,'CnstIsUpdateCascade') 
	                                 ,IsDeleteCascade=ObjectProperty(a.id,'CnstIsDeleteCascade')
	                                 FROM sysobjects a
	                                 join sysforeignkeys b on a.id=b.constid
	                                 join sysobjects c on a.parent_obj=c.id
	                                 where a.xtype='f' AND c.xtype='U'";
            var tb = _helper.ExecDataTable(sqlstr);
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                DBRelation tableRela = new DBRelation
                {
                    Id = i + 1,
                    ConstrName = CommOp.ToStr(tb.Rows[i]["ConstrName"]),//约束名
                    PrimaryTable = CommOp.ToStr(tb.Rows[i]["PrimaryTableName"]),
                    ForeignTable = CommOp.ToStr(tb.Rows[i]["ForeignTableName"]),
                    PrimaryKey = CommOp.ToStr(tb.Rows[i]["PrimaryKey"]),
                    ForeignKey = CommOp.ToStr(tb.Rows[i]["ForeignKey"]),
                    IsUpdateCascade = CommOp.ToStr(tb.Rows[i]["IsUpdateCascade"]) == "1",
                    IsDeleteCascade = CommOp.ToStr(tb.Rows[i]["IsDeleteCascade"]) == "1",
                };
                tableRelas.Add(tableRela);
            }
            return tableRelas;
        }


        /// <summary>
        /// 获取数据库结构信息（DataSet的形式）
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSet(List<string> tablesName, bool isGetData = false)
        {
            StringBuilder sqlsb = new StringBuilder();
            //拼接查询语句，获得DataSet集合
            if (isGetData)
            {
                foreach (var t in tablesName)
                {
                    sqlsb.AppendFormat("select * from {0}" + Environment.NewLine, t);
                }
            }
            else
            {
                foreach (var t in tablesName)
                {
                    sqlsb.AppendFormat("select * from {0} where 1=0" + Environment.NewLine, t);
                }
            }
            var ds = _helper.ExecDataSet(sqlsb.ToString());

            for (int i = 0; i < tablesName.Count(); i++)
            {
                ds.Tables[i].TableName = tablesName[i];
            }
            return ds;
        }


        /// <summary>
        /// 更新DataTable
        /// </summary>
        /// <param name="table"></param>
        public void SaveDataTable(DataTable table)
        {
            var trans = _helper.BeginTrans();
            DbConnection conn = trans.Connection;
            //设置select查询命令，SqlCommandBuilder要求至少有select命令
            DbCommand DBComm = conn.CreateCommand();
            DBComm.CommandText = String.Format("select * from {0} where 1=0", table.TableName);
            DBComm.Transaction = trans;
            DbDataAdapter sda = new SqlDataAdapter((SqlCommand)DBComm);
            DbCommandBuilder scb = new SqlCommandBuilder((SqlDataAdapter)sda);
            //执行更新
            if (table.GetChanges() != null)
            {
                sda.Update(table.GetChanges());
            }
            //使DataTable保存更新
            table.AcceptChanges();
            _helper.EndTrans();
        }


        /// <summary>
        /// 更新DataSet
        /// </summary>
        /// <param name="ds"></param>
        public void SaveDataSet(DataSet ds)
        {
            List<DBRelation> list = GetTableRelation().ToList();
            List<string> nameList = new List<string>();
            foreach (var l in list)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    //不是主表或者已经更新过的主表，继续循环
                    if (ds.Tables[i].TableName != l.PrimaryTable || nameList.Contains(ds.Tables[i].TableName)) continue;
                    nameList.Add(ds.Tables[i].TableName);
                    //先更新主表中的数据
                    SaveDataTable(ds.Tables[i]);
                    break;
                }
            }
           
            //再更新从表中的数据
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                //已经更新过数据的表跳过
                if (!nameList.Contains(ds.Tables[i].TableName))

                SaveDataTable(ds.Tables[i]);
            }

        }


        /// <summary>
        /// 获取表结构的的时候初始化表和字段的描述信息
        /// </summary>
        /// <param name="field"></param>
        private void AddDescription(List<DBTable> tables)
        {
            StringBuilder sqlsb = new StringBuilder();
            foreach (var t in tables.Where(f => f.Description == ""))
            {
                sqlsb.AppendFormat(@"EXEC sp_addextendedproperty 
                              'MS_Description',{0},'user',dbo,'table',{1}" + Environment.NewLine,
                    t.Name, t.Name);
            }
            foreach (var f in tables.SelectMany(t => t.Field.Where(f => f.Description == "")))
            {
                sqlsb.AppendFormat(@"EXEC sp_addextendedproperty 
                              'MS_Description',{0},'user',dbo,'table',{1},'column',{2}" + Environment.NewLine,
                    f.FieldName, f.DBTableName, f.FieldName);
            }

            if (sqlsb.ToString() != "")
            {
                _helper.ExecNonQuery(sqlsb.ToString());
                tables = GetSchema();
            }
        }


        /// <summary>
        /// 更新字段的描述信息
        /// </summary>
        /// <param name="tables"></param>
        public void SaveDescription(List<DBTable> tables)
        {
            StringBuilder sqlsb = new StringBuilder();

            foreach (var t in tables)
            {
                sqlsb.AppendFormat(@"EXEC sp_updateextendedproperty 
                              'MS_Description',{0},'user',dbo,'table',{1}" + Environment.NewLine,
                    t.Description, t.Name);

                foreach (var f in t.Field)
                {
                    sqlsb.AppendFormat(@"EXEC sp_updateextendedproperty 
                              'MS_Description',{0},'user',dbo,'table',{1},'column',{2}" + Environment.NewLine,
                        f.Description, f.DBTableName, f.FieldName);
                }
            }

            if (sqlsb.ToString() != "")
                _helper.ExecNonQuery(sqlsb.ToString());
        }




        #region SqlDbType转换为C#数据类型

        /// <summary>
        /// SqlDbType转换为C#数据类型
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        private Type SqlTypeToCsharpType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(Int64);
                case SqlDbType.Binary:
                    return typeof(Byte[]);
                case SqlDbType.Bit:
                    return typeof(Boolean);
                case SqlDbType.Char:
                    return typeof(String);
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    return typeof(DateTime);
                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset);
                case SqlDbType.Decimal:
                    return typeof(Decimal);
                case SqlDbType.Float:
                    return typeof(Double);
                case SqlDbType.Image:
                    return typeof(Byte[]);
                case SqlDbType.Int:
                    return typeof(Int32);
                case SqlDbType.Money:
                    return typeof(Decimal);
                case SqlDbType.NChar:
                    return typeof(String);
                case SqlDbType.NText:
                    return typeof(String);
                case SqlDbType.NVarChar:
                    return typeof(String);
                case SqlDbType.Real:
                    return typeof(Single);
                case SqlDbType.SmallDateTime:
                    return typeof(DateTime);
                case SqlDbType.SmallInt:
                    return typeof(Int16);
                case SqlDbType.SmallMoney:
                    return typeof(Decimal);
                case SqlDbType.Structured:
                    return typeof(Object);
                case SqlDbType.Text:
                    return typeof(String);
                case SqlDbType.Timestamp:
                    return typeof(Byte[]);
                case SqlDbType.TinyInt:
                    return typeof(Byte);
                case SqlDbType.Udt: //自定义的数据类型
                    return typeof(Object);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                case SqlDbType.VarBinary:
                    return typeof(Byte[]);
                case SqlDbType.VarChar:
                    return typeof(String);
                case SqlDbType.Variant:
                    return typeof(Object);
                case SqlDbType.Xml:
                    return typeof(Object);
                default:
                    return typeof(Object);
            }
        }


        /// <summary>
        /// sql server数据类型（如：varchar）转换为SqlDbType类型
        /// </summary>
        /// <param name="sqlTypeString"></param>
        /// <returns></returns>
        private SqlDbType SqlTypeStrToSqlType(string sqlTypeString)
        {
            SqlDbType dbType;

            switch (sqlTypeString.ToLower())
            {
                case "int":
                    dbType = SqlDbType.Int;
                    break;
                case "varchar":
                    dbType = SqlDbType.VarChar;
                    break;
                case "bit":
                    dbType = SqlDbType.Bit;
                    break;
                case "datetime":
                    dbType = SqlDbType.DateTime;
                    break;
                case "date":
                    dbType = SqlDbType.Date;
                    break;
                case "time":
                    dbType = SqlDbType.Time;
                    break;
                case "datetime2":
                    dbType = SqlDbType.DateTime2;
                    break;
                case "datetimeoffset":
                    dbType = SqlDbType.DateTimeOffset;
                    break;
                case "decimal":
                    dbType = SqlDbType.Decimal;
                    break;
                case "float":
                    dbType = SqlDbType.Float;
                    break;
                case "image":
                    dbType = SqlDbType.Image;
                    break;
                case "money":
                    dbType = SqlDbType.Money;
                    break;
                case "ntext":
                    dbType = SqlDbType.NText;
                    break;
                case "nvarchar":
                    dbType = SqlDbType.NVarChar;
                    break;
                case "smalldatetime":
                    dbType = SqlDbType.SmallDateTime;
                    break;
                case "smallint":
                    dbType = SqlDbType.SmallInt;
                    break;
                case "text":
                    dbType = SqlDbType.Text;
                    break;
                case "bigint":
                    dbType = SqlDbType.BigInt;
                    break;
                case "binary":
                case "geography":
                case "geometry":
                case "hierarchyid":
                    dbType = SqlDbType.Binary;
                    break;
                case "char":
                    dbType = SqlDbType.Char;
                    break;
                case "nchar":
                    dbType = SqlDbType.NChar;
                    break;
                case "numeric":
                    dbType = SqlDbType.Decimal;
                    break;
                case "real":
                    dbType = SqlDbType.Real;
                    break;
                case "smallmoney":
                    dbType = SqlDbType.SmallMoney;
                    break;
                case "sql_variant":
                    dbType = SqlDbType.Variant;
                    break;
                case "timestamp":
                    dbType = SqlDbType.Timestamp;
                    break;
                case "tinyint":
                    dbType = SqlDbType.TinyInt;
                    break;
                case "uniqueidentifier":
                    dbType = SqlDbType.UniqueIdentifier;
                    break;
                case "varbinary":
                    dbType = SqlDbType.VarBinary;
                    break;
                case "xml":
                    dbType = SqlDbType.Xml;
                    break;
                default:
                    dbType = SqlDbType.Variant;
                    break;
            }
            return dbType;
        }

        /// <summary>
        /// sql server中的数据类型，转换为C#中的类型类型
        /// </summary>
        /// <param name="sqlTypeString"></param>
        /// <returns></returns>
        public Type SqlTypeStrToCsharpType(string sqlTypeString)
        {
            SqlDbType dbTpe = SqlTypeStrToSqlType(sqlTypeString);

            return SqlTypeToCsharpType(dbTpe);
        }

        #endregion

    }
}

