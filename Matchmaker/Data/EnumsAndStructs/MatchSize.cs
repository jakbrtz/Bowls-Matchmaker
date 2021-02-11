namespace Matchmaker.Data
{
    public struct MatchSize
    {
        public readonly int team1Size;
        public readonly int team2Size;

        public MatchSize(int team1Size, int team2Size)
        {
            this.team1Size = team1Size;
            this.team2Size = team2Size;
        }

        public MatchSize(int teamSize) : this(teamSize, teamSize) {}

        public int TotalSize => team1Size + team2Size;

        public override int GetHashCode() => team1Size * Team.MaxSize + team2Size;

        public override string ToString() => team1Size + " vs " + team2Size;
    }
}
