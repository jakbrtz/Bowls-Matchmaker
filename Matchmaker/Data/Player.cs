﻿using System.Diagnostics;

namespace Matchmaker.Data
{
    public class Player
    {
        public int ID;
        public string TagNumber = "";
        public string Name = "";
        public Position PositionPrimary = Position.Lead;
        public Position PositionSecondary = Position.None;
        public Grade GradePrimary = Grade.G2;
        public Grade GradeSecondary = Grade.G2;
        public TeamSize PreferredTeamSizes = TeamSize.Triples;

        public bool Visitor => string.IsNullOrEmpty(TagNumber);

        public override string ToString() => Name + (string.IsNullOrEmpty(TagNumber) ? " (visitor)" : $" ({TagNumber})");

        public override int GetHashCode() => ID;

        public EffectiveGrade EffectiveGrade(Position possiblePosition)
        {
            bool positionIsPrimary = PositionPrimary == possiblePosition;
            bool positionIsSecondary = PositionSecondary == possiblePosition;

            Grade grade;
            if (positionIsPrimary)
                grade = GradePrimary;
            else if (positionIsSecondary)
                grade = GradeSecondary;
            else if (GradeSecondary > GradePrimary && PositionSecondary != Position.None)
                grade = GradeSecondary;
            else
                grade = GradePrimary;

            Debug.Assert(grade != Grade.None);

            return new EffectiveGrade
            {
                grade = grade,
                positionIsPrimary = positionIsPrimary,
                positionIsSecondary = positionIsSecondary,
            };
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
}
