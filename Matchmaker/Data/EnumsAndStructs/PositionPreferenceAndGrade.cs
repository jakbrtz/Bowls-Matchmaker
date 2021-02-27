namespace Matchmaker.Data
{
    public struct PositionPreferenceAndGrade
    {
        public PositionPreference position;
        public Grade grade;

        public override string ToString()
        {
            return position.ToString() + " " + grade.ToString();
        }
    }
}
