﻿using System;

namespace Matchmaker.Data
{
    public enum Grade
    {
        G1,
        G2,
        G3,
        None = -1
    }

    public static partial class Enums
    {
        public static Grade[] Grades = new Grade[] {
            Grade.G1,
            Grade.G2,
            Grade.G3,
        };
        
        public static Grade[] GradesIncludingNone = new Grade[] {
            Grade.G1,
            Grade.G2,
            Grade.G3,
            Grade.None,
        };

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
