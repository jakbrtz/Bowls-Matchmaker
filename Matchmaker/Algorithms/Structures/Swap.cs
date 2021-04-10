using Matchmaker.Data;

namespace Matchmaker.Algorithms.Structures
{
    public interface ISwap
    {
        public bool IsPossible();
        public bool IsAllowed();
        public void DoSwap();
        public void RecalculateScore(CachedPenalties penalties);
    }

    public class RegularSwap : ISwap
    {
        private readonly Match match1;
        private readonly Team team1;
        private readonly int position1;
        private readonly Match match2;
        private readonly Team team2;
        private readonly int position2;

        public RegularSwap(int playerIndex1, int playerIndex2, Day day)
        {
            GetIndiciesForPlayerIndex(playerIndex1, out int matchIndex1, out int teamIndex1, out position1);
            match1 = day.matches[matchIndex1];
            team1 = match1.teams[teamIndex1];
            GetIndiciesForPlayerIndex(playerIndex2, out int matchIndex2, out int teamIndex2, out position2);
            match2 = day.matches[matchIndex2];
            team2 = match2.teams[teamIndex2];
        }

        public bool IsPossible()
        {
            return Player1 != null && Player2 != null;
        }

        public bool IsAllowed()
        {
            return !match1.isFixed && !match2.isFixed && !match1.dontModify && !match2.dontModify;
        }

        public void DoSwap()
        {
            Player tmp = Player1;
            Player1 = Player2;
            Player2 = tmp;
        }

        public void RecalculateScore(CachedPenalties penalties)
        {
            penalties.RecalculateScore(match1);
            penalties.RecalculateScore(match2);
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

        public static void GetIndiciesForPlayerIndex(int playerIndex, out int matchIndex, out int teamIndex, out int position)
        {
            matchIndex = playerIndex / Match.MaxPlayers;
            teamIndex = playerIndex % Match.MaxPlayers / Team.MaxSize;
            position = playerIndex % Team.MaxSize;
        }

        public static int CreatePlayerIndex(int matchIndex, int teamIndex, int position)
        {
            return matchIndex * Match.MaxPlayers + teamIndex * Team.MaxSize + position;
        }
    }

    public class SimpleDoubleSwap : ISwap
    {
        private readonly Match match1;
        private readonly int position1;
        private readonly Match match2;
        private readonly int position2;

        public SimpleDoubleSwap(int playerIndex1, int playerIndex2, Day day)
        {
            GetIndiciesForPlayerIndex(playerIndex1, out int matchIndex1, out position1);
            match1 = day.matches[matchIndex1];
            GetIndiciesForPlayerIndex(playerIndex2, out int matchIndex2, out position2);
            match2 = day.matches[matchIndex2];
        }

        public bool IsPossible()
        {
            return Player1a != null && Player1b != null && Player2a != null && Player2b != null;
        }

        public bool IsAllowed()
        {
            return !match1.isFixed && !match2.isFixed && !match1.dontModify && !match2.dontModify;
        }

        public void DoSwap()
        {
            Player tmp;
            tmp = Player1a;
            Player1a = Player2a;
            Player2a = tmp;
            tmp = Player1b;
            Player1b = Player2b;
            Player2b = tmp;
        }

        public void RecalculateScore(CachedPenalties penalties)
        {
            penalties.RecalculateScore(match1);
            penalties.RecalculateScore(match2);
        }

        public Player Player1a
        {
            get => match1.Team1.players[position1];
            set => match1.Team1.players[position1] = value;
        }

        public Player Player2a
        {
            get => match2.Team1.players[position2];
            set => match2.Team1.players[position2] = value;
        }

        public Player Player1b
        {
            get => match1.Team2.players[position1];
            set => match1.Team2.players[position1] = value;
        }

        public Player Player2b
        {
            get => match2.Team2.players[position2];
            set => match2.Team2.players[position2] = value;
        }

        public static void GetIndiciesForPlayerIndex(int playerIndex, out int matchIndex, out int position)
        {
            matchIndex = playerIndex / Team.MaxSize;
            position = playerIndex % Team.MaxSize;
        }
    }
}
