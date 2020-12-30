using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker
{
    public enum Position { 
        Lead    = 0, 
        Second  = 1, 
        Third   = 2, 
        Skip    = 3, 
        None    = -1 
    }

    public enum Grade { 
        G1,
        G2, 
        G3 
    }

    public enum TeamSize { 
        Pairs           = 1 << 2, 
        Triples         = 1 << 3,
        Fours           = 1 << 4,
        PairsOrTriples  = Pairs | Triples, 
        TriplesOrFours  = Triples | Fours, 
        Any             = Pairs | Triples | Fours 
    }

    public static class Enums
    {
        public static ReadOnlyCollection<Position> Positions = new ReadOnlyCollection<Position>(new Position[] {
            Position.Lead,
            Position.Second,
            Position.Third,
            Position.Skip,
        });

        public static ReadOnlyCollection<Position> PositionsIncludingNone = new ReadOnlyCollection<Position>(new Position[] {
            Position.Lead,
            Position.Second,
            Position.Third,
            Position.Skip,
            Position.None,
        });

        public static ReadOnlyCollection<Grade> Grades = new ReadOnlyCollection<Grade>(new Grade[] {
            Grade.G1,
            Grade.G2,
            Grade.G3,
        });

        public static ReadOnlyCollection<TeamSize> TeamSizes = new ReadOnlyCollection<TeamSize>(new TeamSize[] {
            TeamSize.Pairs,
            TeamSize.Triples,
            TeamSize.PairsOrTriples,
            TeamSize.Fours,
            TeamSize.TriplesOrFours,
            TeamSize.Any,
        });
    }

    static class EnumParser
    {
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

        public static string ToUserFriendlyString(this Grade grade)
        {
            return grade.ToString();
        }

        public static bool TryParseGrade(string value, out Grade grade)
        {
            value = value.Trim().ToLower();
            switch (value)
            {
                case "g1":
                case "1":
                    grade = Grade.G1;
                    return true;
                case "g2":
                case "2":
                    grade = Grade.G2;
                    return true;
                case "g3":
                case "3":
                    grade = Grade.G3;
                    return true;
            }
            return Enum.TryParse(value, out grade);
        }

        public static Grade ParseGrade(string value)
        {
            if (TryParseGrade(value, out Grade grade)) return grade;
            throw new ArgumentException("value was not a position preference");
        }

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

        public static string ToUserFriendlyString(this TeamSize teamSize)
        {
            return teamSize switch
            {
                TeamSize.Pairs => "Pairs",
                TeamSize.Triples => "Triples",
                TeamSize.PairsOrTriples => "Pairs or Triples",
                TeamSize.Fours => "Fours",
                TeamSize.TriplesOrFours => "Triples or Fours",
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
                    teamSize = ~teamSize;
                    teamSize &= TeamSize.Any;
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
