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
            match1 = day.matches[playerIndex1 / Match.MaxPlayers];
            team1 = match1.teams[playerIndex1 % Match.MaxPlayers / Team.MaxSize];
            position1 = playerIndex1 % Team.MaxSize;
            match2 = day.matches[playerIndex2 / Match.MaxPlayers];
            team2 = match2.teams[playerIndex2 % Match.MaxPlayers / Team.MaxSize];
            position2 = playerIndex2 % Team.MaxSize;
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
