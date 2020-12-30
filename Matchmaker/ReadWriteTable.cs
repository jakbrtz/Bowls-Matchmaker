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

            writer.AddRow("#", "Name", "Primary Position", "Secondary Position", "Preferred Team Size");

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

        public static bool ImportPlayerDetails(string filename, IList<Player> players)
        {
            using var reader = new ExcelReader(filename);

            // Skip the header
            reader.ReadRow(0, out _);

            while (reader.ReadRow(5, out string[] row))
            {
                // todo: importer where the user selects what the columns are

                // Start reading the data
                var tag = row[0];
                var name = row[1];

                // Skip rows that don't belong to anyone
                if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(name))
                {
                    continue;
                }

                // Resume reading the data
                var primary = EnumParser.ParsePositionAndGrade(row[2]);
                var secondary = EnumParser.ParsePositionAndGrade(row[3]);
                var teamsize = EnumParser.TryParseTeamSize(row[4], out TeamSize ts) ? ts : TeamSize.Any;

                // Find the player
                Player player = players.FirstOrDefault(p => p.Name.Equals(name));
                if (player == null && !string.IsNullOrEmpty(tag))
                {
                    player = players.FirstOrDefault(p => p.TagNumber.Equals(tag));
                }
                if (player == null)
                {
                    player = new Player { ID = Tools.UniqueRandomInt(players) };
                    players.Add(player);
                }

                // Fill in the player's details
                player.TagNumber = tag;
                player.Name = name;
                player.PreferencePrimary = primary;
                player.PreferenceSecondary = secondary;
                player.PreferredTeamSizes = teamsize;
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
            while (singleColumnReader.ReadRow(1, out string[] values))
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

        class CSVWriter : ITableWriter
        {
            readonly StreamWriter streamWriter;

            public CSVWriter(string filename)
            {
                streamWriter = new StreamWriter(filename);
            }

            public void AddRow(params string[] values)
            {
                string line = string.Empty;
                foreach (string value in values)
                {
                    if (!string.IsNullOrEmpty(value))
                        line += ", ";
                    line += "\"" + value.Replace("\"", "\\\"") + "\"";
                    // todo: escape escape characters, and their edge cases
                }
                streamWriter.WriteLine(line);
            }

            public void Dispose()
            {
                streamWriter.Dispose();
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

        interface ITableReader : IDisposable
        {
            bool ReadRow(int columns, out string[] values);
        }

        class ExcelReader : ITableReader
        {
            readonly XLWorkbook workbook;
            readonly IXLWorksheet worksheet;

            readonly int totalRows;
            int row = 0;

            public ExcelReader(string filename)
            {
                workbook = new XLWorkbook(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                worksheet = workbook.Worksheets.First();

                totalRows = worksheet.LastRowUsed().RowNumber();
            }

            public bool ReadRow(int columns, out string[] values)
            {
                values = new string[columns];
                if (row >= totalRows)
                    return false;
                for (int i = 0; i < columns; i++)
                    values[i] = worksheet.Cell(row + 1, i + 1).Value.ToString();
                row++;
                return true;
            }

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

            public bool ReadRow(int columns, out string[] values)
            {
                if (columns != 1) throw new ArgumentException("SingleColumnReader only has one value per line");
                string line = streamReader.ReadLine();
                values = new string[] { line };
                return line != null;
            }

            public void Dispose()
            {
                streamReader.Dispose();
            }
        }
    }
}
