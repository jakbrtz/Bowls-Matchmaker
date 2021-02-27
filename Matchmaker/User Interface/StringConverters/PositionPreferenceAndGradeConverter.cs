using Matchmaker.Data;
using System;

namespace Matchmaker.UserInterface.StringConverters
{
    public static partial class EnumStringConverter
    {
        public static string ToUserFriendlyString(this PositionPreferenceAndGrade positionPreferenceAndGrade)
        {
            return positionPreferenceAndGrade.position.ToUserFriendlyString() + " " + positionPreferenceAndGrade.grade.ToUserFriendlyString();
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

        public static string Abbreviation(this PositionPreferenceAndGrade positionPreferenceAndGrade)
        {
            static string Abbreviate(Position pos) => pos switch
            {
                Position.Lead => "L",
                Position.Second => "2",
                Position.Third => "3",
                Position.Skip => "S",
                _ => "",
            };

            string result = Abbreviate(positionPreferenceAndGrade.position.primary);
            if (positionPreferenceAndGrade.position.secondary != Position.None)
                result += "/" + Abbreviate(positionPreferenceAndGrade.position.secondary);
            result += " " + positionPreferenceAndGrade.grade.ToUserFriendlyString();
            return result;
        }
    }
}
