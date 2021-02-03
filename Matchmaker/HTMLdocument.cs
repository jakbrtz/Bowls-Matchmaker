using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Matchmaker
{
    public static class HTMLdocument
    {
        public static string format = Properties.Resources.table;

        public static void ReloadFormat(string directory)
        {
            if (File.Exists(directory))
            {
                using StreamReader streamReader = new StreamReader(directory);
                format = streamReader.ReadToEnd();
            }
            else
            {
                format = Properties.Resources.table;
            }
        }

        public static string GenerateDays(List<Day> history)
        {
            string result = "";
            foreach (Day day in history)
                result += GenerateDay(day, HTMLmode.ViewHistory) + Environment.NewLine;
            return result;
        }

        public static string GenerateDay(Day day, HTMLmode mode)
        {
            var split = format.Split(new[] { "<!--repeat-->" }, StringSplitOptions.None);

            string result = "";
            for (int i = 0; i < split.Length; i++)
            {
                if (i % 2 == 0)
                {
                    result += Substitute(split[i], day, mode);
                }
                else
                {
                    for (int matchIndex = 0; matchIndex < day.matches.Count; matchIndex++)
                    {
                        result += Substitute(split[i], day, matchIndex, mode);
                    }
                }
            }
            return result;
        }

        static string Substitute(string doc, Day day, int matchIndex, HTMLmode mode)
        {
            Match match = day.matches[matchIndex];
            string result = doc
                .Replace("%rink%",        GetControlForRink(day, matchIndex, mode))
                .Replace("%size%",        GetControlForTeamSize(day, matchIndex, mode))
                .Replace("%delete%",      GetControlForDelete(matchIndex, mode))
                .Replace("%lead1%",       GetName(day, matchIndex, 0, Position.Lead,   mode))
                .Replace("%lead2%",       GetName(day, matchIndex, 1, Position.Lead,   mode))
                .Replace("%second1%",     GetName(day, matchIndex, 0, Position.Second, mode))
                .Replace("%second2%",     GetName(day, matchIndex, 1, Position.Second, mode))
                .Replace("%third1%",      GetName(day, matchIndex, 0, Position.Third,  mode))
                .Replace("%third2%",      GetName(day, matchIndex, 1, Position.Third,  mode))
                .Replace("%skip1%",       GetName(day, matchIndex, 0, Position.Skip,   mode))
                .Replace("%skip2%",       GetName(day, matchIndex, 1, Position.Skip,   mode))
                .Replace("%lead1pref%",   GetPosition(match.Team1, Position.Lead,   mode))
                .Replace("%lead2pref%",   GetPosition(match.Team2, Position.Lead,   mode))
                .Replace("%second1pref%", GetPosition(match.Team1, Position.Second, mode))
                .Replace("%second2pref%", GetPosition(match.Team2, Position.Second, mode))
                .Replace("%third1pref%",  GetPosition(match.Team1, Position.Third,  mode))
                .Replace("%third2pref%",  GetPosition(match.Team2, Position.Third,  mode))
                .Replace("%skip1pref%",   GetPosition(match.Team1, Position.Skip,   mode))
                .Replace("%skip2pref%",   GetPosition(match.Team2, Position.Skip,   mode))
                .Replace("%leadvisible%",   match.Team1.PositionShouldBeFilled(Position.Lead)   ? "" : "style=\"display:none; \"")
                .Replace("%secondvisible%", match.Team1.PositionShouldBeFilled(Position.Second) ? "" : "style=\"display:none; \"")
                .Replace("%thirdvisible%",  match.Team1.PositionShouldBeFilled(Position.Third)  ? "" : "style=\"display:none; \"")
                .Replace("%skipvisible%",   match.Team1.PositionShouldBeFilled(Position.Skip)   ? "" : "style=\"display:none; \"")
                ;
            return Substitute(result, day, mode);
        }

        static string GetName(Day day, int matchIndex, int teamIndex, Position position, HTMLmode mode)
        {
            Team team = day.matches[matchIndex].teams[teamIndex];
            if (!team.PositionShouldBeFilled(position)) return "";
            Player player = team.players[(int)position];
            if (player == null && mode==HTMLmode.ViewHistory) return "[deleted]";
            string name = player?.Name ?? "[no player selected]";
            if (string.IsNullOrEmpty(name)) name = player.TagNumber;
            if (mode == HTMLmode.ViewHistory) return name;
            return $"<a href=\"javascript:window.external.ClickOnPlayer({matchIndex}, {teamIndex}, {(int)position})\" style=\"color:#000000; text-decoration: none;\">{name}</a>";
        }

        static string GetPosition(Team team, Position position, HTMLmode mode)
        {
            if (mode!=HTMLmode.ConfirmMatches) return "";
            Player player = team.players[(int)position];
            if (player == null) return "";
            string result = player.PreferencePrimary.Abbreviation();
            if (player.PreferenceSecondary.HasPreference)
                result += " " + player.PreferenceSecondary.Abbreviation();
            return result;
        }

        static string GetControlForRink(Day day, int matchIndex, HTMLmode mode)
        {
            return TextboxOrPlainText(mode==HTMLmode.ConfirmMatches||mode==HTMLmode.FixMatches, day.matches[matchIndex].rink, "#", $"TypeRink({matchIndex}, this.value)", false);
        }

        static string GetControlForTeamSize(Day day, int matchIndex, HTMLmode mode)
        {
            Match match = day.matches[matchIndex];
            if (mode != HTMLmode.FixMatches) return EnumParser.NameOfTeamSize(match.Size);
            string controls = "";
            controls += $"<select onChange=\"window.external.SelectSize({matchIndex}, this.value)\">";
            for (int size = Team.MinSize; size <= Team.MaxSize; size++)
                controls += $"<option value = \"{size}\" {(day.matches[matchIndex].Size == size ? "selected" : "")}>{EnumParser.NameOfTeamSize(size)}</option>";
            controls += "</select>";
            return controls;
        }

        static string GetControlForDelete(int matchIndex, HTMLmode mode)
        {
            if (mode != HTMLmode.FixMatches) return "&nbsp;";
            return $"<button onclick=\"window.external.DeleteMatch({matchIndex})\">Delete</button>";
        }

        public static string Substitute(string doc, Day day, HTMLmode mode)
        {
            return doc
                .Replace("%date%", GetControlForDate(day, mode));
        }

        static string GetControlForDate(Day day, HTMLmode mode)
        {
            return TextboxOrPlainText(mode!=HTMLmode.ViewHistory, day.date, "Date", "TypeDate(this.value)", true);
        }

        static string TextboxOrPlainText(bool textbox, string value, string placeholder, string function, bool border)
        {
            if (textbox)
            {
                string result = "<input type=\"text\" ";
                result += $"value=\"{value}\" ";
                result += $"placeholder=\"{placeholder}\"";
                if (!string.IsNullOrEmpty(function))
                    result += $"oninput=\"window.external.{function}\" ";
                if (!border)
                    result += "class=\"borderless\"";
                result += "\">";
                return result;
            }
            if (string.IsNullOrEmpty(value))
            {
                return "&nbsp;";
            }
            return value;
        }
    }

    [ComVisible(true)]
    public class HTMLscripter
    {
        public event Action<int, int, int> PlayerClickedOn;
        public event Action<int, string> RinkChanged;
        public event Action<string> DateChanged;
        public event Action<int, int> SizeChanged;
        public event Action<int> MatchDeleted;

        public void ClickOnPlayer(int matchIndex, int teamIndex, int positionIndex)
        {
            PlayerClickedOn(matchIndex, teamIndex, positionIndex);
        }

        public void TypeRink(int matchIndex, string value)
        {
            RinkChanged(matchIndex, value);
        }

        public void TypeDate(string value)
        {
            DateChanged(value);
        }

        public void SelectSize(int matchIndex, int size)
        {
            SizeChanged(matchIndex, size);
        }

        public void DeleteMatch(int matchIndex)
        {
            MatchDeleted(matchIndex);
        }
    }

    public enum HTMLmode { ViewHistory, ConfirmMatches, FixMatches }
}

// todo: 
// how to remove header/footer from page? (edit registry)
// remove magic strings from this file