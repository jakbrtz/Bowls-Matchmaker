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
                    result += Player(position) + " ";
            return result;
        }

        public bool PositionShouldBeFilled(Position position) => Match.PositionShouldBeFilled(position, size);

        /// <summary>
        /// Find what player is playing at a position. This accounts for players playing in two positions. 
        /// Only call the function if you expect a player to be playing in this position
        /// </summary>
        public Player Player(Position position)
        {
            var result = players[(int)position];
            if (result != null || PositionShouldBeFilled(position)) return result;
            return Player(position - 1);
        }

        public Player Player(int position) => Player((Position)position);
    }
}
