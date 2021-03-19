namespace Matchmaker.Data
{
    public abstract class Penalty
    {
        public HistoryOfPenalty historical = new HistoryOfPenalty();
        public double score = 0;

        public double Score() => score + historical.score;
    }

    public class HistoryOfPenalty
    {
        public double score;
        public int mostRecentGameIndex;
        public int numberOfOccurences;

        public void IncreaseScore(double increaseBy)
        {
            score += increaseBy;
            numberOfOccurences++;
        }

        public static HistoryOfPenalty Combine(HistoryOfPenalty historical1, HistoryOfPenalty historical2)
        {
            if (historical1 == null && historical2 == null)
            {
                return new HistoryOfPenalty();
            }
            if (historical1 == null)
            {
                return historical2;
            }
            if (historical2 == null)
            {
                return historical1;
            }
            return new HistoryOfPenalty
            {
                score = historical1.score + historical2.score,
                numberOfOccurences = historical1.numberOfOccurences + historical2.numberOfOccurences,
                mostRecentGameIndex = historical1.mostRecentGameIndex > historical2.mostRecentGameIndex ? historical1.mostRecentGameIndex : historical2.mostRecentGameIndex,
            };
        }

        public override string ToString()
        {
            if (numberOfOccurences == 0) return "no history";
            return $"{numberOfOccurences} occurences, most recently on {mostRecentGameIndex}";
        }
    }

    public class PairAlreadyPlayedInTeam : Penalty
    {
        public Player player1;
        public Player player2;

        public override string ToString() => $"PairAlreadyPlayedInTeam: {player1} & {player2}";
    }

    public class PairAlreadyPlayedAgainstEachOther : Penalty
    {
        public Player player1;
        public Player player2;

        public override string ToString() => $"PairAlreadyPlayedAgainstEachOther: {player1} & {player2}";
    }

    public class IncorrectPosition : Penalty
    {
        public Player player;
        public Position wantedPosition;
        public Position givenPosition;
        public bool usedSecondary;
        public Grade grade;

        public override string ToString() => $"IncorrectPosition: {player} ({grade}) wanted {wantedPosition} but got {givenPosition}. Secondary = {usedSecondary}";
    }

    public class WrongTeamSize : Penalty
    {
        public Player player;
        public TeamSize wantedSize;
        public TeamSize givenSize;

        public override string ToString() => $"WrongTeamSize: {player} wanted {wantedSize} but got {givenSize}";
    }

    public class UnbalancedPlayers : Penalty
    {
        public Player player1;
        public Player player2;
        public EffectiveGrade grade1;
        public EffectiveGrade grade2;

        public override string ToString() => $"UnbalancedPlayers: {player1} vs {player2} = {grade1} vs {grade2}";
    }

    public class UnbalancedTeams : Penalty
    {
        public Match match;
        public EffectiveGrade[] team1Grades;
        public EffectiveGrade[] team2Grades;

        public override string ToString()
        {
            string result = $"UnbalancedTeams: ";
            for (int position = 0; position < Team.MaxSize; position++)
                if (match.Team1.PositionShouldBeFilled((Position)position))
                    result += team1Grades[position] + " ";
            result += "vs ";
            for (int position = 0; position < Team.MaxSize; position++)
                if (match.Team2.PositionShouldBeFilled((Position)position))
                    result += team2Grades[position] + " ";
            return result;
        }
    }
}
