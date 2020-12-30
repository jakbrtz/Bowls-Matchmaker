using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Matchmaker
{
    static class Tools
    {
        public static string NextTagNumber(IList<Player> players)
        {
            bool[] used = new bool[players.Count + 1];
            foreach (Player player in players)
                if (int.TryParse(player.TagNumber, out int number))
                    if (number >= 0 && number < used.Length)
                        used[number] = true;
            for (int i = 1; i < used.Length; i++)
                if (!used[i])
                    return i.ToString();
            return (players.Count + 1).ToString();
        }

        public static bool Filter(Player player, string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return player.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) != -1 || player.TagNumber?.ToString().Contains(search) == true;
        }

        public static int RelevanceToSearch(Player player, string search)
        {
            foreach (string searchWord in search.Split(' '))
            {
                var names = player.Name.Split(' ');
                for (int i = 0; i < names.Length; i++)
                    if (names[i].StartsWith(searchWord, StringComparison.OrdinalIgnoreCase))
                        return i;
            }
            return int.MaxValue;
        }

        public static int PlayerCompare(Player player1, Player player2)
        {
            if (player1.Visitor && !player2.Visitor)
                return 1;
            if (!player1.Visitor && player2.Visitor)
                return -1;
            int result = player1.Name.CompareTo(player2.Name);
            if (result != 0) return result;
            result = TagNumberCompare(player1, player2);
            if (result != 0) return result;
            result = player1.ID.CompareTo(player2.ID);
            return result;
        }

        public static int TagNumberCompare(Player player1, Player player2)
        {
            return NumericTextCompare(player1.TagNumber, player2.TagNumber);
        }

        public static int MatchRinkCompare(Match match1, Match match2)
        {
            return NumericTextCompare(match1.rink, match2.rink);
        }

        public static int NumericTextCompare(string str1, string str2)
        {
            if (str1 == null) str1 = string.Empty;
            if (str2 == null) str2 = string.Empty;
            bool str1IsNumber = double.TryParse(str1.Split(' ')[0], out double number1);
            bool str2IsNumber = double.TryParse(str2.Split(' ')[0], out double number2);
            if (str1IsNumber && str2IsNumber)
            {
                if (number1 > number2)
                    return 1;
                if (number2 > number1)
                    return -1;
            }
            if (str1IsNumber && !str2IsNumber)
                return -1;
            if (str2IsNumber && !str1IsNumber)
                return 1;
            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
                return -1;
            if (!string.IsNullOrEmpty(str2) && string.IsNullOrEmpty(str1))
                return 1;
            return str1.CompareTo(str2);
        }

        public static int UniqueRandomInt(IList<Player> existingPlayers)
        {
            HashSet<int> existing = new HashSet<int>();
            foreach (Player player in existingPlayers)
                existing.Add(player.ID);
            int chosen;
            do
            {
                chosen = random.Next();
            }
            while (existing.Contains(chosen));
            return chosen;
        }

        public static Random random = new Random();

        public static Player FindPlayerInDay(Day day, int id, out Team team, out int position)
        {
            foreach (Match match in day.matches)
            {
                foreach (Team t in match.teams)
                {
                    for (position = 0; position < Team.MaxSize; position++)
                    {
                        if (t.players[position]?.ID == id)
                        {
                            team = t;
                            return t.players[position];
                        }
                    }
                }
            }
            team = null;
            position = 0;
            return null;
        }

        private static readonly Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static double Score(this Day day)
        {
            double total = 0;
            for (int i = 0; i < day.matches.Count; i++)
                for (int j = 0; j < day.matches[i].penalties.Count; j++)
                    total += day.matches[i].penalties[j].Score();
            return total;
        }

        public static int DifferenceBetweenPositions(Position position1, Position position2, int size)
        {
            // This condition stops the program from compiling if I ever change MaxSize
            if (Team.MaxSize == 4)
            {
                if (position1 == position2) return 0;
                if (size == 2) return 1;
                if (size == 3)
                {
                    if (position1 == Position.Second || position2 == Position.Second)
                        return 1;
                    return 2;
                }
                int result = position1 - position2;
                if (result < 0) result = -result;
                return result;
            }
        }

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

        public static IEnumerable<Player> PlayersInDay(Day day)
        {
            if (day != null)
                foreach (Match match in day.matches)
                    foreach (Team team in match.teams)
                        foreach (Player player in team.players)
                            if (player != null)
                                yield return player;
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

    public class Counter<TKey>
    {
        readonly Dictionary<TKey, int> counts = new Dictionary<TKey, int>();

        public int this[TKey key]
        {
            get => counts.TryGetValue(key, out int value) ? value : 0;
            set => counts[key] = value;
        }
    }

    public class IndexableWrapper<T>
    {
        public readonly T Value;
        public readonly int Index;

        public IndexableWrapper(T Value, int Index)
        {
            this.Value = Value;
            this.Index = Index;
        }
    }
}
