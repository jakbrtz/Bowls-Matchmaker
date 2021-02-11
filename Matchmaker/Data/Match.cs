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

        public Team Team1 => teams[0];
        public Team Team2 => teams[1];
        public int Size => Team1.size > Team2.size ? Team1.size : Team2.size;

        public Match()
        {
            teams[0] = new Team();
            teams[1] = new Team();
        }

        public Match(MatchSize size, bool isFixed) : this()
        {
            SetTeamSize(size);
            this.isFixed = isFixed;
        }

        public void SetTeamSize(MatchSize size)
        {
            Team1.size = size.team1Size;
            Team2.size = size.team2Size;
        }

        public override string ToString() => Team1.ToString() + " vs " + Team2.ToString();

        public static bool PositionShouldBeFilled(Position position, int teamSize)
        {
            if (Team.MaxSize == 4)
            {
                return position switch
                {
                    Position.Lead => teamSize > 0,
                    Position.Second => teamSize > 2,
                    Position.Third => teamSize > 3,
                    Position.Skip => teamSize > 1,
                    _ => throw new System.ArgumentException(),
                };
            }
        }

        public bool PositionShouldBeFilled(Position position) => PositionShouldBeFilled(position, Size);
    }
}
