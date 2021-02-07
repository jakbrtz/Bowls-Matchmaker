using System;

namespace Matchmaker.Data
{
    public struct PositionPreference
    {
        public Position primary;
        public Position secondary;

        public PositionPreference(Position primary, Position secondary = Position.None)
        {
            this.primary = primary;
            this.secondary = secondary;
        }

        public override string ToString()
        {
            string result = primary.ToString();
            if (secondary != Position.None)
                result += " / " + secondary.ToString();
            return result;
        }

        public static PositionPreference[] Values = new PositionPreference[] {
            new PositionPreference(Position.Lead, Position.None),
            new PositionPreference(Position.Lead, Position.Second),
            new PositionPreference(Position.Second, Position.Lead),
            new PositionPreference(Position.Second, Position.None),
            new PositionPreference(Position.Second, Position.Skip),
            new PositionPreference(Position.Third, Position.None),
            new PositionPreference(Position.Skip, Position.Second),
            new PositionPreference(Position.Skip, Position.None),
        };
    }

    public static partial class Enums
    {
        public static string ToUserFriendlyString(this PositionPreference positionPreference)
        {
            return positionPreference.ToString();
        }

        public static bool TryParsePositionPreference(string value, out PositionPreference positionPreference)
        {
            positionPreference = new PositionPreference(Position.None);
            string[] options = value.Split(new string[] { "or", "and", "&", "|", "/", "\\" }, StringSplitOptions.None);
            if (options.Length == 0)
            {
                return false;
            }
            if (options.Length == 1)
            {
                if (TryParsePosition(options[0], out Position primary))
                {
                    positionPreference = new PositionPreference(primary);
                    return true;
                }
                return false;
            }
            {
                if (TryParsePosition(options[0], out Position primary) && TryParsePosition(options[1], out Position secondary))
                {
                    positionPreference = new PositionPreference(primary, secondary);
                    return true;
                }
                return false;
            }
        }

        public static PositionPreference ParsePositionPreference(string value)
        {
            if (TryParsePositionPreference(value, out PositionPreference positionPreference)) return positionPreference;
            throw new ArgumentException("value was not a position preference");
        }
    }
}
