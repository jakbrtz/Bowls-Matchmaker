using Matchmaker.Data;
using System.Collections.Generic;

namespace Matchmaker.Algorithms.Structures
{
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
                                    double score = weights.PairPlayedTogetherInTeam.Result(Min(gamesPlayerHasPlayed[player1], gamesPlayerHasPlayed[player2]));
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
                            double score = weights.PairPlayedTogetherAgainstEachOther.Result(Min(gamesPlayerHasPlayed[player1], gamesPlayerHasPlayed[player2]));
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

        private static int Min(int a, int b) => a < b ? a : b;
    }

    public class Counter<TKey>
    {
        readonly Dictionary<TKey, int> counts = new Dictionary<TKey, int>();

        public int this[TKey key]
        {
            get => counts.TryGetValue(key, out int value) ? value : 0;
            set => counts[key] = value;
        }
    }
}
