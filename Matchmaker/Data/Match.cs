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
        public int Size => Team1.size;

        public Match()
        {
            teams[0] = new Team();
            teams[1] = new Team();
        }

        public Match(int size, bool isFixed) : this()
        {
            SetTeamSize(size);
            this.isFixed = isFixed;
        }

        public void SetTeamSize(int size)
        {
            teams[0].size = size;
            teams[1].size = size;
        }

        public override string ToString() => Team1.ToString() + " vs " + Team2.ToString();
    }
}
