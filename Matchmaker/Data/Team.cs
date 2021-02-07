namespace Matchmaker.Data
{
    public class Team
    {
        public const int MinSize = 2;
        public const int MaxSize = 4;

        public int size = MaxSize;
        public Player[] players = new Player[MaxSize];

        public override string ToString()
        {
            string result = "";
            for (int position = 0; position < MaxSize; position++)
                if (PositionShouldBeFilled((Position)position))
                    result += players[position] + " ";
            return result;
        }

        public bool PositionShouldBeFilled(Position position)
        {
            return position switch
            {
                Position.Lead => size > 0,
                Position.Second => size > 2,
                Position.Third => size > 3,
                Position.Skip => size > 1,
                _ => throw new System.ArgumentException(),
            };
        }
    }
}
