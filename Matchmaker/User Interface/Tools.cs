using Matchmaker.Data;
using System.Collections;
using System.IO;
using System.Linq;

namespace Matchmaker.UserInterface
{
    static class Tools
    {
        public static string ShortListDescription(this IList list)
        {
            return list.Count switch
            {
                1 => list[0] + "",
                2 => list[0] + " and " + list[1],
                3 => list[0] + ", " + list[1] + " and " + list[2],
                _ => list[0] + ", " + list[1] + " and " + (list.Count - 2) + " others",
            };
        }

        public static void PickNumGamesForPlayers(int numPlayers, int preferredSize, out int numPairs, out int numTrips, out int numFours)
        {
            numPairs = 0;
            numTrips = 0;
            numFours = 0;

            if (numPlayers < 4) return;

            if (numPlayers % 2 != 0) numPlayers--;

            switch (preferredSize)
            {
                case 2:
                    numPairs = numPlayers / 4;
                    switch (numPlayers % 4)
                    {
                        case 2:
                            numPairs--;
                            numTrips++;
                            break;
                    }
                    break;
                case 3:
                    numTrips = numPlayers / 6;
                    switch (numPlayers % 6)
                    {
                        case 2:
                            numTrips--;
                            numPairs += 2;
                            break;
                        case 4:
                            numPairs++;
                            break;
                    }
                    break;
                case 4:
                    numFours = numPlayers / 8;
                    switch (numPlayers % 8)
                    {
                        case 2:
                            numFours--;
                            numPairs ++;
                            numTrips++;
                            break;
                        case 4:
                            numPairs++;
                            break;
                        case 6:
                            numTrips++;
                            break;
                    }
                    break;
            }
        }

        public static string GuessFilename(Day day)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            string result = string.Empty;
            foreach (char c in day.date)
                if (!invalidChars.Contains(c))
                    result += c;
            return result;
        }
    }
}
