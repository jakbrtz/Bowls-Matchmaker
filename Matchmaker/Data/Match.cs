using System.Collections.Generic;

namespace Matchmaker.Data
{
    public class Match
    {
        public const int MaxPlayers = 2 * Team.MaxSize;

        public Team[] teams = new Team[2];

        public List<Penalty> penalties = new List<Penalty>();

        public string rink;

        public bool isFixed;

        public bool dontModify;

        public Team Team1 => teams[0];
        public Team Team2 => teams[1];
        public int Size => Team1.size > Team2.size ? Team1.size : Team2.size;

        public Match()
        {
            teams[0] = new Team();
            teams[1] = new Team();
        }

        public Match(MatchSize size, bool isFixed, bool dontModify) : this()
        {
            SetTeamSize(size);
            this.isFixed = isFixed;
            this.dontModify = dontModify;
        }

        public void SetTeamSize(MatchSize size)
        {
            Team1.size = size.team1Size;
            Team2.size = size.team2Size;
        }

        public override string ToString() => Team1.ToString() + " vs " + Team2.ToString();

        private static readonly int[] _minTeamSizeForPosition = new int[Team.MaxSize] { 0, 2, 3, 1 };
        public static bool PositionShouldBeFilled(Position position, int teamSize)
        {
            return teamSize > _minTeamSizeForPosition[(int)position];
        }

        public bool PositionShouldBeFilled(Position position) => PositionShouldBeFilled(position, Size);

        public MatchSize GetMatchSize() => new MatchSize(Team1.size, Team2.size);
    }
}
