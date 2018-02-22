using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Jurassic.Com.DBManager.Models
{
    public class DBField
    {
        public int Id { get; set; }
        public int DBTableId { get; set; }
        public string DBTableName { get; set; }
        public string FieldName { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public int MaxLength { get; set; }
        public bool AllowNull { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentifyField { get; set; }
        public string Description { get; set; }

    }
}
