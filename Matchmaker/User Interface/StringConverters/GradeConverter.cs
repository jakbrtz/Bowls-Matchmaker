using Matchmaker.Data;
using System;

namespace Matchmaker.UserInterface.StringConverters
{
    class GradeConverter : EnumStringConverter<Grade>
    {
        public override Grade StringToEnum(string value) => EnumStringConverter.ParseGrade(value);
        public override string EnumToString(Grade value) => EnumStringConverter.ToUserFriendlyString(value);
    }

    public static partial class EnumStringConverter
    {
        public static string ToUserFriendlyString(this Grade grade)
        {
            return grade switch
            {
                Grade.G1 => "G1",
                Grade.G2 => "G2",
                Grade.G3 => "G3",
                Grade.None => "",
                _ => throw new InvalidOperationException(),
            };
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
                case "none":
                case "":
                    grade = Grade.None;
                    return true;
            }
            return Enum.TryParse(value, out grade);
        }

        public static Grade ParseGrade(string value)
        {
            if (TryParseGrade(value, out Grade grade)) return grade;
            throw new ArgumentException("value was not a grade");
        }
    }
}
