using System;

namespace Matchmaker.Data
{
    public struct PositionPreferenceAndGrade
    {
        public PositionPreference position;
        public Grade grade;

        public override string ToString()
        {
            return position.ToUserFriendlyString() + " " + grade.ToUserFriendlyString();
        }

        public string Abbreviation()
        {
            static string Abbreviate(Position pos) => pos switch
            {
                Position.Lead => "L",
                Position.Second => "2",
                Position.Third => "3",
                Position.Skip => "S",
                _ => "",
            };

            string result = Abbreviate(position.primary);
            if (position.secondary != Position.None)
                result += "/" + Abbreviate(position.secondary);
            result += " " + grade.ToUserFriendlyString();
            return result;
        }
    }

    public static partial class Enums
    {
        public static string ToUserFriendlyString(this PositionPreferenceAndGrade positionPreferenceAndGrade)
        {
            return positionPreferenceAndGrade.ToString();
        }

        public static bool TryParsePositionPreferenceAndGrade(string value, out PositionPreferenceAndGrade positionPreferenceAndGrade)
        {
            positionPreferenceAndGrade = new PositionPreferenceAndGrade() { position = new PositionPreference(Position.None), grade = Grade.G1 };
            string[] options = value.Split(new string[] { "or", "and", "&", "|", "/", "\\", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (options.Length == 0)
            {
                return false;
            }
            if (!TryParsePosition(options[0], out positionPreferenceAndGrade.position.primary))
            {
                return false;
            }
            if (options.Length == 1)
            {
                return true;
            }
            if (TryParsePosition(options[1], out Position secondary))
            {
                positionPreferenceAndGrade.position.secondary = secondary;
                if (options.Length == 2)
                {
                    return true;
                }
                return TryParseGrade(options[2], out positionPreferenceAndGrade.grade);
            }
            else if (TryParseGrade(options[1], out positionPreferenceAndGrade.grade))
            {
                if (options.Length == 2)
                {
                    return true;
                }
                return TryParsePosition(options[2], out positionPreferenceAndGrade.position.secondary);
            }
            return false;
        }

        public static PositionPreferenceAndGrade ParsePositionPreferenceAndGrade(string value)
        {
            if (TryParsePositionPreferenceAndGrade(value, out PositionPreferenceAndGrade positionPreferenceAndGrade)) return positionPreferenceAndGrade;
            throw new ArgumentException("value was not a position preference");
        }
    }
}
