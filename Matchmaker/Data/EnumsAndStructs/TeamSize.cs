namespace Matchmaker.Data
{
    public enum TeamSize
    {
        Pairs = 1 << 2,
        Triples = 1 << 3,
        Fours = 1 << 4,
        PairsOrTriples = Pairs | Triples,
        TriplesOrFours = Triples | Fours,
        Any = Pairs | Triples | Fours
    }

    public static partial class Enums
    {
        public static TeamSize[] TeamSizes = new TeamSize[] {
            TeamSize.Pairs,
            TeamSize.Triples,
            TeamSize.PairsOrTriples,
            TeamSize.Fours,
            TeamSize.TriplesOrFours,
            TeamSize.Any,
        };
    }
}
