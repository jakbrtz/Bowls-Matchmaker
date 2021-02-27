using ClosedXML.Excel;
using System;
using System.IO;

namespace Matchmaker.FileOperations
{
    interface ITableWriter : IDisposable
    {
        void AddRow(params string[] values);
    }

    class ExcelWriter : ITableWriter
    {
        readonly string filename;
        readonly XLWorkbook workbook;
        readonly IXLWorksheet worksheet;

        int row = 0;

        public ExcelWriter(string filename)
        {
            this.filename = filename;
            workbook = new XLWorkbook();
            worksheet = workbook.Worksheets.Add("Sheet1");
        }

        public void AddRow(params string[] values)
        {
            for (int i = 0; i < values.Length; i++)
                worksheet.Cell(row + 1, i + 1).Value = values[i];
            row++;
        }

        public void Dispose()
        {
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(filename);
            workbook.Dispose();
        }
    }

    class SingleColumnWriter : ITableWriter
    {
        readonly StreamWriter streamWriter;

        public SingleColumnWriter(string filename)
        {
            streamWriter = new StreamWriter(filename);
        }

        public void AddRow(params string[] values)
        {
            if (values.Length != 1) throw new ArgumentException("SingleColumnWriter is only allowed to write one value per line");
            streamWriter.WriteLine(values[0]);
        }

        public void Dispose()
        {
            streamWriter.Dispose();
        }
    }
}
