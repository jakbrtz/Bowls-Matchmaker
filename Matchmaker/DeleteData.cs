using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matchmaker
{
    public static class DeleteData
    {
        public static void DeleteFromHistory(IList<Day> history, IList<int> indices)
        {
            // Where ever a day is referenced by index, the index needs to change
            // Work out what each index gets changed to
            int numberOfDaysDeleted = 0;
            int[] dayIndexMap = new int[history.Count];
            for (int dayIndex = 0; dayIndex < history.Count; dayIndex++)
            {
                if (numberOfDaysDeleted < indices.Count && indices[numberOfDaysDeleted] == dayIndex)
                {
                    dayIndexMap[dayIndex] = -1;
                    numberOfDaysDeleted++;
                }
                else
                {
                    dayIndexMap[dayIndex] = dayIndex - numberOfDaysDeleted;
                }
            }

            // Go through all penalties and update their day reference
            foreach (Day day in history)
                foreach (Match match in day.matches)
                    foreach (Penalty penalty in match.penalties)
                        if (penalty.historical.mostRecentGameIndex != -1)
                            penalty.historical.mostRecentGameIndex = dayIndexMap[penalty.historical.mostRecentGameIndex];

            // Delete the days
            foreach (int indexForDeleting in indices)
                history.RemoveAt(indexForDeleting);
        }

        public static void DeletePlayers(List<Player> players, IList<Player> allPlayers, IList<Day> history)
        {
            foreach (Day day in history)
            {
                foreach (Match match in day.matches)
                {
                    foreach (Team team in match.teams)
                    {
                        for (int position = 0; position < Team.MaxSize; position++)
                        {
                            if (players.Contains(team.players[position]))
                            {
                                team.players[position] = null;
                            }
                        }
                    }
                    foreach (Penalty penalty in match.penalties)
                    {
                        switch (penalty)
                        {
                            case PairAlreadyPlayedInTeam p:
                                if (players.Contains(p.player1))
                                    p.player1 = null;
                                if (players.Contains(p.player2))
                                    p.player2 = null;
                                break;
                            case PairAlreadyPlayedAgainstEachOther p:
                                if (players.Contains(p.player1))
                                    p.player1 = null;
                                if (players.Contains(p.player2))
                                    p.player2 = null;
                                break;
                            case IncorrectPosition p:
                                if (players.Contains(p.player))
                                    p.player = null;
                                break;
                            case WrongTeamSize p:
                                if (players.Contains(p.player))
                                    p.player = null;
                                break;
                            case UnbalancedPlayers p:
                                if (players.Contains(p.player1))
                                    p.player1 = null;
                                if (players.Contains(p.player2))
                                    p.player2 = null;
                                break;
                            case UnbalancedTeams p:
                                // Nothing to do
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
            }
            foreach (Player player in players)
                allPlayers.Remove(player);
        }
    }
}
