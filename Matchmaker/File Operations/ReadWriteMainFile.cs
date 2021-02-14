using Matchmaker.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dict = System.Collections.Generic.Dictionary<string, object>;

namespace Matchmaker.FileOperations
{
    static class ReadWriteMainFile
    {
        const string codeForNewSection = "--";

        public static void Output(string filename, IList<Player> players, IList<Day> history, Weights weights)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            StringBuilder stringBuilder = new StringBuilder();

            foreach (Player player in players)
                WritePlayer(player);
            foreach (Day day in history)
                WriteDay(day);
            WriteWeights(weights);

            using StreamWriter streamWriter = new StreamWriter(filename + ".old");
            streamWriter.Write(stringBuilder);

            OutputJson(filename, players, history, weights);

            void WriteNewSection(string name)
            {
                Write(codeForNewSection, name);
            }

            void Write(string code, object data)
            {
                stringBuilder.Append(code + data + Environment.NewLine);
            }

            void FinishSection()
            {
                stringBuilder.Append(Environment.NewLine);
            }

            void WritePlayer(Player player)
            {
                WriteNewSection("PLAYER:");
                Write("Na", player.Name);
                Write("Id", player.ID);
                Write("P1", player.PositionPrimary);
                Write("P2", player.PositionSecondary);
                Write("G1", player.GradePrimary);
                Write("G2", player.GradeSecondary);
                Write("TS", player.PreferredTeamSizes);
                Write("TN", player.TagNumber);
                FinishSection();
            }

            void WriteDay(Day day)
            {
                WriteNewSection("DAY:");
                Write("Da", day.date);
                foreach (Match match in day.matches)
                {
                    WriteMatch(match);
                }
                FinishSection();
            }

            void WriteMatch(Match match)
            {
                WriteNewSection("MATCH:");
                Write("Ri", match.rink);
                Write("IF", match.isFixed);
                Write("T1", ConvertTeam(match.Team1));
                Write("T2", ConvertTeam(match.Team2));
                foreach (Penalty penalty in match.penalties)
                {
                    WritePenalty(penalty);
                }
                FinishSection();
            }

            string ConvertTeam(Team team)
            {
                string result = "";
                for (int position = 0; position < Team.MaxSize; position++)
                {
                    if (team.PositionShouldBeFilled((Position)position))
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            result += ",";
                        }
                        result += ConvertPlayer(team.players[position]);
                    }
                }
                return result;
            }

            string ConvertPlayer(Player player)
            {
                return player?.ID.ToString() ?? "_";
            }

            void WritePenalty(Penalty penalty)
            {
                switch (penalty)
                {
                    case PairAlreadyPlayedInTeam p: WritePenaltyPAIRALREADYPLAYEDINTEAM(p); break;
                    case PairAlreadyPlayedAgainstEachOther p: WritePenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(p); break;
                    case IncorrectPosition p: WritePenaltyINCORRECTPOSITION(p); break;
                    case WrongTeamSize p: WritePenaltyWRONGTEAMSIZE(p); break;
                    case UnbalancedPlayers p: WritePenaltyUNBALANCEDPLAYERS(p); break;
                    case UnbalancedTeams p: WritePenaltyUNBALANCEDTEAMS(p); break;
                    default: throw new ArgumentException("Unknown penalty");
                }
            }

            void WritePenaltyPAIRALREADYPLAYEDINTEAM(PairAlreadyPlayedInTeam penalty)
            {
                WriteNewSection("PAIRALREADYPLAYEDINTEAM:");
                Write("RG", penalty.historical.mostRecentGameIndex);
                Write("NO", penalty.historical.numberOfOccurences);
                Write("HS", penalty.historical.score);
                Write("P1", ConvertPlayer(penalty.player1));
                Write("P2", ConvertPlayer(penalty.player2));
                Write("Sc", penalty.score);
                FinishSection();
            }

            void WritePenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(PairAlreadyPlayedAgainstEachOther penalty)
            {
                WriteNewSection("PAIRALREADYPLAYEDAGAINSTEACHOTHER:");
                Write("RG", penalty.historical.mostRecentGameIndex);
                Write("NO", penalty.historical.numberOfOccurences);
                Write("HS", penalty.historical.score);
                Write("P1", ConvertPlayer(penalty.player1));
                Write("P2", ConvertPlayer(penalty.player2));
                Write("Sc", penalty.score);
                FinishSection();
            }

            void WritePenaltyINCORRECTPOSITION(IncorrectPosition penalty)
            {
                WriteNewSection("INCORRECTPOSITION:");
                Write("RG", penalty.historical.mostRecentGameIndex);
                Write("NO", penalty.historical.numberOfOccurences);
                Write("HS", penalty.historical.score);
                Write("Pl", ConvertPlayer(penalty.player));
                Write("Sc", penalty.score);
                Write("GP", penalty.givenPosition);
                Write("WP", penalty.wantedPosition);
                Write("US", penalty.usedSecondary);
                Write("Gr", penalty.grade);
                FinishSection();
            }

            void WritePenaltyWRONGTEAMSIZE(WrongTeamSize penalty)
            {
                WriteNewSection("WRONGTEAMSIZE:");
                Write("RG", penalty.historical.mostRecentGameIndex);
                Write("NO", penalty.historical.numberOfOccurences);
                Write("HS", penalty.historical.score);
                Write("Pl", ConvertPlayer(penalty.player));
                Write("Sc", penalty.score);
                Write("GS", penalty.givenSize);
                Write("WS", penalty.wantedSize);
                FinishSection();
            }

            void WritePenaltyUNBALANCEDPLAYERS(UnbalancedPlayers penalty)
            {
                WriteNewSection("UNBALANCEDPLAYERS:");
                Write("RG", penalty.historical.mostRecentGameIndex);
                Write("NO", penalty.historical.numberOfOccurences);
                Write("HS", penalty.historical.score);
                Write("P1", ConvertPlayer(penalty.player1));
                Write("P2", ConvertPlayer(penalty.player2));
                Write("G1", ConvertEffectiveGrade(penalty.grade1));
                Write("G2", ConvertEffectiveGrade(penalty.grade2));
                Write("Sc", penalty.score);
                FinishSection();
            }

            void WritePenaltyUNBALANCEDTEAMS(UnbalancedTeams penalty)
            {
                WriteNewSection("UNBALANCEDTEAMS:");
                Write("Sc", penalty.score);
                Write("G1", string.Join(",", penalty.team1Grades.Select(ConvertEffectiveGrade)));
                Write("G2", string.Join(",", penalty.team2Grades.Select(ConvertEffectiveGrade)));
                FinishSection();
            }

            static string ConvertEffectiveGrade(EffectiveGrade effectiveGrade)
            {
                return $"G{effectiveGrade.grade} P{effectiveGrade.positionIsPrimary} S{effectiveGrade.positionIsSecondary}";
            }

            void WriteWeights(Weights weights)
            {
                WriteNewSection("WEIGHTS:");
                Write("IP", ConvertWeight(weights.IncorrectPosition));
                Write("TS", ConvertWeight(weights.IncorrectTeamSize));
                Write("PA", ConvertWeight(weights.PairPlayedTogetherAgainstEachOther));
                Write("PT", ConvertWeight(weights.PairPlayedTogetherInTeam));
                Write("SP", ConvertWeight(weights.SecondaryPosition));
                Write("GP", ConvertWeight(weights.UnbalancedPlayers));
                Write("GT", ConvertWeight(weights.UnbalancedTeams));
                Write("PG", ConvertWeight(weights.BadPositionForGoodGrade));
                FinishSection();
            }

            static string ConvertWeight(Weight weight)
            {
                return "S" + weight.Score + " M" + weight.Multiplier;
            }
        }

        public static void Input(string filename, IList<Player> players, IList<Day> history, Weights weights)
        {
            try
            {
                InputJson(filename, players, history, weights);
                return;
            }
            catch
            {

            }

            if (!File.Exists(filename)) return;

            using StreamReader sr = new StreamReader(filename);

            while (HasNextLine(out string code, out string value))
            {
                if (code == codeForNewSection)
                {
                    switch (value)
                    {
                        case "PLAYER:":
                            InputPlayer();
                            break;
                        case "DAY:":
                            InputDay();
                            break;
                        case "WEIGHTS:":
                            InputWeights();
                            break;
                        default:
                            InputUnknownSection();
                            break;
                    }
                }
            }

            bool HasNextLine(out string code, out string value)
            {
                string line = sr.ReadLine();
                code = line;
                value = "";
                if (string.IsNullOrEmpty(line)) return false;
                if (line.Length > 2)
                {
                    code = line.Remove(2);
                    value = line.Substring(2);
                }
                return true;
            }

            int ConvertInt(string data)
            {
                if (int.TryParse(data, out int i))
                    return i;
                throw new ArgumentException();
            }

            double ConvertDouble(string data)
            {
                if (double.TryParse(data, out double d))
                    return d;
                throw new ArgumentException();
            }

            TEnum ConvertEnum<TEnum>(string data) where TEnum : struct
            {
                if (Enum.TryParse(data, out TEnum value))
                    return value;
                throw new ArgumentException();
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

            bool ConvertBool(string data)
            {
                return !string.IsNullOrEmpty(data) && char.ToUpper(data[0]) == 'T';
            }

            Weight ConvertWeight(string data)
            {
                Weight weight = new Weight();
                string[] fields = data.Split(' ');
                foreach (string field in fields)
                {
                    string code = field.Remove(1);
                    string value = field.Substring(1);
                    switch (code)
                    {
                        case "S":
                            weight.Score = ConvertDouble(value);
                            break;
                        case "M":
                            weight.Multiplier = ConvertDouble(value);
                            break;
                    }
                }
                return weight;
            }

            EffectiveGrade ConvertEffectiveGrade(string data)
            {
                EffectiveGrade grade = new EffectiveGrade();
                string[] fields = data.Split(' ');
                foreach (string field in fields)
                {
                    string code = field.Remove(1);
                    string value = field.Substring(1);
                    switch (code)
                    {
                        case "G":
                            grade.grade = ConvertEnum<Grade>(value);
                            break;
                        case "P":
                            grade.positionIsPrimary = ConvertBool(value);
                            break;
                        case "S":
                            grade.positionIsSecondary = ConvertBool(value);
                            break;
                    }
                }
                return grade;
            }

            void InputUnknownSection()
            {
                while (HasNextLine(out string code, out _))
                {
                    if (code == codeForNewSection)
                    {
                        InputUnknownSection();
                    }
                }
            }

            void InputPlayer()
            {
                Player player = new Player();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "Na":
                            player.Name = value;
                            break;
                        case "Id":
                            player.ID = ConvertInt(value);
                            break;
                        case "P1":
                            player.PositionPrimary = ConvertEnum<Position>(value);
                            break;
                        case "P2":
                            player.PositionSecondary = ConvertEnum<Position>(value);
                            break;
                        case "G1":
                            player.GradePrimary = ConvertEnum<Grade>(value);
                            break;
                        case "G2":
                            player.GradeSecondary = ConvertEnum<Grade>(value);
                            break;
                        case "TS":
                            player.PreferredTeamSizes = ConvertEnum<TeamSize>(value);
                            break;
                        case "TN":
                            player.TagNumber = value;
                            break;
                    }
                }
                players.Add(player);
            }

            void InputDay()
            {
                Day day = new Day();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case codeForNewSection:
                            if (value == "MATCH:")
                            {
                                InputMatch(day);
                            }
                            else
                            {
                                InputUnknownSection();
                            }
                            break;
                        case "Da":
                            day.date = value;
                            break;
                    }
                }
                history.Add(day);
            }

            void InputMatch(Day day)
            {
                Match match = new Match();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case codeForNewSection:
                            switch (value)
                            {
                                case "PAIRALREADYPLAYEDINTEAM:":
                                    InputPenaltyPAIRALREADYPLAYEDINTEAM(match);
                                    break;
                                case "PAIRALREADYPLAYEDAGAINSTEACHOTHER:":
                                    InputPenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(match);
                                    break;
                                case "INCORRECTPOSITION:":
                                    InputPenaltyINCORRECTPOSITION(match);
                                    break;
                                case "WRONGTEAMSIZE:":
                                    InputPenaltyWRONGTEAMSIZE(match);
                                    break;
                                case "UNBALANCEDPLAYERS:":
                                    InputPenaltyUNBALANCEDPLAYERS(match);
                                    break;
                                case "UNBALANCEDTEAMS:":
                                    InputPenaltyUNBALANCEDTEAMS(match);
                                    break;
                                default:
                                    InputUnknownSection();
                                    break;
                            }
                            break;
                        case "Ri":
                            match.rink = value;
                            break;
                        case "IF":
                            match.isFixed = ConvertBool(value);
                            break;
                        case "T1":
                        case "T2":
                            var team = code == "T1" ? match.Team1 : match.Team2;
                            string[] playerIDs = value.Split(',');
                            team.size = playerIDs.Length;
                            Position position = (Position)0;
                            foreach (string playerID in playerIDs)
                            {
                                while (!team.PositionShouldBeFilled(position))
                                {
                                    position++;
                                }
                                team.players[(int)position] = ConvertPlayer(playerID);
                                position++;
                            }
                            break;
                    }
                }
                day.matches.Add(match);
            }

            void InputPenaltyPAIRALREADYPLAYEDINTEAM(Match match)
            {
                PairAlreadyPlayedInTeam penalty = new PairAlreadyPlayedInTeam();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "RG":
                            penalty.historical.mostRecentGameIndex = ConvertInt(value);
                            break;
                        case "NO":
                            penalty.historical.numberOfOccurences = ConvertInt(value);
                            break;
                        case "HS":
                            penalty.historical.score = ConvertDouble(value);
                            break;
                        case "P1":
                            penalty.player1 = ConvertPlayer(value);
                            break;
                        case "P2":
                            penalty.player2 = ConvertPlayer(value);
                            break;
                        case "Sc":
                            penalty.score = ConvertDouble(value);
                            break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void InputPenaltyPAIRALREADYPLAYEDAGAINSTEACHOTHER(Match match)
            {
                PairAlreadyPlayedAgainstEachOther penalty = new PairAlreadyPlayedAgainstEachOther();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "RG":
                            penalty.historical.mostRecentGameIndex = ConvertInt(value);
                            break;
                        case "NO":
                            penalty.historical.numberOfOccurences = ConvertInt(value);
                            break;
                        case "HS":
                            penalty.historical.score = ConvertDouble(value);
                            break;
                        case "P1":
                            penalty.player1 = ConvertPlayer(value);
                            break;
                        case "P2":
                            penalty.player2 = ConvertPlayer(value);
                            break;
                        case "Sc":
                            penalty.score = ConvertDouble(value);
                            break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void InputPenaltyINCORRECTPOSITION(Match match)
            {
                IncorrectPosition penalty = new IncorrectPosition();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "RG":
                            penalty.historical.mostRecentGameIndex = ConvertInt(value);
                            break;
                        case "NO":
                            penalty.historical.numberOfOccurences = ConvertInt(value);
                            break;
                        case "HS":
                            penalty.historical.score = ConvertDouble(value);
                            break;
                        case "Pl":
                            penalty.player = ConvertPlayer(value);
                            break;
                        case "Sc":
                            penalty.score = ConvertDouble(value);
                            break;
                        case "GP":
                            penalty.givenPosition = ConvertEnum<Position>(value);
                            break;
                        case "WP":
                            penalty.wantedPosition = ConvertEnum<Position>(value);
                            break;
                        case "US":
                            penalty.usedSecondary = ConvertBool(value);
                            break;
                        case "Gr":
                            penalty.grade = ConvertEnum<Grade>(value);
                            break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void InputPenaltyWRONGTEAMSIZE(Match match)
            {
                WrongTeamSize penalty = new WrongTeamSize();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "RG":
                            penalty.historical.mostRecentGameIndex = ConvertInt(value);
                            break;
                        case "NO":
                            penalty.historical.numberOfOccurences = ConvertInt(value);
                            break;
                        case "HS":
                            penalty.historical.score = ConvertDouble(value);
                            break;
                        case "Pl":
                            penalty.player = ConvertPlayer(value);
                            break;
                        case "Sc":
                            penalty.score = ConvertDouble(value);
                            break;
                        case "GS":
                            penalty.givenSize = ConvertEnum<TeamSize>(value);
                            break;
                        case "WS":
                            penalty.wantedSize = ConvertEnum<TeamSize>(value);
                            break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void InputPenaltyUNBALANCEDPLAYERS(Match match)
            {
                UnbalancedPlayers penalty = new UnbalancedPlayers();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "RG":
                            penalty.historical.mostRecentGameIndex = ConvertInt(value);
                            break;
                        case "NO":
                            penalty.historical.numberOfOccurences = ConvertInt(value);
                            break;
                        case "HS":
                            penalty.historical.score = ConvertDouble(value);
                            break;
                        case "P1":
                            penalty.player1 = ConvertPlayer(value);
                            break;
                        case "P2":
                            penalty.player2 = ConvertPlayer(value);
                            break;
                        case "G1":
                            penalty.grade1 = ConvertEffectiveGrade(value);
                            break;
                        case "G2":
                            penalty.grade2 = ConvertEffectiveGrade(value);
                            break;
                        case "Sc":
                            penalty.score = ConvertDouble(value);
                            break;
                    }
                }
                match.penalties.Add(penalty);
            }

            void InputPenaltyUNBALANCEDTEAMS(Match match)
            {
                UnbalancedTeams penalty = new UnbalancedTeams();
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "Sc":
                            penalty.score = ConvertDouble(value);
                            break;
                        case "G1":
                        case "G2":
                            string[] stringGrades = value.Split(',');
                            EffectiveGrade[] grades = new EffectiveGrade[Team.MaxSize];
                            for (int i = 0; i < Team.MaxSize; i++)
                                grades[i] = ConvertEffectiveGrade(stringGrades[i]);
                            if (code == "G1")
                                penalty.team1Grades = grades;
                            else
                                penalty.team2Grades = grades;
                            break;
                    }
                }
                penalty.match = match;
                match.penalties.Add(penalty);
            }

            void InputWeights()
            {
                while (HasNextLine(out string code, out string value))
                {
                    switch (code)
                    {
                        case "IP":
                            weights.IncorrectPosition = ConvertWeight(value);
                            break;
                        case "YS":
                            weights.IncorrectTeamSize = ConvertWeight(value);
                            break;
                        case "PA":
                            weights.PairPlayedTogetherAgainstEachOther = ConvertWeight(value);
                            break;
                        case "PT":
                            weights.PairPlayedTogetherInTeam = ConvertWeight(value);
                            break;
                        case "SP":
                            weights.SecondaryPosition = ConvertWeight(value);
                            break;
                        case "GP":
                            weights.UnbalancedPlayers = ConvertWeight(value);
                            break;
                        case "GT":
                            weights.UnbalancedTeams = ConvertWeight(value);
                            break;
                        case "PG":
                            weights.BadPositionForGoodGrade = ConvertWeight(value);
                            break;
                    }
                }
            }
        }

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

        public static void OutputJson(string filename, IList<Player> players, IList<Day> history, Weights weights)
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
                { "BadPositionForGoodGrade", WriteWeight(weights.BadPositionForGoodGrade) },
            };

            Dict WriteWeight(Weight weight) => new Dict {
                { "Score", weight.Score },
                { "Multiplier", weight.Multiplier },
            };
        }

        public static void InputJson(string filename, IList<Player> players, IList<Day> history, Weights weights)
        {
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
                Read(dict, "BadPositionForGoodGrade", ref weights.BadPositionForGoodGrade, dictReader: ReadWeight);
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
