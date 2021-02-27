namespace Matchmaker.Data
{
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
    }
}
