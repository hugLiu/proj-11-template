using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Jurassic.Com.DBManager.Models
{
    public class DBRelation
    {
        public int Id { get; set; }
        public string ConstrName { get; set; }
        public string PrimaryTable { get; set; }
        public string ForeignTable { get; set; }
        public string PrimaryKey { get; set; }
        public string ForeignKey { get; set; }
        public string Datatype { get; set; }
        public bool IsUpdateCascade { get; set; }
        public bool IsDeleteCascade { get; set; }
    }
}
