using Matchmaker.Data;
using System;

namespace Matchmaker.UserInterface.StringConverters
{
    class PositionPreferenceConverter : EnumStringConverter<PositionPreference>
    {
        public override PositionPreference StringToEnum(string value) => EnumStringConverter.ParsePositionPreference(value);
    }

    public static partial class EnumStringConverter
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
