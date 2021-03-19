using Matchmaker.Data;

namespace Matchmaker.Algorithms.Structures
{
    public struct PairOfPlayers
    {
        public Player player1;
        public Player player2;

        public PairOfPlayers(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }

        public override int GetHashCode()
        {
            return player1?.GetHashCode() ^ player2?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            return obj is PairOfPlayers other &&
                ((player1 == other.player1 && player2 == other.player2) ||
                 (player1 == other.player2 && player2 == other.player1));
        }

        public override string ToString() => player1 + " " + player2;
    }
}
