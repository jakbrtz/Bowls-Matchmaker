using System.Collections.Generic;

namespace Matchmaker
{
    public partial class Player
    {
        public int ID { get; set; }
        public string TagNumber { get; set; } = "";
        public string Name { get; set; } = "";        
        public Position PositionPrimary { get; set; } = Position.Lead;
        public Position PositionSecondary { get; set; } = Position.None;
        public Grade GradePrimary { get; set; } = Grade.G2;
        public Grade GradeSecondary { get; set; } = Grade.G2;
        public TeamSize PreferredTeamSizes { get; set; } = TeamSize.Triples;
        public bool Visitor => string.IsNullOrEmpty(TagNumber);

        public override string ToString() => Name + (string.IsNullOrEmpty(TagNumber) ? " (visitor)" : $" ({TagNumber})");

        public override int GetHashCode() => ID;

        public bool PositionIsPrimary(Position possiblePosition)
        {
            // Maybe one day I'll change this so Third gets counted as a Primary Position even when it's not specifically mentioned
            return possiblePosition == PositionPrimary;
        }

        public bool PositionIsSecondary(Position possiblePosition)
        {
            // Maybe one day I'll change this so Third gets counted as a Secondary Position even when it's not specifically mentioned
            return possiblePosition == PositionSecondary;
        }

        public EffectiveGrade EffectiveGrade(Position possiblePosition)
        {
            bool positionIsPrimary = PositionIsPrimary(possiblePosition);
            bool positionIsSecondary = PositionIsSecondary(possiblePosition);

            Grade grade;
            if (positionIsPrimary)
                grade = GradePrimary;
            else if (positionIsSecondary)
                grade = GradeSecondary;
            else if (GradeSecondary > GradePrimary && PositionSecondary != Position.None)
                grade = GradeSecondary;
            else
                grade = GradePrimary;

            return new EffectiveGrade
            {
                grade = grade,
                positionIsPrimary = positionIsPrimary,
                positionIsSecondary = positionIsSecondary,
            };
        }
    }

    public class Day
    {
        public string date;
        public List<Match> matches = new List<Match>();

        public override string ToString() => date;
    }

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

    public class Team
    {
        public const int MinSize = 2;
        public const int MaxSize = 4;

        public int size = MaxSize;
        public Player[] players = new Player[MaxSize];

        public override string ToString()
        {
            List<Player> playersInRelevantPosition = new List<Player>(MaxSize);
            for (int position = 0; position < MaxSize; position++)
                if (PositionShouldBeFilled((Position)position))
                    playersInRelevantPosition.Add(players[position]);
            return string.Join(" ", playersInRelevantPosition);
        }

        public bool PositionShouldBeFilled(Position position)
        {
            return position switch
            {
                Position.Lead   => size > 0,
                Position.Second => size > 2,
                Position.Third  => size > 3,
                Position.Skip   => size > 1,
                _               => throw new System.ArgumentException(),
            };
        }
    }
}
