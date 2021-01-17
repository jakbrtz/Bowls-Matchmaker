using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace Matchmaker
{
    static class ReadWriteTable
    {
        public static bool ExportPlayerDetails(string filename, IList<Player> players)
        {
            using var writer = new ExcelWriter(filename);

            writer.AddRow("#", "Name", "Primary Position and Grade", "Secondary Position and Grade", "Preferred Team Size");

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                writer.AddRow(
                    player.TagNumber, 
                    player.Name,
                    player.PreferencePrimary.ToUserFriendlyString(),
                    player.PreferenceSecondary.ToUserFriendlyString(),
                    player.PreferredTeamSizes.ToUserFriendlyString());
            }

            return true;
        }

        public static bool ExportPlayersFromDay(string filename, Day day)
        {
            using SingleColumnWriter singleColumnWriter = new SingleColumnWriter(filename);
            foreach (Player player in Tools.PlayersInDay(day))
                singleColumnWriter.AddRow(player.Name);
            return true;
        }

        public static bool ImportPlayerForDay(string filename, List<Player> allPlayers, HashSet<Player> players)
        {
            using SingleColumnReader singleColumnReader = new SingleColumnReader(filename);
            players.Clear();
            while (singleColumnReader.ReadRow(out string[] values))
            {
                string name = values[0];
                Player bestMatch = null;
                int bestMatchRelevance = int.MaxValue;
                foreach (Player player in allPlayers)
                {
                    int relevance = Tools.RelevanceToSearch(player, name);
                    if (relevance < bestMatchRelevance)
                    {
                        bestMatchRelevance = relevance;
                        bestMatch = player;
                    }
                }
                if (bestMatch != null)
                {
                    players.Add(bestMatch);
                }
            }
            return true;
        }
    }
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
