using Matchmaker.Collections;
using Matchmaker.Data;
using System.Collections;
using System.Diagnostics;
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

        public static void PickNumGamesForPlayers(int numPlayers, MatchSize preferredSize, out Counter<MatchSize> numMatchSizes)
        {
            numMatchSizes = new Counter<MatchSize>();
            // If there are only 4 players then there's no way to set this up
            if (numPlayers < 4) return;
            // How many games can we make?
            numMatchSizes[preferredSize] = numPlayers / preferredSize.TotalSize;
            // How many players are left over?
            int leftovers = numPlayers % preferredSize.TotalSize;
            // If we don't have any leftovers then we're done
            if (leftovers == 0) return;
            // We don't have enough leftover players to make a game, so give up one of the games of preferred size
            if (leftovers < 4 && numMatchSizes[preferredSize] > 0)
            {
                numMatchSizes[preferredSize]--;
                leftovers += preferredSize.TotalSize;
            }
            Debug.Assert(leftovers >= 4, "There should be at least 4 leftovers");
            // We have an odd number of leftover players but our preferred size was odd, so give up one of the games of preferred size
            if (leftovers % 2 == 1 && numMatchSizes[preferredSize] > 0 && preferredSize.TotalSize % 2 == 1)
            {
                numMatchSizes[preferredSize]--;
                leftovers += preferredSize.TotalSize;
            }
            Debug.Assert(leftovers % 2 == 0 || preferredSize.TotalSize % 2 == 0, "There should be an even number of leftovers, unless there is an even preferred size");
            // We have an odd number of leftover players, so add an uneven match
            if (leftovers % 2 == 1)
            {
                if (leftovers == 7)
                {
                    numMatchSizes[MatchSize.FourVsTrip]++;
                    leftovers -= 7;
                }
                else
                {
                    numMatchSizes[MatchSize.TripVsPair]++;
                    leftovers -= 5;
                }
            }
            Debug.Assert(leftovers % 2 == 0, "There should be an even number of leftovers");
            // Add pairs to fill up the remaining spots
            while (leftovers >= 4)
            {
                numMatchSizes[MatchSize.Pairs]++;
                leftovers -= 4;
            }
            // If we were left with 2 players, remove a pair and add a trip
            if (leftovers == 2)
            {
                numMatchSizes[MatchSize.Pairs]--;
                leftovers += 4;
                numMatchSizes[MatchSize.Triples]++;
                leftovers -= 6;
            }
            Debug.Assert(leftovers == 0, "There shouldn't be any more leftovers");
            Debug.Assert(SumOfPlayersInMatchSizes(numMatchSizes) == numPlayers, "The match sizes should be distributed exactly");
        }

        public static int SumOfPlayersInMatchSizes(Counter<MatchSize> numMatchSizes)
        {
            return numMatchSizes.Sum(kvp => kvp.Value * kvp.Key.TotalSize);
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
