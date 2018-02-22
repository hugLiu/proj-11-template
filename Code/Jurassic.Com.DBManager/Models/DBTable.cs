using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Jurassic.Com.DBManager.Models
{
    public class DBTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private List<DBField> _field = new List<DBField>();

        public List<DBField> Field
        {
            get { return _field; }
            set { _field = value; }
        }
        //public List<DBField> Field { get; set; }
        private List<DBRelation> _reletion = new List<DBRelation>();

        public List<DBRelation> Reletion
        {
            get { return _reletion; }
            set { _reletion = value; }
        }

    }
}
