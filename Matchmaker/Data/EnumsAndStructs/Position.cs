using System;

namespace Matchmaker.Data
{
    public enum Position
    {
        Lead = 0,
        Second = 1,
        Third = 2,
        Skip = 3,
        None = -1
    }


    public static partial class Enums
    {
        public static Position[] Positions = new Position[] {
            Position.Lead,
            Position.Second,
            Position.Third,
            Position.Skip,
        };

        public static Position[] PositionsIncludingNone = new Position[] {
            Position.Lead,
            Position.Second,
            Position.Third,
            Position.Skip,
            Position.None,
        };

        public static string ToUserFriendlyString(this Position position)
        {
            return position switch
            {
                Position.Lead => "Lead",
                Position.Second => "Second",
                Position.Third => "Third",
                Position.Skip => "Skip",
                Position.None => "",
                _ => throw new InvalidOperationException(),
            };
        }

        public static string ToUserFriendlyStringPlural(this Position position)
        {
            return position switch
            {
                Position.Lead => "Leads",
                Position.Second => "Seconds",
                Position.Third => "Thirds",
                Position.Skip => "Skips",
                Position.None => "",
                _ => throw new InvalidOperationException(),
            };
        }

        public static bool TryParsePosition(string value, out Position position)
        {
            value = value.Trim().ToLower();

            switch (value)
            {
                case "lead":
                case "l":
                case "1":
                    position = Position.Lead;
                    return true;
                case "second":
                case "s":
                case "2nd":
                case "2":
                    position = Position.Second;
                    return true;
                case "third":
                case "t":
                case "3rd":
                case "3":
                    position = Position.Third;
                    return true;
                case "skip":
                case "sk":
                case "fourth":
                case "4":
                case "4th":
                    position = Position.Skip;
                    return true;
                case "":
                case "none":
                    position = Position.None;
                    return true;
            }
            return Enum.TryParse(value, out position);
        }

        public static Position ParsePosition(string value)
        {
            if (TryParsePosition(value, out Position position)) return position;
            throw new ArgumentException("value was not a position");
        }
    }
}
