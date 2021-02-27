using System.Diagnostics;

namespace Matchmaker.Data
{
    public struct EffectiveGrade
    {
        public Grade grade;
        public bool positionIsPrimary;
        public bool positionIsSecondary;

        public EffectiveGrade(Player player, Position possiblePosition)
        {
            positionIsPrimary = player.PositionPrimary == possiblePosition;
            positionIsSecondary = player.PositionSecondary == possiblePosition;

            if (positionIsPrimary)
                grade = player.GradePrimary;
            else if (positionIsSecondary)
                grade = player.GradeSecondary;
            else if (player.GradeSecondary > player.GradePrimary && player.PositionSecondary != Position.None)
                grade = player.GradeSecondary;
            else
                grade = player.GradePrimary;

            Debug.Assert(grade != Grade.None);
        }

        public int Score()
        {
            int score = (int)grade;
            if (!positionIsPrimary && !positionIsSecondary)
                score++;
            return score;
        }

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
