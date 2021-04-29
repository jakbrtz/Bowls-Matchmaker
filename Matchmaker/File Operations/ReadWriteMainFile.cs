using Matchmaker.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                { "dontModify", match.dontModify },
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
                    try
                    {
                        obj = Read(o, converter, dictReader);
                        return true;
                    }
                    catch (InvalidDataException e)
                    {
                        Debug.Fail(e.Message);
                    }
                }
                else
                {
                    Debug.Fail($"Could not find key \"{key}\"");
                }
                obj = default;
                return false;
            }

            T Read<T>(object o, Func<string, T> converter = null, Func<Dict, T> dictReader = null)
            {
                if (o is T t)
                {
                    return t;
                }
                if (converter != null && o is string s)
                {
                    return converter(s);
                }
                if (dictReader != null && o is JObject jo)
                {
                    return dictReader(jo.ToObject<Dict>());
                }
                if (o is JToken j)
                {
                    return j.ToObject<T>();
                }
                if (o is null)
                {
                    return default;
                }
                throw new InvalidDataException($"Could not convert {o} to {typeof(T)}");
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

                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "Name": player.Name = Read<string>(kvp.Value); break;
                        case "TagNumber": player.TagNumber = Read<string>(kvp.Value); break;
                        case "ID": player.ID = Read<int>(kvp.Value); break;
                        case "PositionPrimary": player.PositionPrimary = Read(kvp.Value, converter: ConvertEnum<Position>); break;
                        case "PositionSecondary": player.PositionSecondary = Read(kvp.Value, converter: ConvertEnum<Position>); break;
                        case "GradePrimary": player.GradePrimary = Read(kvp.Value, converter: ConvertEnum<Grade>); break;
                        case "GradeSecondary": player.GradeSecondary = Read(kvp.Value, converter: ConvertEnum<Grade>); break;
                        case "PreferredTeamSizes": player.PreferredTeamSizes = Read(kvp.Value, converter: ConvertEnum<TeamSize>); break;
                    }
                }

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
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "date": day.date = Read<string>(kvp.Value); break;
                        case "matches": ReadMatches(Read<List<Dict>>(kvp.Value), day); break;
                    }
                }
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
                void ReadTeamStringIntoTeam(Team team, object obj)
                {
                    string str = Read<string>(obj);
                    string[] playerIDs = str.Split(',');
                    team.size = playerIDs.Length;
                    LoadIntoBiggerArray(playerIDs, ConvertPlayer, team.players);
                }

                Match match = new Match();
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "Rink": match.rink = Read<string>(kvp.Value); break;
                        case "isFixed": match.isFixed = Read<bool>(kvp.Value); break;
                        case "dontModify": match.dontModify = Read<bool>(kvp.Value); break;
                        case "Team1": ReadTeamStringIntoTeam(match.Team1, kvp.Value); break;
                        case "Team2": ReadTeamStringIntoTeam(match.Team2, kvp.Value); break;
                        case "penalties": ReadPenalties(Read<List<Dict>>(kvp.Value), match); break;
                    }
                }
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
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "historical.mostRecentGameIndex": penalty.historical.mostRecentGameIndex = Read<int>(kvp.Value); break;
                        case "historical.numberOfOccurences": penalty.historical.numberOfOccurences = Read<int>(kvp.Value); break;
                        case "historical.score": penalty.historical.score = Read<double>(kvp.Value); break;
                        case "player1": penalty.player1 = Read(kvp.Value, ConvertPlayer); break;
                        case "player2": penalty.player2 = Read(kvp.Value, ConvertPlayer); break;
                        case "score": penalty.score = Read<double>(kvp.Value); break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void ReadPenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(Dict dict, Match match)
            {
                PairAlreadyPlayedAgainstEachOther penalty = new PairAlreadyPlayedAgainstEachOther();
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "historical.mostRecentGameIndex": penalty.historical.mostRecentGameIndex = Read<int>(kvp.Value); break;
                        case "historical.numberOfOccurences": penalty.historical.numberOfOccurences = Read<int>(kvp.Value); break;
                        case "historical.score": penalty.historical.score = Read<double>(kvp.Value); break;
                        case "player1": penalty.player1 = Read(kvp.Value, ConvertPlayer); break;
                        case "player2": penalty.player2 = Read(kvp.Value, ConvertPlayer); break;
                        case "score": penalty.score = Read<double>(kvp.Value); break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void ReadPenaltyINCORRECTPOSITION(Dict dict, Match match)
            {
                IncorrectPosition penalty = new IncorrectPosition(); 
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "historical.mostRecentGameIndex": penalty.historical.mostRecentGameIndex = Read<int>(kvp.Value); break;
                        case "historical.numberOfOccurences": penalty.historical.numberOfOccurences = Read<int>(kvp.Value); break;
                        case "historical.score": penalty.historical.score = Read<double>(kvp.Value); break;
                        case "player": penalty.player = Read(kvp.Value, ConvertPlayer); break;
                        case "score": penalty.score = Read<double>(kvp.Value); break;
                        case "givenPosition": penalty.givenPosition = Read(kvp.Value, converter: ConvertEnum<Position>); break;
                        case "wantedPosition": penalty.wantedPosition = Read(kvp.Value, converter: ConvertEnum<Position>); break;
                        case "usedSecondary": penalty.usedSecondary = Read<bool>(kvp.Value); break;
                        case "grade": penalty.grade = Read(kvp.Value, converter: ConvertEnum<Grade>); break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void ReadPenaltyWRONGTEAMSIZE(Dict dict, Match match)
            {
                WrongTeamSize penalty = new WrongTeamSize();
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "historical.mostRecentGameIndex": penalty.historical.mostRecentGameIndex = Read<int>(kvp.Value); break;
                        case "historical.numberOfOccurences": penalty.historical.numberOfOccurences = Read<int>(kvp.Value); break;
                        case "historical.score": penalty.historical.score = Read<double>(kvp.Value); break;
                        case "player": penalty.player = Read(kvp.Value, ConvertPlayer); break;
                        case "score": penalty.score = Read<double>(kvp.Value); break;
                        case "givenSize": penalty.givenSize = Read(kvp.Value, converter: ConvertEnum<TeamSize>); break;
                        case "wantedSize": penalty.wantedSize = Read(kvp.Value, converter: ConvertEnum<TeamSize>); break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void ReadPenaltyUNBALANCEDPLAYERS(Dict dict, Match match)
            {
                UnbalancedPlayers penalty = new UnbalancedPlayers();
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "historical.mostRecentGameIndex": penalty.historical.mostRecentGameIndex = Read<int>(kvp.Value); break;
                        case "historical.numberOfOccurences": penalty.historical.numberOfOccurences = Read<int>(kvp.Value); break;
                        case "historical.score": penalty.historical.score = Read<double>(kvp.Value); break;
                        case "player1": penalty.player1 = Read(kvp.Value, ConvertPlayer); break;
                        case "player2": penalty.player2 = Read(kvp.Value, ConvertPlayer); break;
                        case "grade1": penalty.grade1 = Read(kvp.Value, dictReader: ReadEffectiveGrade); break;
                        case "grade2": penalty.grade2 = Read(kvp.Value, dictReader: ReadEffectiveGrade); break;
                        case "score": penalty.score = Read<double>(kvp.Value); break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void ReadPenaltyUNBALANCEDTEAMS(Dict dict, Match match)
            {
                UnbalancedTeams penalty = new UnbalancedTeams();
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "historical.mostRecentGameIndex": penalty.historical.mostRecentGameIndex = Read<int>(kvp.Value); break;
                        case "historical.numberOfOccurences": penalty.historical.numberOfOccurences = Read<int>(kvp.Value); break;
                        case "historical.score": penalty.historical.score = Read<double>(kvp.Value); break;
                        case "score": penalty.score = Read<double>(kvp.Value); break;
                        case "team1Grades": penalty.team1Grades = ReadEffectiveGrades(Read<List<Dict>>(kvp.Value)); break;
                        case "team2Grades": penalty.team2Grades = ReadEffectiveGrades(Read<List<Dict>>(kvp.Value)); break;
                    }
                }
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
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "grade": effectiveGrade.grade = Read(kvp.Value, converter: ConvertEnum<Grade>); break;
                        case "positionIsPrimary": effectiveGrade.positionIsPrimary = Read<bool>(kvp.Value); break;
                        case "positionIsSecondary": effectiveGrade.positionIsSecondary = Read<bool>(kvp.Value); break;
                    }
                }
                return effectiveGrade;
            }

            void ReadWeights(Dict dict)
            {
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "IncorrectPosition": weights.IncorrectPosition = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "IncorrectTeamSize": weights.IncorrectTeamSize = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "PairPlayedTogetherAgainstEachOther": weights.PairPlayedTogetherAgainstEachOther = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "PairPlayedTogetherInTeam": weights.PairPlayedTogetherInTeam = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "SecondaryPosition": weights.SecondaryPosition = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "UnbalancedPlayers": weights.UnbalancedPlayers = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "UnbalancedTeams": weights.UnbalancedTeams = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "GoodLeadsMoveUp": weights.GoodLeadsMoveUp = Read(kvp.Value, dictReader: ReadWeight); break;
                        case "GoodSkipsGetSkip": weights.GoodSkipsGetSkip = Read(kvp.Value, dictReader: ReadWeight); break;
                    }
                }
            }

            Weight ReadWeight(Dict dict)
            {
                Weight weight = new Weight();
                foreach (var kvp in dict)
                {
                    switch (kvp.Key)
                    {
                        case "Score": weight.Score = Read<double>(kvp.Value); break;
                        case "Multiplier": weight.Multiplier = Read<double>(kvp.Value); break;
                    }
                }
                return weight;
            }
        }
    }
}
