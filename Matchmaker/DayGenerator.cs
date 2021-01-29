using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Matchmaker
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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var penalties = new CachedPenalties(parameters);

            sw.Stop();
            Console.WriteLine("CachedPenalties: " + sw.ElapsedMilliseconds);
            sw.Restart();

            Parallel.For(0, attempts, TryGenerate);

            void TryGenerate(int i)
            {
                Day day = RandomDay();
                improvers[i] = new DayImprover(day, penalties);
                improvers[i].Improve();
            }

            sw.Stop();
            Console.WriteLine("Parralel.For: " + sw.ElapsedMilliseconds);

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
                for (int teamSizeIndex = 0; teamSizeIndex < parameters.numTeamSizes.Length; teamSizeIndex++)
                    for (int i = 0; i < parameters.numTeamSizes[teamSizeIndex]; i++)
                        day.matches.Add(new Match(Team.MinSize + teamSizeIndex, false));
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
                    for (int position = 0; position < Team.MaxSize; position++)
                        if (match.Team1.PositionShouldBeFilled((Position)position))
                            requestedPlayers[position] += 2;

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

        public Swap DoOneImprovement()
        {
            Swap bestSwap = null;

            for (int p1 = 0; p1 < day.matches.Count * Match.MaxPlayers; p1++)
            {
                for (int p2 = 0; p2 < p1; p2++)
                {
                    Swap swap = new Swap(p1, p2, day);
                    if (swap.Player1 != null && swap.Player2 != null && !swap.match1.isFixed && !swap.match2.isFixed)
                    {
                        DoSwapAndRecalculate(swap);
                        double score = day.Score();
                        DoSwapAndRecalculate(swap);
                        if (score < BestScore)
                        {
                            BestScore = score;
                            bestSwap = swap;
                        }
                    }
                }
            }
            if (bestSwap != null)
            {
                DoSwapAndRecalculate(bestSwap);
            }
            return bestSwap;
        }

        void DoSwapAndRecalculate(Swap swap)
        {
            swap.DoSwap();
            penalties.RecalculateScore(swap.match1);
            penalties.RecalculateScore(swap.match2);
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
                    numberOfPlayers += day.matches[matchIndex].Size * 2;
                progress = (double)improvementsMade / numberOfPlayers;
                if (progress > 1) progress = 1;
            }
            
        }

        public override string ToString() => $"DayImprover: {(finished ? "finished" : "in progress")}) score = {BestScore: 0.##} improvementsMade = {improvementsMade}";
    }

    public class CachedPenalties
    {
        private readonly Weights weights;

        private readonly Dictionary<PairOfPlayers, HistoryOfPenalty> repeatedTeamMates;
        private readonly Dictionary<PairOfPlayers, HistoryOfPenalty> repeatedEnemies;
        private readonly Dictionary<Player, HistoryOfPenalty> incorrectPositions;
        private readonly Dictionary<Player, HistoryOfPenalty> wrongTeamSizes;
        private readonly Dictionary<Player, HistoryOfPenalty> unbalancedPlayerDictionary;

        public CachedPenalties(DayGeneratorParameters parameters)
        {
            IList<Player> players = parameters.players;
            IList<Day> history = parameters.history;
            weights = parameters.weights;

            repeatedTeamMates = new Dictionary<PairOfPlayers, HistoryOfPenalty>();
            repeatedEnemies = new Dictionary<PairOfPlayers, HistoryOfPenalty>();
            incorrectPositions = new Dictionary<Player, HistoryOfPenalty>();
            wrongTeamSizes = new Dictionary<Player, HistoryOfPenalty>();
            unbalancedPlayerDictionary = new Dictionary<Player, HistoryOfPenalty>();

            Counter<Player> gamesPlayerHasPlayed = new Counter<Player>();
            HashSet<Player> playersHashSet = new HashSet<Player>(players);
            for (int dayIndex = history.Count - 1; dayIndex >= 0; dayIndex--)
            {
                Day previousDay = history[dayIndex];

                // Get all pairs of players who played on this day
                foreach (Match match in previousDay.matches)
                {
                    foreach (Team team in match.teams)
                    {
                        for (int position1 = 0; position1 < Team.MaxSize; position1++)
                        {
                            for (int position2 = position1 + 1; position2 < Team.MaxSize; position2++)
                            {
                                Player player1 = team.players[position1];
                                Player player2 = team.players[position2];
                                if (player1 != null && player2 != null && playersHashSet.Contains(player1) && playersHashSet.Contains(player2))
                                {
                                    PairOfPlayers pair = new PairOfPlayers(player1, player2);
                                    double score = weights.PairPlayedTogetherInTeam.Result(Math.Min(gamesPlayerHasPlayed[player1], gamesPlayerHasPlayed[player2]));
                                    if (!repeatedTeamMates.ContainsKey(pair))
                                    {
                                        repeatedTeamMates[pair] = new HistoryOfPenalty { mostRecentGameIndex = dayIndex, };
                                    }
                                    repeatedTeamMates[pair].IncreaseScore(score);
                                }
                            }
                        }
                    }
                }

                foreach (var match in previousDay.matches)
                {
                    for (int position = 0; position < Team.MaxSize; position++)
                    {
                        Player player1 = match.Team1.players[position];
                        Player player2 = match.Team2.players[position];
                        if (player1 != null && player2 != null && playersHashSet.Contains(player1) && playersHashSet.Contains(player2))
                        {
                            PairOfPlayers pair = new PairOfPlayers(player1, player2);
                            double score = weights.PairPlayedTogetherAgainstEachOther.Result(Math.Min(gamesPlayerHasPlayed[player1], gamesPlayerHasPlayed[player2]));
                            if (!repeatedEnemies.ContainsKey(pair))
                            {
                                repeatedEnemies[pair] = new HistoryOfPenalty { mostRecentGameIndex = dayIndex, };
                            }
                            repeatedEnemies[pair].IncreaseScore(score);
                        }
                    }
                }

                // Find penalities in this day
                foreach (Match match in previousDay.matches)
                {
                    foreach (Penalty penalty in match.penalties)
                    {
                        void RecordPenalty(Player player, Dictionary<Player, HistoryOfPenalty> dictionary, Weight weight)
                        {
                            if (playersHashSet.Contains(player))
                            {
                                if (!dictionary.ContainsKey(player))
                                {
                                    dictionary[player] = new HistoryOfPenalty { mostRecentGameIndex = dayIndex };
                                }
                                dictionary[player].IncreaseScore(weight.Result(gamesPlayerHasPlayed[player] + 1));
                            }
                        }

                        if (penalty is IncorrectPosition incorrectPosition)
                        {
                            RecordPenalty(incorrectPosition.player, incorrectPositions, incorrectPosition.usedSecondary ? weights.SecondaryPosition : weights.IncorrectPosition);
                        }
                        else if (penalty is WrongTeamSize wrongTeamSize)
                        {
                            RecordPenalty(wrongTeamSize.player, wrongTeamSizes, weights.IncorrectTeamSize);
                        }
                        else if (penalty is UnbalancedPlayers unbalancedPlayers)
                        {
                            RecordPenalty(unbalancedPlayers.player1, unbalancedPlayerDictionary, weights.UnbalancedPlayers);
                            RecordPenalty(unbalancedPlayers.player2, unbalancedPlayerDictionary, weights.UnbalancedPlayers);
                        }
                    }
                }

                // Count the players that played this
                foreach (Match match in previousDay.matches)
                    foreach (Team team in match.teams)
                        foreach (Player player in team.players)
                            if (player != null)
                                gamesPlayerHasPlayed[player]++;
            }
        }

        public double RecalculateScore(Day day)
        {
            double score = 0;
            foreach (Match match in day.matches)
                score += RecalculateScore(match);
            return score;
        }

        public double RecalculateScore(Match match)
        {
            double score = 0;
            match.penalties.Clear();

            void Add(Penalty penalty)
            {
                match.penalties.Add(penalty);
                score += penalty.Score();
            }

            foreach (var team in match.teams)
                for (int p1 = 0; p1 < Team.MaxSize; p1++)
                    for (int p2 = 0; p2 < p1; p2++)
                        if (IsPenalty(team.players[p1], team.players[p2], out PairAlreadyPlayedInTeam penalty))
                            Add(penalty);
            foreach (var team in match.teams)
                for (int position = 0; position < Team.MaxSize; position++)
                    if (IsPenalty(team.players[position], (Position)position, team.size, out IncorrectPosition penalty))
                        Add(penalty);
            foreach (var team in match.teams)
                for (int position = 0; position < Team.MaxSize; position++)
                    if (IsPenalty(team.players[position], (TeamSize)(1 << team.size), out WrongTeamSize penalty))
                        Add(penalty);
            for (int position = 0; position < Team.MaxSize; position++)
                if (IsPenalty(match.Team1.players[position], match.Team2.players[position], out PairAlreadyPlayedAgainstEachOther penalty))
                    Add(penalty);
            for (int position = 0; position < Team.MaxSize; position++)
                if (IsPenalty(match.Team1.players[position], match.Team2.players[position], (Position)position, out UnbalancedPlayers penalty))
                    Add(penalty);
            {
                if (IsPenalty(match, out UnbalancedTeams penalty))
                    Add(penalty);
            }
            return score;
        }

        public bool IsPenalty(Player player1, Player player2, out PairAlreadyPlayedInTeam penalty)
        {
            if (player1 != null && player2 != null && repeatedTeamMates.TryGetValue(new PairOfPlayers(player1, player2), out HistoryOfPenalty historical))
            {
                penalty = new PairAlreadyPlayedInTeam
                {
                    player1 = player1,
                    player2 = player2,
                    historical = historical,
                    score = weights.PairPlayedTogetherInTeam.Score,
                };
                return true;
            }
            penalty = null;
            return false;
        }

        public bool IsPenalty(Player player1, Player player2, out PairAlreadyPlayedAgainstEachOther penalty)
        {
            if (player1 != null && player2 != null && repeatedEnemies.TryGetValue(new PairOfPlayers(player1, player2), out HistoryOfPenalty historical))
            {
                penalty = new PairAlreadyPlayedAgainstEachOther
                {
                    player1 = player1,
                    player2 = player2,
                    historical = historical,
                    score = weights.PairPlayedTogetherInTeam.Score,
                };
                return true;
            }
            penalty = null;
            return false;
        }

        public bool IsPenalty(Player player, Position position, int size, out IncorrectPosition penalty)
        {
            if (player == null || player.PositionIsPrimary(position))
            {
                penalty = null;
                return false;
            }

            bool usedSecondary = player.PositionIsSecondary(position);
            EffectiveGrade effectiveGrade = player.EffectiveGrade(position);
            double score =
                (usedSecondary ? weights.SecondaryPosition.Score : weights.IncorrectPosition.Score) *
                (Tools.DifferenceBetweenPositions(player.PositionPrimary, position, size) * 2 - 1) +
                weights.BadPositionForGoodGrade.Score *
                (1 - (effectiveGrade.Score() + 1) / (EffectiveGrade.MaxScore + 1));

            if (!incorrectPositions.TryGetValue(player, out HistoryOfPenalty historical))
                historical = new HistoryOfPenalty(); // todo adjust history depending on severity

            penalty = new IncorrectPosition
            {
                player = player,
                givenPosition = position,
                wantedPosition = player.PositionPrimary,
                grade = effectiveGrade.grade,
                score = score,
                usedSecondary = usedSecondary,
                historical = historical,
            };

            return true;
        }

        public bool IsPenalty(Player player, TeamSize size, out WrongTeamSize penalty)
        {
            if (player == null || player.PreferredTeamSizes.HasFlag(size))
            {
                penalty = null;
                return false;
            }

            if (!wrongTeamSizes.TryGetValue(player, out HistoryOfPenalty historical))
                historical = new HistoryOfPenalty();

            penalty = new WrongTeamSize
            {
                player = player,
                givenSize = size,
                historical = historical,
                score = weights.IncorrectTeamSize.Score,
                wantedSize = player.PreferredTeamSizes,
            };

            return true;
        }

        public bool IsPenalty(Player player1, Player player2, Position position, out UnbalancedPlayers penalty)
        {
            if (player1 != null && player2 != null && player1.EffectiveGrade(position).Score() != player2.EffectiveGrade(position).Score())
            {
                unbalancedPlayerDictionary.TryGetValue(player1, out HistoryOfPenalty historical1);
                unbalancedPlayerDictionary.TryGetValue(player2, out HistoryOfPenalty historical2);
                HistoryOfPenalty historical = HistoryOfPenalty.Combine(historical1, historical2);

                penalty = new UnbalancedPlayers
                {
                    player1 = player1,
                    grade1 = player1.EffectiveGrade(position),
                    player2 = player2,
                    grade2 = player2.EffectiveGrade(position),
                    historical = historical,
                    score = weights.UnbalancedPlayers.Score,
                };
                return true;
            }
            penalty = null;
            return false;
        }

        public bool IsPenalty(Match match, out UnbalancedTeams penalty)
        {
            int difference = 0;
            for (int position = 0; position < Team.MaxSize; position++)
            {
                if (match.Team1.players[position] != null && match.Team2.players[position] != null)
                {
                    difference += match.Team1.players[position].EffectiveGrade((Position)position).Score();
                    difference -= match.Team2.players[position].EffectiveGrade((Position)position).Score();
                }
            }
            if (difference == 0)
            {
                penalty = null;
                return false;
            }
            if (difference < 0) difference = -difference;
            EffectiveGrade[] team1Grades = new EffectiveGrade[Team.MaxSize];
            EffectiveGrade[] team2Grades = new EffectiveGrade[Team.MaxSize];
            for (int position = 0; position < Team.MaxSize; position++)
            {
                if (match.Team1.PositionShouldBeFilled((Position)position))
                {
                    team1Grades[position] = match.Team1.players[position].EffectiveGrade((Position)position);
                    team2Grades[position] = match.Team2.players[position].EffectiveGrade((Position)position);
                }
            }
            penalty = new UnbalancedTeams
            {
                match = match,
                historical = new HistoryOfPenalty(),
                score = weights.UnbalancedTeams.Score * difference / match.Size,
                team1Grades = team1Grades,
                team2Grades = team2Grades,
            };
            return true;
        }
    }

    public class Swap
    {
        public readonly Match match1;
        private readonly Team team1;
        private readonly int position1;
        public readonly Match match2;
        private readonly Team team2;
        private readonly int position2;

        public Swap(int playerIndex1, int playerIndex2, Day day)
        {
            match1 = day.matches[playerIndex1 / Match.MaxPlayers];
            team1 = match1.teams[playerIndex1 % Match.MaxPlayers / Team.MaxSize];
            position1 = playerIndex1 % Team.MaxSize;
            match2 = day.matches[playerIndex2 / Match.MaxPlayers];
            team2 = match2.teams[playerIndex2 % Match.MaxPlayers / Team.MaxSize];
            position2 = playerIndex2 % Team.MaxSize;
        }

        public bool BothPlayersAreNotNull()
        {
            return Player1 != null && Player2 != null;
        }

        public void DoSwap()
        {
            Player tmp = Player1;
            Player1 = Player2;
            Player2 = tmp;
        }

        public Player Player1
        {
            get => team1.players[position1];
            set => team1.players[position1] = value;
        }

        public Player Player2
        {
            get => team2.players[position2];
            set => team2.players[position2] = value;
        }

        public static int CreatePlayerIndex(int matchIndex, int teamIndex, int position)
        {
            return matchIndex * Match.MaxPlayers + teamIndex * Team.MaxSize + position;
        }

        public static void GetIndiciesForPlayerIndex(int playerIndex, out int matchIndex, out int teamIndex, out int position)
        {
            matchIndex = playerIndex / Match.MaxPlayers;
            teamIndex = playerIndex % Match.MaxPlayers / Team.MaxSize;
            position = playerIndex % Team.MaxSize;
        }
    }

    public class DayGeneratorParameters
    {
        public IList<Player> players;
        public IList<Day> history;
        public Weights weights;
        public int[] numTeamSizes;
    }
}
