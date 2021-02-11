using Matchmaker.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// todo: use JSON instead

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

            using StreamWriter streamWriter = new StreamWriter(filename);
            streamWriter.Write(stringBuilder);

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
    }
}
