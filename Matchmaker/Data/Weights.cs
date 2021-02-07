namespace Matchmaker.Data
{
    public class Weights
    {
        public Weight PairPlayedTogetherInTeam = new Weight();
        public Weight PairPlayedTogetherAgainstEachOther = new Weight();
        public Weight IncorrectPosition = new Weight();
        public Weight SecondaryPosition = new Weight();
        public Weight BadPositionForGoodGrade = new Weight();
        public Weight IncorrectTeamSize = new Weight();
        public Weight UnbalancedPlayers = new Weight();
        public Weight UnbalancedTeams = new Weight();

        public Weights()
        {
            ResetToDefaults();
        }

        public void ResetToDefaults()
        {
            PairPlayedTogetherInTeam.Set(20, 0.75);
            PairPlayedTogetherAgainstEachOther.Set(20, 0.75);
            IncorrectPosition.Set(100, 0.75);
            SecondaryPosition.Set(10, 0.75);
            BadPositionForGoodGrade.Set(20, 0.75);
            IncorrectTeamSize.Set(100, 0.75);
            UnbalancedPlayers.Set(5, 0.75);
            UnbalancedTeams.Set(5, 0.75);
        }
    }

    public partial class Weight
    {
        public double Score;
        public double Multiplier;

        public void Set(double score, double multiplier)
        {
            Score = score;
            Multiplier = multiplier;
        }

        public double Result(int daysAgo)
        {
            double result = Score;
            for (int i = 0; i < daysAgo; i++)
                result *= Multiplier;
            return result;
        }
    }
}
