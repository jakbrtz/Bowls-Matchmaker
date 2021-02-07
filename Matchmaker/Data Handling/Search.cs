using Matchmaker.Data;
using System;

namespace Matchmaker.DataHandling
{
    public static class Search
    {
        static string SimplifySearch(string old) => old.ToLower().Replace("'", "").Replace("-", "").Replace("'", "");

        public static bool Filter(Player player, string search)
        {
            if (string.IsNullOrEmpty(search)) return true;
            return SimplifySearch(player.Name).IndexOf(SimplifySearch(search), StringComparison.OrdinalIgnoreCase) != -1 || player.TagNumber?.ToString().Contains(search) == true;
        }

        public static int RelevanceToSearch(Player player, string search)
        {
            if (search == player.TagNumber) return int.MinValue;

            string playerName = SimplifySearch(player.Name);
            search = SimplifySearch(search);

            const int bignumber = 1 << 10; // there's no way the user will use more than this many words
            if (playerName == search) return int.MinValue;
            int earliestMatchingWord = -1;
            int numberOfMatchingWords = 0;
            foreach (string searchWord in search.Split(' '))
            {
                var names = playerName.Split(' ');
                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i].StartsWith(searchWord, StringComparison.OrdinalIgnoreCase))
                    {
                        numberOfMatchingWords++;
                        if (i < earliestMatchingWord)
                        {
                            earliestMatchingWord = i;
                        }
                        break;
                    }
                }
            }
            if (numberOfMatchingWords == 0) return int.MaxValue;
            return (bignumber - numberOfMatchingWords) * bignumber + earliestMatchingWord;
        }
    }
}
