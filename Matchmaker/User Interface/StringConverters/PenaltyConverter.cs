using Matchmaker.Data;
using System;
using System.Collections.Generic;

namespace Matchmaker.UserInterface.StringConverters
{
    public static class PenaltyConverter
    {
        public static string Convert(Penalty penalty)
        {
            string result = "";
            switch (penalty)
            {
                case PairAlreadyPlayedInTeam p:
                    result += p.player1 + " & " + p.player2;
                    result += " played with each other already. ";
                    return result;
                case PairAlreadyPlayedAgainstEachOther p:
                    result += p.player1 + " & " + p.player2;
                    result += " played against each other already. ";
                    return result;
                case IncorrectPosition p:
                    result = $"{p.player} ({p.grade.ToUserFriendlyString()}) wanted {p.wantedPosition.ToUserFriendlyString()} but got {p.givenPosition.ToUserFriendlyString()}";
                    if (p.usedSecondary)
                        result += " (second choice)";
                    result += ". ";
                    return result;
                case WrongTeamSize p:
                    result = $"{p.player} wanted to play {p.wantedSize.ToUserFriendlyString()} but got {p.givenSize.ToUserFriendlyString()}. ";
                    return result;
                case UnbalancedPlayers p:
                    result = $"{p.player1} & {p.player2} are not an even match. ({p.grade1} vs {p.grade2}). ";
                    return result;
                case UnbalancedTeams p:
                    result = $"{p.match} is not an even match. ";
                    for (int position = 0; position < Team.MaxSize; position++)
                        if (p.match.Team1.PositionShouldBeFilled((Position)position))
                            result += p.team1Grades[position] + " ";
                    result += "vs ";
                    for (int position = 0; position < Team.MaxSize; position++)
                        if (p.match.Team2.PositionShouldBeFilled((Position)position))
                            result += p.team2Grades[position] + " ";
                    return result;
                default:
                    throw new ArgumentException("Unknown penalty");
            }
        }

        public static string CiteOccurence(Penalty penalty, IList<Day> history)
        {
            if (penalty.historical.numberOfOccurences == 0) return "";
            string result = "This has happened ";
            result += penalty.historical.numberOfOccurences switch
            {
                1 => "once",
                2 => "twice",
                _ => penalty.historical.numberOfOccurences + " times",
            };
            result += ", most recently on ";
            if (penalty.historical.mostRecentGameIndex == -1)
                result += "[deleted]";
            else if (history == null)
                result += penalty.historical.mostRecentGameIndex;
            else
                result += history[penalty.historical.mostRecentGameIndex];
            return result;
        }
    }
}
