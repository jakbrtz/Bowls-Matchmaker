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

        public static readonly MatchSize Pairs = new MatchSize(2);
        public static readonly MatchSize Triples = new MatchSize(3);
        public static readonly MatchSize Fours = new MatchSize(4);
        public static readonly MatchSize TripVsPair = new MatchSize(3, 2);
        public static readonly MatchSize FourVsTrip = new MatchSize(4, 3);
    }
}
