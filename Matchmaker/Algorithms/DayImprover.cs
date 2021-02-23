using Matchmaker.Algorithms.Structures;
using Matchmaker.Data;

namespace Matchmaker.Algorithms
{
    public class DayImprover : IAlgorithmWithProgress
    {
        public readonly Day day;
        public readonly CachedPenalties penalties;
        public double BestScore { get; private set; }

        int improvementsMade = 0;
        bool finished = false;

        public DayImprover(Day day, CachedPenalties penalties)
        {
            this.day = day;
            this.penalties = penalties;

            BestScore = penalties.RecalculateScore(day);
        }

        public DayImprover(Day day, DayGeneratorParameters parameters) : this(day, new CachedPenalties(parameters)) { }

        public void Improve()
        {
            finished = false;
            for (improvementsMade = 0; improvementsMade < 1000; improvementsMade++)
            {
                var bestSwap = DoOneImprovement();
                if (bestSwap == null) break;
            }
            finished = true;
        }

        public ISwap DoOneImprovement()
        {
            ISwap bestSwap = null;

            void CheckIfSwapIsImprovement(ISwap swap)
            {
                if (!swap.IsValid()) return;
                if (swap.InvolvesFixedMatches()) return;
                DoSwapAndRecalculate(swap);
                double score = Score(day);
                DoSwapAndRecalculate(swap);
                if (score < BestScore)
                {
                    BestScore = score;
                    bestSwap = swap;
                }
            }

            // Simple swaps
            for (int p1 = 0; p1 < day.matches.Count * Match.MaxPlayers; p1++)
            {
                for (int p2 = 0; p2 < p1; p2++)
                {
                    CheckIfSwapIsImprovement(new RegularSwap(p1, p2, day));
                }
            }

            // Paired swaps
            if (bestSwap == null)
            {
                for (int p1 = 0; p1 < day.matches.Count * Match.MaxPlayers; p1++)
                {
                    for (int p2 = 0; p2 < p1; p2++)
                    {
                        // todo: avoid checking the same swap twice:
                        // p1 % Match.MaxPlayers < Team.MaxPlayers
                        CheckIfSwapIsImprovement(new SimpleDoubleSwap(p1, p2, day));
                    }
                }
            }

            // Do swap and return
            if (bestSwap != null)
            {
                DoSwapAndRecalculate(bestSwap);
            }
            return bestSwap;
        }

        double Score(Day day)
        {
            double total = 0;
            for (int i = 0; i < day.matches.Count; i++)
                for (int j = 0; j < day.matches[i].penalties.Count; j++)
                    total += day.matches[i].penalties[j].Score();
            return total;
        }

        void DoSwapAndRecalculate(ISwap swap)
        {
            swap.DoSwap();
            swap.RecalculateScore(penalties);
        }

        public void GetProgress(out double progress, out double score)
        {
            score = BestScore;
            if (finished)
            {
                progress = 1;
            }
            else
            {
                int numberOfPlayers = 0;
                for (int matchIndex = 0; matchIndex < day.matches.Count; matchIndex++)
                    numberOfPlayers += day.matches[matchIndex].Team1.size + day.matches[matchIndex].Team2.size;
                progress = (double)improvementsMade / numberOfPlayers;
                if (progress > 1) progress = 1;
            }

        }

        public override string ToString() => $"DayImprover: {(finished ? "finished" : "in progress")}) score = {BestScore: 0.##} improvementsMade = {improvementsMade}";
    }
}
