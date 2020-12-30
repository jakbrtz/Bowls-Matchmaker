using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Matchmaker
{
    public partial class Player : IFormattable
    {
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) return ToString();
            return format
                .Replace("%name", string.IsNullOrEmpty(Name) ? TagNumber : Name)
                .Replace("%tag", string.IsNullOrEmpty(TagNumber) ? "(visitor)" : TagNumber)
                .Replace("%position", PositionPrimary.ToString())
                .Replace("%visitor", Visitor ? "Visitor" : "Member");
        }

        public PositionPreference PositionPreference
        {
            get
            {
                return new PositionPreference(PositionPrimary, PositionSecondary);
            }
            set
            {
                PositionPrimary = value.primary;
                PositionSecondary = value.secondary;
            }
        }

        public PositionAndGrade PreferencePrimary
        {
            get
            {
                return new PositionAndGrade
                {
                    position = PositionPrimary,
                    grade = GradePrimary,
                };
            }
            set
            {
                PositionPrimary = value.position;
                GradePrimary = value.grade;
            }
        }

        public PositionAndGrade PreferenceSecondary
        {
            get
            {
                return new PositionAndGrade
                {
                    position = PositionSecondary,
                    grade = GradeSecondary,
                };
            }
            set
            {
                PositionSecondary = value.position;
                GradeSecondary = value.grade;
            }
        }
    }

    public struct PositionAndGrade
    {
        public Position position;
        public Grade grade;

        public override string ToString() => position + " " + grade;

        public string Abbreviation()
        {
            return position switch
            {
                Position.Lead =>   "L",
                Position.Second => "S",
                Position.Third =>  "T",
                Position.Skip =>   "SK",
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

        public static List<PositionPreference> Values = new List<PositionPreference>() {
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
                Position.Lead =>   "L",
                Position.Second => "2",
                Position.Third =>  "3",
                Position.Skip =>   "S",
                _ => "",
            };

            string result = Abbreviate(position.primary);
            if (position.secondary != Position.None)
                result += "/" + Abbreviate(position.secondary);
            result += " " + grade.ToUserFriendlyString();
            return result;
        }
    }

    public struct EffectiveGrade
    {
        public Grade grade;
        public bool positionIsPrimary;
        public bool positionIsSecondary;

        public int Score()
        {
            int score = (int)grade;
            if (!positionIsPrimary && !positionIsSecondary)
                score++;
            return score;
        }

        public static int MaxScore = Enums.Grades.Count;

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

    class PositionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            foreach (var match in PositionPreference.Values)
                if (match.ToString() == value.ToString())
                    return match;
            throw new InvalidCastException("Unrecognised Position Preference");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }
    }
}
