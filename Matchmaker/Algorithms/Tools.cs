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
    }

    public interface IAlgorithmWithProgress
    {
        public void GetProgress(out double progress, out double score);
    }
}
