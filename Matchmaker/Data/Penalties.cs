using System.Collections.Generic;

namespace Matchmaker.Data
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

    public abstract class Penalty
    {
        public HistoryOfPenalty historical = new HistoryOfPenalty();
        public double score = 0;

        public abstract string Reason(IList<Day> history);

        public override string ToString() => Score() + ": " + Reason(null);

        public double Score() => score + historical.score;
    }

    public class HistoryOfPenalty
    {
        public double score;
        public int mostRecentGameIndex;
        public int numberOfOccurences;

        public string CiteOccurence(IList<Day> history)
        {
            if (numberOfOccurences == 0) return "";
            string result = "This has happened ";
            result += numberOfOccurences switch
            {
                1 => "once",
                2 => "twice",
                _ => numberOfOccurences + " times",
            };
            result += ", most recently on ";
            if (mostRecentGameIndex == -1)
                result += "[deleted]";
            else if (history == null)
                result += mostRecentGameIndex;
            else
                result += history[mostRecentGameIndex];
            return result;
        }

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

        public override string Reason(IList<Day> history)
        {
            string result = "";
            result += player1 + " & " + player2;
            result += " played with each other already. ";
            result += historical.CiteOccurence(history);
            return result;
        }
    }

    public class PairAlreadyPlayedAgainstEachOther : Penalty
    {
        public Player player1;
        public Player player2;

        public override string Reason(IList<Day> history)
        {
            string result = "";
            result += player1 + " & " + player2;
            result += " played against each other already. ";
            result += historical.CiteOccurence(history);
            return result;
        }
    }

    public class IncorrectPosition : Penalty
    {
        public Player player;
        public Position wantedPosition;
        public Position givenPosition;
        public bool usedSecondary;
        public Grade grade;

        public override string Reason(IList<Day> history)
        {
            string result = $"{player} ({grade.ToUserFriendlyString()}) wanted {wantedPosition.ToUserFriendlyString()} but got {givenPosition.ToUserFriendlyString()}";
            if (usedSecondary)
                result += " (second choice)";
            result += ". ";
            result += historical.CiteOccurence(history);
            return result;
        }
    }

    public class WrongTeamSize : Penalty
    {
        public Player player;
        public TeamSize wantedSize;
        public TeamSize givenSize;

        public override string Reason(IList<Day> history)
        {
            string result = $"{player} wanted to play {wantedSize.ToUserFriendlyString()} but got {givenSize.ToUserFriendlyString()}. ";
            result += historical.CiteOccurence(history);
            return result;
        }
    }

    public class UnbalancedPlayers : Penalty
    {
        public Player player1;
        public Player player2;
        public EffectiveGrade grade1;
        public EffectiveGrade grade2;

        public override string Reason(IList<Day> history)
        {
            string result = $"{player1} & {player2} are not an even match. ({grade1} vs {grade2}). ";
            result += historical.CiteOccurence(history);
            return result;
        }
    }

    public class UnbalancedTeams : Penalty
    {
        public Match match;
        public EffectiveGrade[] team1Grades;
        public EffectiveGrade[] team2Grades;

        public override string Reason(IList<Day> history)
        {
            string result = $"{match} is not an even match. ";
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
