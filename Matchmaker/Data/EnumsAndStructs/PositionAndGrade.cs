using System;

namespace Matchmaker.Data
{
    public struct PositionAndGrade
    {
        public Position position;
        public Grade grade;

        public override string ToString() => position + " " + grade;

        public string Abbreviation()
        {
            return position switch
            {
                Position.Lead => "L",
                Position.Second => "S",
                Position.Third => "T",
                Position.Skip => "SK",
                _ => "",
            } + grade switch
            {
                Grade.G1 => "G1",
                Grade.G2 => "G2",
                Grade.G3 => "G3",
                _ => "",
            };
        }

        public bool HasPreference => position != Position.None;
    }

    public static partial class Enums
    {
        public static string ToUserFriendlyString(this PositionAndGrade positionAndGrade)
        {
            if (positionAndGrade.HasPreference)
                return positionAndGrade.ToString();
            return "";
        }

        public static bool TryParsePositionAndGrade(string value, out PositionAndGrade positionAndGrade)
        {
            for (int i = value.Length; i >= 0; i--)
            {
                string positionStr;
                string gradeStr;
                if (i == value.Length)
                {
                    positionStr = value;
                    gradeStr = "";
                }
                else if (i == 0)
                {
                    positionStr = "";
                    gradeStr = value;
                }
                else
                {
                    positionStr = value.Remove(i);
                    gradeStr = value.Substring(i);
                }
                if (TryParsePosition(positionStr, out Position position) && TryParseGrade(gradeStr, out Grade grade))
                {
                    positionAndGrade = new PositionAndGrade { position = position, grade = grade };
                    return true;
                }
            }
            {
                if (TryParsePosition(value, out Position position))
                {
                    positionAndGrade = new PositionAndGrade { position = position };
                    return true;
                }
            }
            positionAndGrade = new PositionAndGrade();
            return false;
        }

        public static PositionAndGrade ParsePositionAndGrade(string value)
        {
            if (TryParsePositionAndGrade(value, out PositionAndGrade positionAndGrade)) return positionAndGrade;
            throw new ArgumentException("value was not a position and grade");
        }
    }
}
