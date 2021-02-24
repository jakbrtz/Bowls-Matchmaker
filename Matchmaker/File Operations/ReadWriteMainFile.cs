using Matchmaker.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dict = System.Collections.Generic.Dictionary<string, object>;

namespace Matchmaker.FileOperations
{
    static class ReadWriteMainFile
    {
        static ReadWriteMainFile()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new StringEnumConverter());
                settings.Converters.Add(new IntConverter());
                return settings;
            };
        }

        class IntConverter : JsonConverter
        {
            public override bool CanWrite => false;
            public override bool CanConvert(Type objectType) => objectType == typeof(int) || objectType == typeof(long) || objectType == typeof(object);
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => (reader.TokenType == JsonToken.Integer) ? Convert.ToInt32(reader.Value) : serializer.Deserialize(reader); 
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
        }

        public static void Output(string filename, IList<Player> players, IList<Day> history, Weights weights)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            using StreamWriter streamWriter = new StreamWriter(filename);

            streamWriter.Write(JsonConvert.SerializeObject(new Dict() {
                { "players", players.Select(WritePlayer) },
                { "history", history.Select(WriteDay) },
                { "weights", WriteWeights(weights) },
            }));

            Dict WritePlayer(Player player) => new Dict {
                { "Name", player.Name },
                { "ID", player.ID },
                { "PositionPrimary", player.PositionPrimary },
                { "PositionSecondary", player.PositionSecondary },
                { "GradePrimary", player.GradePrimary },
                { "GradeSecondary", player.GradeSecondary },
                { "PreferredTeamSizes", player.PreferredTeamSizes },
                { "TagNumber", player.TagNumber },
            };

            Dict WriteDay(Day day) => new Dict {
                { "date", day.date },
                { "matches", day.matches.Select(WriteMatch) },
            };

            Dict WriteMatch(Match match) => new Dict {
                { "Rink", match.rink },
                { "isFixed", match.isFixed },
                { "Team1", TeamAsString(match.Team1) },
                { "Team2", TeamAsString(match.Team2) },
                { "penalties", match.penalties.Select(WritePenalty) }
            };

            IEnumerable<T> relevantToTeam<T>(Team team, IList<T> list)
            {
                for (int position = 0; position < Team.MaxSize; position++)
                {
                    if (team.PositionShouldBeFilled((Position)position))
                    {
                        yield return list[position];
                    }
                }
            }

            string TeamAsString(Team team) => string.Join(",", relevantToTeam(team, team.players).Select(ConvertPlayer));

            string ConvertPlayer(Player player) => player?.ID.ToString() ?? "_";

            Dict WritePenalty(Penalty penalty) => penalty switch
            {
                PairAlreadyPlayedInTeam p => WritePenaltyPAIRALREADYPLAYEDINTEAM(p),
                PairAlreadyPlayedAgainstEachOther p => WritePenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(p),
                IncorrectPosition p => WritePenaltyINCORRECTPOSITION(p),
                WrongTeamSize p => WritePenaltyWRONGTEAMSIZE(p),
                UnbalancedPlayers p => WritePenaltyUNBALANCEDPLAYERS(p),
                UnbalancedTeams p => WritePenaltyUNBALANCEDTEAMS(p),
                _ => throw new ArgumentException("Unknown penalty")
            };

            Dict WritePenaltyPAIRALREADYPLAYEDINTEAM(PairAlreadyPlayedInTeam penalty) => new Dict {
                { "type", "PAIRALREADYPLAYEDINTEAM" },
                { "historical.mostRecentGameIndex", penalty.historical.mostRecentGameIndex},
                { "historical.numberOfOccurences", penalty.historical.numberOfOccurences},
                { "historical.score", penalty.historical.score},
                { "player1", ConvertPlayer(penalty.player1)},
                { "player2", ConvertPlayer(penalty.player2)},
                { "score", penalty.score},
            };

            Dict WritePenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(PairAlreadyPlayedAgainstEachOther penalty) => new Dict {
                { "type", "PAIRALREADYPLAYEDAGAINSTEACHOTHER" },
                { "historical.mostRecentGameIndex", penalty.historical.mostRecentGameIndex},
                { "historical.numberOfOccurences", penalty.historical.numberOfOccurences},
                { "historical.score", penalty.historical.score},
                { "player1", ConvertPlayer(penalty.player1)},
                { "player2", ConvertPlayer(penalty.player2)},
                { "score", penalty.score},
            };

            Dict WritePenaltyINCORRECTPOSITION(IncorrectPosition penalty) => new Dict {
                { "type", "INCORRECTPOSITION" },
                { "historical.mostRecentGameIndex", penalty.historical.mostRecentGameIndex},
                { "historical.numberOfOccurences", penalty.historical.numberOfOccurences},
                { "historical.score", penalty.historical.score},
                { "player", ConvertPlayer(penalty.player)},
                { "givenPosition", penalty.givenPosition},
                { "wantedPosition", penalty.wantedPosition},
                { "usedSecondary", penalty.usedSecondary},
                { "grade", penalty.grade},
            };

            Dict WritePenaltyWRONGTEAMSIZE(WrongTeamSize penalty) => new Dict {
                { "type", "WRONGTEAMSIZE" },
                { "historical.mostRecentGameIndex", penalty.historical.mostRecentGameIndex},
                { "historical.numberOfOccurences", penalty.historical.numberOfOccurences},
                { "historical.score", penalty.historical.score},
                { "player", ConvertPlayer(penalty.player)},
                { "score", penalty.score},
                { "givenSize", penalty.givenSize},
                { "wantedSize", penalty.wantedSize},
            };

            Dict WritePenaltyUNBALANCEDPLAYERS(UnbalancedPlayers penalty) => new Dict {
                { "type", "UNBALANCEDPLAYERS" },
                { "historical.mostRecentGameIndex", penalty.historical.mostRecentGameIndex},
                { "historical.numberOfOccurences", penalty.historical.numberOfOccurences},
                { "historical.score", penalty.historical.score},
                { "player1", ConvertPlayer(penalty.player1)},
                { "player2", ConvertPlayer(penalty.player2)},
                { "grade1", WriteEffectiveGrade(penalty.grade1)},
                { "grade2", WriteEffectiveGrade(penalty.grade2)},
                { "score", penalty.score},
            };

            Dict WritePenaltyUNBALANCEDTEAMS(UnbalancedTeams penalty) => new Dict {
                { "type", "UNBALANCEDTEAMS" },
                { "score", penalty.score},
                { "grade1", relevantToTeam(penalty.match.Team1, penalty.team1Grades).Select(WriteEffectiveGrade)},
                { "grade2", relevantToTeam(penalty.match.Team2, penalty.team2Grades).Select(WriteEffectiveGrade)},
            };

            Dict WriteEffectiveGrade(EffectiveGrade effectiveGrade) => new Dict {
                { "grade", effectiveGrade.grade },
                { "positionIsPrimary", effectiveGrade.positionIsPrimary },
                { "positionIsSecondary", effectiveGrade.positionIsSecondary },
            };

            Dict WriteWeights(Weights weights) => new Dict {
                { "IncorrectPosition", WriteWeight(weights.IncorrectPosition) },
                { "IncorrectTeamSize", WriteWeight(weights.IncorrectTeamSize) },
                { "PairPlayedTogetherAgainstEachOther", WriteWeight(weights.PairPlayedTogetherAgainstEachOther) },
                { "PairPlayedTogetherInTeam", WriteWeight(weights.PairPlayedTogetherInTeam) },
                { "SecondaryPosition", WriteWeight(weights.SecondaryPosition) },
                { "UnbalancedPlayers", WriteWeight(weights.UnbalancedPlayers) },
                { "UnbalancedTeams", WriteWeight(weights.UnbalancedTeams) },
                { "GoodSkipsGetSkip", WriteWeight(weights.GoodSkipsGetSkip) },
                { "GoodLeadsMoveUp", WriteWeight(weights.GoodLeadsMoveUp) },
            };

            Dict WriteWeight(Weight weight) => new Dict {
                { "Score", weight.Score },
                { "Multiplier", weight.Multiplier },
            };
        }

        public static void Input(string filename, IList<Player> players, IList<Day> history, Weights weights)
        {
            if (!File.Exists(filename)) return;
            using StreamReader sr = new StreamReader(filename);
            string str = sr.ReadToEnd();
            Dict dict = JsonConvert.DeserializeObject<Dict>(str);

            if (TryRead(dict, "players", out List<Dict> playersList))
                ReadPlayers(playersList);
            if (TryRead(dict, "history", out List<Dict> daysList))
                ReadDays(daysList);
            if (TryRead(dict, "weights", out Dict weightsDict))
                ReadWeights(weightsDict);

            bool TryRead<T>(Dict d, string key, out T obj, Func<string, T> converter = null, Func<Dict, T> dictReader = null)
            {
                if (d.TryGetValue(key, out object o))
                {
                    if (o is T t)
                    {
                        obj = t;
                        return true;
                    }
                    if (converter != null && o is string s)
                    {
                        obj = converter(s);
                        return true;
                    }
                    if (dictReader != null && o is JObject jo)
                    {
                        obj = dictReader(jo.ToObject<Dict>());
                        return true;
                    }
                    if (o is JToken j)
                    {
                        obj = j.ToObject<T>();
                        return true;
                    }
                    if (o is null)
                    {
                        obj = default;
                        return true;
                    }
                }
                // todo: alert that the file hasn't been read correctly
                obj = default;
                return false;
            }

            void Read<T>(Dict dictionary, string key, ref T field, Func<string, T> converter = null, Func<Dict, T> dictReader = null)
            {
                if (TryRead(dictionary, key, out T obj, converter, dictReader)) field = obj;
            }

            Player ConvertPlayer(string data)
            {
                if (int.TryParse(data, out int id))
                    foreach (Player player in players)
                        if (player.ID == id)
                            return player;
                if (data == "_") return null;
                throw new ArgumentException();
            }

            TEnum ConvertEnum<TEnum>(string data) where TEnum : struct
            {
                if (Enum.TryParse(data, out TEnum value))
                    return value;
                throw new ArgumentException();
            }

            void LoadIntoBiggerArray<TSource, TTarget>(ICollection<TSource> source, Func<TSource, TTarget> converter, TTarget[] target)
            {
                int size = source.Count;
                Position position = 0;
                foreach (TSource item in source)
                {
                    while (!Match.PositionShouldBeFilled(position, size)) position++;
                    target[(int)position++] = converter(item);
                }
            }

            void ReadPlayers(List<Dict> playersList)
            {
                foreach (Dict dict in playersList)
                {
                    ReadPlayer(dict);
                }
            }

            void ReadPlayer(Dict dict)
            {
                Player player = new Player();
                Read(dict, "Name", ref player.Name);
                Read(dict, "TagNumber", ref player.TagNumber);
                Read(dict, "ID", ref player.ID);
                Read(dict, "PositionPrimary", ref player.PositionPrimary, ConvertEnum<Position>);
                Read(dict, "PositionSecondary", ref player.PositionSecondary, ConvertEnum<Position>);
                Read(dict, "GradePrimary", ref player.GradePrimary, ConvertEnum<Grade>);
                Read(dict, "GradeSecondary", ref player.GradeSecondary, ConvertEnum<Grade>);
                Read(dict, "PreferredTeamSizes", ref player.PreferredTeamSizes, ConvertEnum<TeamSize>);
                players.Add(player);
            }

            void ReadDays(List<Dict> daysList)
            {
                foreach (Dict dict in daysList)
                {
                    ReadDay(dict);
                }
            }

            void ReadDay(Dict dict)
            {
                Day day = new Day();
                Read(dict, "date", ref day.date);
                if (TryRead(dict, "matches", out List<Dict> matches))
                    ReadMatches(matches, day);
                history.Add(day);
            }

            void ReadMatches(List<Dict> playersList, Day day)
            {
                foreach (Dict dict in playersList)
                {
                    ReadMatch(dict, day);
                }
            }

            void ReadMatch(Dict dict, Day day)
            {
                void ReadTeamStringIntoTeam(Team team, string str)
                {
                    string[] playerIDs = str.Split(',');
                    team.size = playerIDs.Length;
                    LoadIntoBiggerArray(playerIDs, ConvertPlayer, team.players);
                }

                Match match = new Match();
                Read(dict, "Rink", ref match.rink);
                Read(dict, "isFixed", ref match.isFixed);
                if (TryRead(dict, "Team1", out string team1String))
                    ReadTeamStringIntoTeam(match.Team1, team1String);
                if (TryRead(dict, "Team2", out string team2String))
                    ReadTeamStringIntoTeam(match.Team2, team2String);
                if (TryRead(dict, "penalties", out List<Dict> penalties))
                    ReadPenalties(penalties, match);
                day.matches.Add(match);
            }

            void ReadPenalties(List<Dict> penalties, Match match)
            {
                foreach (Dict penalty in penalties)
                {
                    ReadPenalty(penalty, match);
                }
            }

            void ReadPenalty(Dict penalty, Match match)
            {
                if (TryRead(penalty, "type", out string typeString))
                {
                    switch (typeString)
                    {
                        case "PAIRALREADYPLAYEDINTEAM:":
                            ReadPenaltyPAIRALREADYPLAYEDINTEAM(penalty, match);
                            break;
                        case "PAIRALREADYPLAYEDAGAINSTEACHOTHER:":
                            ReadPenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(penalty, match);
                            break;
                        case "INCORRECTPOSITION:":
                            ReadPenaltyINCORRECTPOSITION(penalty, match);
                            break;
                        case "WRONGTEAMSIZE:":
                            ReadPenaltyWRONGTEAMSIZE(penalty, match);
                            break;
                        case "UNBALANCEDPLAYERS:":
                            ReadPenaltyUNBALANCEDPLAYERS(penalty, match);
                            break;
                        case "UNBALANCEDTEAMS:":
                            ReadPenaltyUNBALANCEDTEAMS(penalty, match);
                            break;
                    }
                }
            }

            void ReadPenaltyPAIRALREADYPLAYEDINTEAM(Dict dict, Match match)
            {
                PairAlreadyPlayedInTeam penalty = new PairAlreadyPlayedInTeam();
                Read(dict, "historical.mostRecentGameIndex", ref penalty.historical.mostRecentGameIndex);
                Read(dict, "historical.numberOfOccurences", ref penalty.historical.numberOfOccurences);
                Read(dict, "historical.score", ref penalty.historical.score);
                Read(dict, "player1", ref penalty.player1, ConvertPlayer);
                Read(dict, "player2", ref penalty.player2, ConvertPlayer);
                Read(dict, "score", ref penalty.score);
                match.penalties.Add(penalty);
            }

            void ReadPenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(Dict dict, Match match)
            {
                PairAlreadyPlayedAgainstEachOther penalty = new PairAlreadyPlayedAgainstEachOther(); 
                Read(dict, "historical.mostRecentGameIndex", ref penalty.historical.mostRecentGameIndex);
                Read(dict, "historical.numberOfOccurences", ref penalty.historical.numberOfOccurences);
                Read(dict, "historical.score", ref penalty.historical.score);
                Read(dict, "player1", ref penalty.player1, ConvertPlayer);
                Read(dict, "player2", ref penalty.player2, ConvertPlayer);
                Read(dict, "score", ref penalty.score);
                match.penalties.Add(penalty);
            }

            void ReadPenaltyINCORRECTPOSITION(Dict dict, Match match)
            {
                IncorrectPosition penalty = new IncorrectPosition();
                Read(dict, "historical.mostRecentGameIndex", ref penalty.historical.mostRecentGameIndex);
                Read(dict, "historical.numberOfOccurences", ref penalty.historical.numberOfOccurences);
                Read(dict, "historical.score", ref penalty.historical.score);
                Read(dict, "player", ref penalty.player, ConvertPlayer);
                Read(dict, "score", ref penalty.score);
                Read(dict, "givenPosition", ref penalty.givenPosition, ConvertEnum<Position>);
                Read(dict, "wantedPosition", ref penalty.wantedPosition, ConvertEnum<Position>);
                Read(dict, "usedSecondary", ref penalty.usedSecondary);
                Read(dict, "grade", ref penalty.grade);
                match.penalties.Add(penalty);
            }

            void ReadPenaltyWRONGTEAMSIZE(Dict dict, Match match)
            {
                WrongTeamSize penalty = new WrongTeamSize();
                Read(dict, "historical.mostRecentGameIndex", ref penalty.historical.mostRecentGameIndex);
                Read(dict, "historical.numberOfOccurences", ref penalty.historical.numberOfOccurences);
                Read(dict, "historical.score", ref penalty.historical.score);
                Read(dict, "player", ref penalty.player, ConvertPlayer);
                Read(dict, "score", ref penalty.score);
                Read(dict, "givenSize", ref penalty.givenSize, ConvertEnum<TeamSize>);
                Read(dict, "wantedSize", ref penalty.wantedSize, ConvertEnum<TeamSize>);
                match.penalties.Add(penalty);
            }

            void ReadPenaltyUNBALANCEDPLAYERS(Dict dict, Match match)
            {
                UnbalancedPlayers penalty = new UnbalancedPlayers();
                Read(dict, "historical.mostRecentGameIndex", ref penalty.historical.mostRecentGameIndex);
                Read(dict, "historical.numberOfOccurences", ref penalty.historical.numberOfOccurences);
                Read(dict, "historical.score", ref penalty.historical.score);
                Read(dict, "player1", ref penalty.player1, ConvertPlayer);
                Read(dict, "player2", ref penalty.player2, ConvertPlayer);
                Read(dict, "grade1", ref penalty.grade1, dictReader: ReadEffectiveGrade);
                Read(dict, "grade2", ref penalty.grade2, dictReader: ReadEffectiveGrade);
                Read(dict, "score", ref penalty.score);
                match.penalties.Add(penalty);
            }

            void ReadPenaltyUNBALANCEDTEAMS(Dict dict, Match match)
            {
                UnbalancedTeams penalty = new UnbalancedTeams();
                Read(dict, "score", ref penalty.score);
                if (TryRead(dict, "team1Grades", out List<Dict> team1Grades))
                    penalty.team1Grades = ReadEffectiveGrades(team1Grades);
                if (TryRead(dict, "team2Grades", out List<Dict> team2Grades))
                    penalty.team2Grades = ReadEffectiveGrades(team2Grades);
                penalty.match = match;
                match.penalties.Add(penalty);
            }

            EffectiveGrade[] ReadEffectiveGrades(List<Dict> listDict)
            {
                EffectiveGrade[] effectiveGrades = new EffectiveGrade[Team.MaxSize];
                LoadIntoBiggerArray(listDict, ReadEffectiveGrade, effectiveGrades);
                return effectiveGrades;
            }

            EffectiveGrade ReadEffectiveGrade(Dict dict)
            {
                EffectiveGrade effectiveGrade = new EffectiveGrade();
                Read(dict, "grade", ref effectiveGrade.grade, ConvertEnum<Grade>);
                Read(dict, "positionIsPrimary", ref effectiveGrade.positionIsPrimary);
                Read(dict, "positionIsSecondary", ref effectiveGrade.positionIsSecondary);
                return effectiveGrade;
            }

            void ReadWeights(Dict dict)
            {
                Read(dict, "IncorrectPosition", ref weights.IncorrectPosition, dictReader: ReadWeight);
                Read(dict, "IncorrectTeamSize", ref weights.IncorrectTeamSize, dictReader: ReadWeight);
                Read(dict, "PairPlayedTogetherAgainstEachOther", ref weights.PairPlayedTogetherAgainstEachOther, dictReader: ReadWeight);
                Read(dict, "PairPlayedTogetherInTeam", ref weights.PairPlayedTogetherInTeam, dictReader: ReadWeight);
                Read(dict, "SecondaryPosition", ref weights.SecondaryPosition, dictReader: ReadWeight);
                Read(dict, "UnbalancedPlayers", ref weights.UnbalancedPlayers, dictReader: ReadWeight);
                Read(dict, "UnbalancedTeams", ref weights.UnbalancedTeams, dictReader: ReadWeight);
                Read(dict, "GoodLeadsMoveUp", ref weights.GoodLeadsMoveUp, dictReader: ReadWeight);
                Read(dict, "GoodSkipsGetSkip", ref weights.GoodSkipsGetSkip, dictReader: ReadWeight);
            }

            Weight ReadWeight(Dict dict)
            {
                Weight weight = new Weight();
                Read(dict, "Score", ref weight.Score);
                Read(dict, "Multiplier", ref weight.Multiplier);
                return weight;
            }
        }
    }
}
