using Matchmaker.Data;
using System;

namespace Matchmaker.UserInterface.StringConverters
{
    public static partial class EnumStringConverter
    {
        public static string ToUserFriendlyString(this PositionAndGrade positionAndGrade)
        {
            if (positionAndGrade.HasPreference)
                return positionAndGrade.position.ToUserFriendlyString() + " " + positionAndGrade.grade.ToUserFriendlyString();
            return "";
        }

        public static bool TryParsePositionAndGrade(string value, out PositionAndGrade positionAndGrade)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                positionAndGrade = new PositionAndGrade { position = Position.None, grade = Grade.None };
                return true;
            }
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
                    positionAndGrade = new PositionAndGrade { position = position, grade = Grade.G2 };
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

        public static string Abbreviation(this PositionAndGrade positionAndGrade)
        {
            return positionAndGrade.position switch
            {
                Position.Lead => "L",
                Position.Second => "S",
                Position.Third => "T",
                Position.Skip => "SK",
                _ => "",
            } + positionAndGrade.grade switch
            {
                Grade.G1 => "G1",
                Grade.G2 => "G2",
                Grade.G3 => "G3",
                _ => "",
            };
        }
    }
}
