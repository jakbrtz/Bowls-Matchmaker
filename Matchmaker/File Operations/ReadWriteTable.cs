using System.Collections.Generic;
using System.Linq;
using Matchmaker.Data;
using Matchmaker.DataHandling;
using Matchmaker.UserInterface.StringConverters;
using Day = Matchmaker.Data.Day;

namespace Matchmaker.FileOperations
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
            foreach (Player player in day.Players())
                singleColumnWriter.AddRow(player.Name);
            return true;
        }

        public static void ImportPlayerForDay(string filename, List<Player> allPlayers, HashSet<Player> importedPlayers, out List<string> namesNotImported)
        {
            // Clear the output lists
            importedPlayers.Clear();
            namesNotImported = new List<string>();
            // Read from the file
            using (SingleColumnReader singleColumnReader = new SingleColumnReader(filename))
            {
                while (singleColumnReader.ReadRow(out string[] values))
                {
                    // Look for names that are good matches
                    string name = values[0];
                    Player exactMatch = Search.GetExactMatch(allPlayers, name, importedPlayers);
                    if (exactMatch != null)
                        importedPlayers.Add(exactMatch);
                    else
                        namesNotImported.Add(name);
                }
            }
            // Looks through the failed matches and search for approximate matches
            for (int i = namesNotImported.Count - 1; i >= 0; i--)
            {
                string name = namesNotImported[i];
                Player bestMatch = Search.GetBestMatch(allPlayers, name, importedPlayers);
                if (bestMatch != null)
                {
                    importedPlayers.Add(bestMatch);
                    namesNotImported.RemoveAt(i);
                }
            }
        }
    }
}
