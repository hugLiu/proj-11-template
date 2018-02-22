using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DBHelperTests
{
    public static class TestObjects
    {
        public static DataTable MakeCustomerTable()
        {
            DataTable dt = new DataTable("Biz_Customer");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("Tel", typeof(string));
            dt.Columns.Add("ManagerId", typeof(int));
            dt.Rows.Add(10, "Peter", "NewYork", "0736-1234567", null);
            dt.Rows.Add(11, "Sun", "Beijing", "0716-5664567", null);
            dt.Rows.Add(12, "Movie", "Wuhan", "027-12345678", null);
            return dt;
        }
        public static DataTable MakeCustomerTableUpper()
        {
            DataTable dt = new DataTable("BIZ_CUSTOMER");
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("ADDRESS", typeof(string));
            dt.Columns.Add("TEL", typeof(string));
            dt.Columns.Add("MANAGERID", typeof(int));
            dt.Rows.Add(10, "Peter", "NewYork", "0736-1234567", null);
            dt.Rows.Add(11, "Sun", "Beijing", "0716-5664567", null);
            dt.Rows.Add(12, "Movie", "Wuhan", "027-12345678", null);
            return dt;
        }
    }

    class Biz_Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Tel { get; set; }

        public int? ManagerId { get; set; }
    }
}
