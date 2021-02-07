using Matchmaker.Data;
using System;
using System.Collections.Generic;

namespace Matchmaker.DataHandling
{
    public static class DataCreation
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

        private static readonly Random rng = new Random();

        public static int UniqueRandomInt(IList<Player> existingPlayers)
        {
            HashSet<int> existing = new HashSet<int>();
            foreach (Player player in existingPlayers)
                existing.Add(player.ID);
            int chosen;
            do
            {
                chosen = rng.Next();
            }
            while (existing.Contains(chosen));
            return chosen;
        }
    }
}
