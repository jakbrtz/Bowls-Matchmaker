using Matchmaker.Data;

namespace Matchmaker.Algorithms.Structures
{
    public class Swap
    {
        public readonly Match match1;
        private readonly Team team1;
        private readonly int position1;
        public readonly Match match2;
        private readonly Team team2;
        private readonly int position2;

        public Swap(int playerIndex1, int playerIndex2, Day day)
        {
            GetIndiciesForPlayerIndex(playerIndex1, out int matchIndex1, out int teamIndex1, out position1);
            match1 = day.matches[matchIndex1];
            team1 = match1.teams[teamIndex1];
            GetIndiciesForPlayerIndex(playerIndex2, out int matchIndex2, out int teamIndex2, out position2);
            match2 = day.matches[matchIndex2];
            team2 = match2.teams[teamIndex2];
        }

        public bool BothPlayersAreNotNull()
        {
            return Player1 != null && Player2 != null;
        }

        public void DoSwap()
        {
            Player tmp = Player1;
            Player1 = Player2;
            Player2 = tmp;
        }

        public Player Player1
        {
            get => team1.players[position1];
            set => team1.players[position1] = value;
        }

        public Player Player2
        {
            get => team2.players[position2];
            set => team2.players[position2] = value;
        }

        public static int CreatePlayerIndex(int matchIndex, int teamIndex, int position)
        {
            return matchIndex * Match.MaxPlayers + teamIndex * Team.MaxSize + position;
        }

        public static void GetIndiciesForPlayerIndex(int playerIndex, out int matchIndex, out int teamIndex, out int position)
        {
            matchIndex = playerIndex / Match.MaxPlayers;
            teamIndex = playerIndex % Match.MaxPlayers / Team.MaxSize;
            position = playerIndex % Team.MaxSize;
        }
    }
}
