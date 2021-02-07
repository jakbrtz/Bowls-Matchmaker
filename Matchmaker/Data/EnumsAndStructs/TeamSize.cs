using System;

namespace Matchmaker.Data
{
    public enum TeamSize
    {
        Pairs = 1 << 2,
        Triples = 1 << 3,
        Fours = 1 << 4,
        PairsOrTriples = Pairs | Triples,
        TriplesOrFours = Triples | Fours,
        Any = Pairs | Triples | Fours
    }

    public static partial class Enums
    {
        public static TeamSize[] TeamSizes = new TeamSize[] {
            TeamSize.Pairs,
            TeamSize.Triples,
            TeamSize.PairsOrTriples,
            TeamSize.Fours,
            TeamSize.TriplesOrFours,
            TeamSize.Any,
        };

        public static string ToUserFriendlyString(this TeamSize teamSize)
        {
            return teamSize switch
            {
                TeamSize.Pairs => "Pairs",
                TeamSize.Triples => "Triples",
                TeamSize.PairsOrTriples => "Pairs or Triples",
                TeamSize.Fours => "Fours",
                TeamSize.TriplesOrFours => "Not Pairs",
                TeamSize.Any => "Any",
                _ => teamSize.ToString(),
            };
        }

        public static bool TryParseTeamSize(string value, out TeamSize teamSize)
        {
            value = value.Trim().ToLower().Replace("?", "").Replace("end rink", "");

            if (string.IsNullOrEmpty(value))
            {
                teamSize = TeamSize.Any;
                return true;
            }

            string[] options = value.Split(new string[] { "or", "and", "&", "|" }, StringSplitOptions.None);

            if (options.Length > 1)
            {
                teamSize = 0;
                foreach (string option in options)
                {
                    if (TryParseTeamSize(option, out TeamSize subValue))
                    {
                        teamSize |= subValue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }

            if (value.StartsWith("not") && value.Length > 3)
            {
                if (TryParseTeamSize(value.Substring(3), out teamSize))
                {
                    teamSize = ~teamSize & TeamSize.Any;
                    return true;
                }
            }

            switch (value)
            {
                case "pair":
                case "pairs":
                case "p":
                case "two":
                case "twos":
                case "2":
                    teamSize = TeamSize.Pairs;
                    return true;
                case "triples":
                case "triple":
                case "trips":
                case "trip":
                case "triplets":
                case "triplet":
                case "threes":
                case "three":
                case "3":
                    teamSize = TeamSize.Triples;
                    return true;
                case "fours":
                case "four":
                case "4":
                case "f":
                    teamSize = TeamSize.Fours;
                    return true;
                case "anything":
                case "any":
                case "all":
                    teamSize = TeamSize.Any;
                    return true;
            }
            return Enum.TryParse(value, out teamSize);
        }

        public static TeamSize ParseTeamSize(string value)
        {
            if (TryParseTeamSize(value, out TeamSize teamSize)) return teamSize;
            throw new ArgumentException($"{value} was not a teamsize");
        }

        public static string NameOfTeamSize(int size)
        {
            return size switch
            {
                2 => "Pairs",
                3 => "Triples",
                4 => "Fours",
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
