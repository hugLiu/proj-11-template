using System.Collections.Generic;
using System.Data;

namespace Jurassic.Com.OfficeLib
{
    public interface IExcelHelper
    {
        int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten);
        DataSet ExcelToDataSet(bool isFirstRowColumn);
        DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn);
        DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn, int firstRowNum, int firstColumnNum);
        int ObjectToExcel<T>(IEnumerable<T> data, string sheetName, bool isColumnWritten);
    }
}