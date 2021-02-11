using Matchmaker.Algorithms.Structures;
using Matchmaker.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Matchmaker.Algorithms
{
    public class DayGenerator : IAlgorithmWithProgress
    {
        const int attempts = 5;

        private readonly DayGeneratorParameters parameters;
        public DayGenerator(DayGeneratorParameters parameters)
        {
            this.parameters = parameters;
        }

        readonly DayImprover[] improvers = new DayImprover[attempts];

        public Day Generate()
        {
            var penalties = new CachedPenalties(parameters);

            Parallel.For(0, attempts, TryGenerate);

            void TryGenerate(int i)
            {
                Day day = RandomDay();
                improvers[i] = new DayImprover(day, penalties);
                improvers[i].Improve();
            }

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

            best.matches.Shuffle();

            return best;
        }

        Day RandomDay()
        {
            Day day = new Day();
            SetUpMatchSizes();
            PlacePlayersAccordingToPosition();
            return day;

            void SetUpMatchSizes()
            {
                foreach (var matchSizeAndCount in parameters.numTeamSizes)
                    for (int i = 0; i < matchSizeAndCount.Value; i++)
                        day.matches.Add(new Match(matchSizeAndCount.Key, false));
            }

            void PlacePlayersAccordingToPosition()
            {
                // The players can be placed randomly, but the algorithm takes a lot less time if players are placed in the correct position

                // Randomly rearrange the list of players
                List<Player> playersClone = new List<Player>(parameters.players);
                playersClone.Shuffle();

                // Assign each player to their primary position
                List<Player>[] playersPerPosition = new List<Player>[Team.MaxSize];
                for (int position = 0; position < Team.MaxSize; position++)
                    playersPerPosition[position] = new List<Player>();
                foreach (Player player in playersClone)
                    playersPerPosition[(int)player.PositionPrimary].Add(player);

                // Look at how many players are requested for each position
                int[] requestedPlayers = new int[Team.MaxSize];
                foreach (Match match in day.matches)
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
                                bool chosenIsSecondary = list[chosen].PositionIsSecondary((Position)targetPosition);
                                bool currentIsSecondary = list[i].PositionIsSecondary((Position)targetPosition);
                                if (chosenIsSecondary && !currentIsSecondary) return false;
                                if (!chosenIsSecondary && currentIsSecondary) return true;
                                // Pick the player with the better grade
                                return list[i].GradePrimary < list[chosen].GradePrimary;
                            }
                        }

                        // Move the player
                        playersPerPosition[targetPosition].Add(list[chosen]);
                        list.RemoveAt(chosen);
                    }
                }

                // Insert the players into the day
                int[] index = new int[Team.MaxSize];
                foreach (Match match in day.matches)
                    foreach (Team team in match.teams)
                        for (int position = 0; position < Team.MaxSize; position++)
                            if (team.PositionShouldBeFilled((Position)position))
                                team.players[position] = playersPerPosition[position][index[position]++];
            }
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
