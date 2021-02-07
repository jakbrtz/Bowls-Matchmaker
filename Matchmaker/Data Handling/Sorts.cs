using Matchmaker.Data;

namespace Matchmaker.DataHandling
{
    public static class Sorts
    {
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
    }
}
