using Matchmaker.Data;
using System;
using System.Collections.Generic;

namespace Matchmaker.Algorithms
{
    internal static class Tools
    {
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

        public static bool PositionCouldBeSecondary(Position position, Player player)
        {
            // This condition stops the program from compiling if I ever change MaxSize
            if (Team.MaxSize == 4)
            {
                if (player.PositionSecondary != Position.None) return false;
                if (player.PositionPrimary == Position.Lead) return position == Position.Second;
                if (player.PositionPrimary == Position.Second) return true;
                return position != Position.Lead;
            }
        }

        public static void GetIndiciesForPlayerIndex(int playerIndex, out int matchIndex, out int teamIndex, out int position)
        {
            matchIndex = playerIndex / Match.MaxPlayers;
            teamIndex = playerIndex % Match.MaxPlayers / Team.MaxSize;
            position = playerIndex % Team.MaxSize;
        }

        public static int CreatePlayerIndex(int matchIndex, int teamIndex, int position)
        {
            return matchIndex * Match.MaxPlayers + teamIndex * Team.MaxSize + position;
        }
    }

    public interface IAlgorithmWithProgress
    {
        public void GetProgress(out double progress, out double score);
    }
}
