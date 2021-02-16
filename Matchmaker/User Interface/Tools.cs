using Matchmaker.Collections;
using Matchmaker.Data;
using System.Collections;
using System.Collections.Generic;
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

        public static void PickNumGamesForPlayers(int numPlayers, MatchSize preferredSize, out Counter<MatchSize> numberOfPlayers)
        {
            numberOfPlayers = new Counter<MatchSize>();
            // If there are only 4 players then there's no way to set this up
            if (numPlayers < 4) return;
            // How many players in the preference size?
            int sizePref = preferredSize.team1Size + preferredSize.team2Size;
            // How many games can we make?
            numberOfPlayers[preferredSize] = numPlayers / sizePref;
            // How many players are left over?
            int leftovers = numPlayers % sizePref;
            // If we don't have any leftovers then we're done
            if (leftovers == 0) return;
            // We don't have enough leftover players to make a game, so give up one of the games of preferred size
            if (leftovers < 4 && numberOfPlayers[preferredSize] > 0)
            {
                numberOfPlayers[preferredSize]--;
                leftovers += sizePref;
            }
            // We have an odd number of leftover players but our preferred size was odd, so give up one of the games of preferred size
            if (leftovers % 2 == 1 && numberOfPlayers[preferredSize] > 0 && sizePref % 2 == 1)
            {
                numberOfPlayers[preferredSize]--;
                leftovers += sizePref;
            }
            // We have an odd number of leftover players, so add an uneven match
            if (leftovers % 2 == 1)
            {
                if (leftovers == 7)
                {
                    numberOfPlayers[MatchSize.FourVsTrip]++;
                    leftovers -= 7;
                }
                else
                {
                    numberOfPlayers[MatchSize.TripVsPair]++;
                    leftovers -= 5;
                }
            }
            // Add pairs to fill up the remaining spots
            while (leftovers >= 4)
            {
                numberOfPlayers[MatchSize.Pairs]++;
                leftovers -= 4;
            }
            // If we were left with 2 players, remove a pair and add a trip
            if (leftovers == 2)
            {
                numberOfPlayers[MatchSize.Pairs]--;
                leftovers += 4;
                numberOfPlayers[MatchSize.Triples]++;
                leftovers -= 6;
            }
        }

        public static int SumOfPlayersInMatchSizes(Counter<MatchSize> sizes)
        {
            return sizes.Sum(kvp => kvp.Value * kvp.Key.TotalSize);
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
