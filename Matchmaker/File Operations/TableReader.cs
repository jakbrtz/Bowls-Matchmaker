using ClosedXML.Excel;
using System;
using System.IO;
using System.Linq;

namespace Matchmaker.FileOperations
{
    public interface ITableReader : IDisposable
    {
        bool ReadRow(out string[] values);
        int NumColumns { get; }
    }

    public class ExcelReader : ITableReader
    {
        readonly XLWorkbook workbook;
        readonly IXLWorksheet worksheet;

        readonly int totalRows;
        readonly int totalColumns;
        int row = 0;

        public ExcelReader(string filename)
        {
            workbook = new XLWorkbook(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            worksheet = workbook.Worksheets.First();

            totalRows = worksheet.LastRowUsed().RowNumber();
            totalColumns = worksheet.LastColumnUsed().ColumnNumber();
        }

        public bool ReadRow(out string[] values)
        {
            values = new string[totalColumns];
            if (row >= totalRows)
                return false;
            for (int i = 0; i < totalColumns; i++)
                values[i] = worksheet.Cell(row + 1, i + 1).Value.ToString();
            row++;
            return true;
        }

        public int NumColumns => totalColumns;

        public void Dispose()
        {
            workbook.Dispose();
        }
    }

    class SingleColumnReader : ITableReader
    {
        readonly StreamReader streamReader;

        public SingleColumnReader(string filename)
        {
            streamReader = new StreamReader(filename);
        }

        public bool ReadRow(out string[] values)
        {
            string line = streamReader.ReadLine();
            values = new string[] { line };
            return line != null;
        }

        public int NumColumns => 1;

        public void Dispose()
        {
            streamReader.Dispose();
        }
    }
}
