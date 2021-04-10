using Matchmaker.Algorithms.Structures;
using Matchmaker.Collections;
using Matchmaker.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Matchmaker.Algorithms
{
    public class DayGenerator : IAlgorithmWithProgress
    {
        const int defaultAttempts = 5;

        private readonly DayGeneratorParameters parameters;
        private readonly int attempts;
        private readonly DayImprover[] improvers;

        public DayGenerator(DayGeneratorParameters parameters)
        {
            this.parameters = parameters;
            this.attempts = parameters.existingDay == null ? defaultAttempts : 1;
            this.improvers = new DayImprover[attempts];
        }

        public Day Generate()
        {
            var penalties = new CachedPenalties(parameters);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Parallel.For(0, attempts, TryGenerate);

            void TryGenerate(int i)
            {
                Day day = RandomDay();
                improvers[i] = new DayImprover(day, penalties);
                improvers[i].Improve();
            }

            sw.Stop();
            Debug.WriteLine($"Did {attempts} runs in {sw.ElapsedMilliseconds}ms");

            Day best = null;
            double bestScore = double.MaxValue;
            foreach (DayImprover improver in improvers)
            {
                if (improver.BestScore < bestScore)
                {
                    bestScore = improver.BestScore;
                    best = improver.day;
                }
            }

            if (parameters.existingDay == null)
                best.matches.Shuffle();

            return best;
        }

        Day RandomDay()
        {
            Day day = new Day();

            // Make a clone of data from parameters
            Counter<MatchSize> numMatchSizesClone = new Counter<MatchSize>(parameters.numMatchSizes);
            List<Player> playersClone;

            // If we have an existing day, try to copy matches across
            if (parameters.existingDay != null)
            {
                HashSet<Player> playersAsSet = new HashSet<Player>(parameters.players);
                foreach (Match match in parameters.existingDay.matches)
                {
                    // Make sure we need a match with this size
                    MatchSize matchSize = match.GetMatchSize();
                    if (numMatchSizesClone[matchSize] > 0)
                    {
                        // Check that all players from that match are still needed in this match
                        bool allPlayersInExisting = true;
                        foreach (Team team in match.teams)
                            foreach (Player player in team.players)
                                if (player != null)
                                    if (!playersAsSet.Contains(player))
                                        allPlayersInExisting = false;
                        if (allPlayersInExisting)
                        {
                            // Create a new match
                            Match copiedMatch = new Match(matchSize, match.isFixed);
                            copiedMatch.rink = match.rink;
                            day.matches.Add(copiedMatch);
                            // Copy the players from the existing match
                            for (int teamIndex = 0; teamIndex < 2; teamIndex++)
                            {
                                for (int position = 0; position < Team.MaxSize; position++)
                                {
                                    Player player = match.teams[teamIndex].players[position];
                                    copiedMatch.teams[teamIndex].players[position] = player;
                                    playersAsSet.Remove(player);
                                }
                            }
                            // Record that we've used a match of this size already
                            numMatchSizesClone[matchSize]--;
                        }
                    }
                }

                playersClone = new List<Player>(playersAsSet);
            }
            else
            {
                playersClone = new List<Player>(parameters.players);
            }

            // Create blank matches
            List<Match> matchesToAdd = new List<Match>();
            foreach (var matchSizeAndCount in numMatchSizesClone)
                for (int i = 0; i < matchSizeAndCount.Value; i++)
                    matchesToAdd.Add(new Match(matchSizeAndCount.Key, false));

            // The players could be placed randomly, but the improver algorithm takes a lot less time if players are placed in the correct position

            // Randomly rearrange the list of players
            playersClone.Shuffle();

            // Assign each player to their primary position
            List<Player>[] playersPerPosition = new List<Player>[Team.MaxSize];
            for (int position = 0; position < Team.MaxSize; position++)
                playersPerPosition[position] = new List<Player>();
            foreach (Player player in playersClone)
                playersPerPosition[(int)player.PositionPrimary].Add(player);

            // Look at how many players are requested for each position
            int[] requestedPlayers = new int[Team.MaxSize];
            foreach (Match match in matchesToAdd)
                foreach (Team team in match.teams)
                    for (int position = 0; position < Team.MaxSize; position++)
                        if (team.PositionShouldBeFilled((Position)position))
                            requestedPlayers[position]++;

            for (int position = 0; position < Team.MaxSize; position++)
            {
                // If earlier positions require more players, give players from this group
                for (int earlierPosition = 0; earlierPosition < position; earlierPosition++)
                    while (playersPerPosition[earlierPosition].Count < requestedPlayers[earlierPosition] && playersPerPosition[position].Count > 0)
                        MovePlayer(earlierPosition);
                // If this group has too many players, give to the next group
                while (playersPerPosition[position].Count > requestedPlayers[position])
                    MovePlayer(position + 1);

                void MovePlayer(int targetPosition)
                {
                    var list = playersPerPosition[position];

                    // Pick a player to get moved
                    int chosen = -1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (IsBetter())
                        {
                            chosen = i;
                        }

                        bool IsBetter()
                        {
                            // If we haven't chosen anything yet then this must be better
                            if (chosen == -1) return true;
                            // If one player's primary position is closer to the target then they are a good choice
                            // This can only happen if the target is later than the current position
                            if (list[i].PositionPrimary != list[chosen].PositionPrimary)
                                return list[i].PositionPrimary > list[chosen].PositionPrimary;
                            // If the player's secondary position is the target then they are a good choice
                            bool chosenIsSecondary = list[chosen].PositionSecondary == (Position)targetPosition;
                            bool currentIsSecondary = list[i].PositionSecondary == (Position)targetPosition;
                            if (chosenIsSecondary && !currentIsSecondary) return false;
                            if (!chosenIsSecondary && currentIsSecondary) return true;
                            // Pick the player with the worse grade (unless the target is Lead, in that case pick the better grade)
                            return (Position)targetPosition == Position.Lead
                                ? list[i].GradePrimary > list[chosen].GradePrimary
                                : list[i].GradePrimary > list[chosen].GradePrimary;
                        }
                    }

                    // Move the player
                    playersPerPosition[targetPosition].Add(list[chosen]);
                    list.RemoveAt(chosen);
                }
            }

            // Insert the players into the list of matches to add
            int[] index = new int[Team.MaxSize];
            foreach (Match match in matchesToAdd)
                foreach (Team team in match.teams)
                    for (int position = 0; position < Team.MaxSize; position++)
                        if (team.PositionShouldBeFilled((Position)position))
                            team.players[position] = playersPerPosition[position][index[position]++];

            // Add the matches to the day
            foreach (Match match in matchesToAdd)
                day.matches.Add(match);

            return day;
        }

        public void GetProgress(out double progress, out double score)
        {
            score = double.NaN;
            progress = 0;
            for (int i = 0; i < attempts; i++)
            {
                if (improvers[i] != null)
                {
                    improvers[i].GetProgress(out double improverProgress, out double improverScore);
                    progress += improverProgress;
                    if (double.IsNaN(score) || improverScore < score)
                        score = improverScore;
                }
            }
            progress /= attempts;
        }

        public override string ToString()
        {
            GetProgress(out double progress, out double score);
            return $"DayGenerator: {progress * 100: 0.##}% score = {score: 0.##}";
        }
    }
}
