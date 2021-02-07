using System.Collections.Generic;

namespace Matchmaker.Data
{
    public class Day
    {
        public string date;
        public List<Match> matches = new List<Match>();

        public override string ToString() => date;
    }

    public static class DayExtension
    {
        /// <summary>
        /// Get every player that plays on a particular day
        /// </summary>
        public static IEnumerable<Player> Players(this Day day)
        {
            if (day != null)
                foreach (Match match in day.matches)
                    foreach (Team team in match.teams)
                        foreach (Player player in team.players)
                            if (player != null)
                                yield return player;
        }
    }
}
