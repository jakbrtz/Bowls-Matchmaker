namespace Matchmaker.Data
{
    public enum Position
    {
        Lead = 0,
        Second = 1,
        Third = 2,
        Skip = 3,
        None = -1
    }

    public static partial class Enums
    {
        public static Position[] Positions = new Position[] {
            Position.Lead,
            Position.Second,
            Position.Third,
            Position.Skip,
        };

        public static Position[] PositionsIncludingNone = new Position[] {
            Position.Lead,
            Position.Second,
            Position.Third,
            Position.Skip,
            Position.None,
        };
    }
}
