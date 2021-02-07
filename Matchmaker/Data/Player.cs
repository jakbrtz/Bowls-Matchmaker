using System;

namespace Matchmaker.Data
{
    public partial class Player : IFormattable
    {
        // todo: are all these {get;set;} necessary?
        public int ID { get; set; }
        public string TagNumber { get; set; } = "";
        public string Name { get; set; } = "";
        public Position PositionPrimary { get; set; } = Position.Lead;
        public Position PositionSecondary { get; set; } = Position.None;
        public Grade GradePrimary { get; set; } = Grade.G2;
        public Grade GradeSecondary { get; set; } = Grade.G2;
        public TeamSize PreferredTeamSizes { get; set; } = TeamSize.Triples;
        public bool Visitor => string.IsNullOrEmpty(TagNumber);

        public override string ToString() => Name + (string.IsNullOrEmpty(TagNumber) ? " (visitor)" : $" ({TagNumber})");

        public override int GetHashCode() => ID; // todo: create a separate class with an extra idx field

        public bool PositionIsPrimary(Position possiblePosition)
        {
            // Maybe one day I'll change this so Third gets counted as a Primary Position even when it's not specifically mentioned
            return possiblePosition == PositionPrimary;
        }

        public bool PositionIsSecondary(Position possiblePosition)
        {
            // Maybe one day I'll change this so Third gets counted as a Secondary Position even when it's not specifically mentioned
            return possiblePosition == PositionSecondary;
        }

        public EffectiveGrade EffectiveGrade(Position possiblePosition)
        {
            bool positionIsPrimary = PositionIsPrimary(possiblePosition);
            bool positionIsSecondary = PositionIsSecondary(possiblePosition);

            Grade grade;
            if (positionIsPrimary)
                grade = GradePrimary;
            else if (positionIsSecondary)
                grade = GradeSecondary;
            else if (GradeSecondary > GradePrimary && PositionSecondary != Position.None)
                grade = GradeSecondary;
            else
                grade = GradePrimary;

            return new EffectiveGrade
            {
                grade = grade,
                positionIsPrimary = positionIsPrimary,
                positionIsSecondary = positionIsSecondary,
            };
        }

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
}
