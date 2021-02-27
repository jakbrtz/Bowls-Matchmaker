namespace Matchmaker.Data
{
    public struct PositionAndGrade
    {
        public Position position;
        public Grade grade;

        public override string ToString() => position + " " + grade;

        public bool HasPreference => position != Position.None;
    }
}
