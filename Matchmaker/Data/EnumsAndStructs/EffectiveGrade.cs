namespace Matchmaker.Data
{
    public struct EffectiveGrade
    {
        public Grade grade;
        public bool positionIsPrimary;
        public bool positionIsSecondary;

        public int Score()
        {
            int score = (int)grade;
            if (!positionIsPrimary && !positionIsSecondary)
                score++;
            return score;
        }

        public static int MaxScore = Enums.Grades.Length;

        public override string ToString()
        {
            string result = "";
            result += grade;
            if (positionIsSecondary)
                result += " (secondary)";
            else if (!positionIsPrimary)
                result += " (moved)";
            return result;
        }
    }
}
