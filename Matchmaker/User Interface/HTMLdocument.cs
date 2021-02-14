using Matchmaker.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Matchmaker.UserInterface
{
    public static class HTMLdocument
    {
        // todo: load these strings from a file which the user can edit
        const string repeatMarker = "<!--repeat-->";
        const string invisible = "style=\"display:none; \"";
        const string hyperlinkedPlayer = "<a href=\"javascript:window.external.ClickOnPlayer({0}, {1}, {2})\" style=\"color:#000000; text-decoration: none;\">{3}</a>";
        const string typeRinkFunction = "TypeRink({0}, this.value)";
        const string dropDownTeamSizeStart = "<select onChange=\"window.external.SelectSize({0}, this.value)\">";
        const string dropDownTeamSizeOptionNotSelected = "<option value = \"{0}\" >{1}</option>";
        const string dropDownTeamSizeOptionSelected = "<option value = \"{0}\" selected>{1}</option>";
        const string dropDownTeamSizeEnd = "</select>";
        const string empty = "&nbsp;";
        const string deleteMatch = "<button onclick=\"window.external.DeleteMatch({0})\">Delete</button>";
        const string typeDateFunction = "TypeDate(this.value)";
        const string oninputFunction = "oninput=\"window.external.{0}\" ";
        const string borderlessClass = "class=\"borderless\"";
        const string textboxWithProperties = "<input type=\"text\" value=\"{0}\" placeholder=\"{1}\" {2} {3} \">";
        const string deletedPlayer = "[deleted]";
        const string noPlayerSelected = "[no player selected]";
        const string rinkPlaceHolder = "#";
        const string datePlaceholder = "Date";

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
            var split = format.Split(new[] { repeatMarker }, StringSplitOptions.None);

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
                .Replace("%lead1pref%",   GetPosition(day, matchIndex, 0, Position.Lead,   mode))
                .Replace("%lead2pref%",   GetPosition(day, matchIndex, 1, Position.Lead,   mode))
                .Replace("%second1pref%", GetPosition(day, matchIndex, 0, Position.Second, mode))
                .Replace("%second2pref%", GetPosition(day, matchIndex, 1, Position.Second, mode))
                .Replace("%third1pref%",  GetPosition(day, matchIndex, 0, Position.Third,  mode))
                .Replace("%third2pref%",  GetPosition(day, matchIndex, 1, Position.Third,  mode))
                .Replace("%skip1pref%",   GetPosition(day, matchIndex, 0, Position.Skip,   mode))
                .Replace("%skip2pref%",   GetPosition(day, matchIndex, 1, Position.Skip,   mode))
                .Replace("%leadvisible%",   match.PositionShouldBeFilled(Position.Lead)   ? "" : invisible)
                .Replace("%secondvisible%", match.PositionShouldBeFilled(Position.Second) ? "" : invisible)
                .Replace("%thirdvisible%",  match.PositionShouldBeFilled(Position.Third)  ? "" : invisible)
                .Replace("%skipvisible%",   match.PositionShouldBeFilled(Position.Skip)   ? "" : invisible)
                ;
            return Substitute(result, day, mode);
        }

        static string GetName(Day day, int matchIndex, int teamIndex, Position position, HTMLmode mode)
        {
            Match match = day.matches[matchIndex];
            Team team = match.teams[teamIndex];
            if (!match.PositionShouldBeFilled(position)) return "";
            Player player = mode == HTMLmode.FixMatches ? team.players[(int)position] : team.Player(position);
            if (player == null && mode == HTMLmode.ViewHistory) return deletedPlayer;
            string name = player?.Name ?? noPlayerSelected;
            if (string.IsNullOrEmpty(name)) name = player.TagNumber;
            if (mode == HTMLmode.ViewHistory) return name;
            return string.Format(hyperlinkedPlayer, matchIndex, teamIndex, (int)position, name);
        }

        static string GetPosition(Day day, int matchIndex, int teamIndex, Position position, HTMLmode mode)
        {
            if (mode != HTMLmode.ConfirmMatches) return "";
            Match match = day.matches[matchIndex];
            Team team = match.teams[teamIndex];
            if (!match.PositionShouldBeFilled(position)) return "";
            Player player = team.Player(position);
            if (player == null) return "";
            string result = player.PreferencePrimary.Abbreviation();
            if (player.PreferenceSecondary.HasPreference)
                result += " " + player.PreferenceSecondary.Abbreviation();
            return result;
        }

        static string GetControlForRink(Day day, int matchIndex, HTMLmode mode)
        {
            return TextboxOrPlainText(mode != HTMLmode.ViewHistory, day.matches[matchIndex].rink, rinkPlaceHolder, string.Format(typeRinkFunction, matchIndex), false);
        }

        static string GetControlForTeamSize(Day day, int matchIndex, HTMLmode mode)
        {
            Match match = day.matches[matchIndex];
            if (mode != HTMLmode.FixMatches) return Enums.NameOfTeamSize(match.Size);
            string controls = string.Format(dropDownTeamSizeStart, matchIndex);
            for (int size = Team.MinSize; size <= Team.MaxSize; size++)
                controls += string.Format(match.Size == size ? dropDownTeamSizeOptionSelected : dropDownTeamSizeOptionNotSelected, size, Enums.NameOfTeamSize(size));
            controls += dropDownTeamSizeEnd;
            return controls;
        }

        static string GetControlForDelete(int matchIndex, HTMLmode mode)
        {
            if (mode != HTMLmode.FixMatches) return empty;
            return string.Format(deleteMatch, matchIndex);
        }

        public static string Substitute(string doc, Day day, HTMLmode mode)
        {
            return doc
                .Replace("%date%", GetControlForDate(day, mode));
        }

        static string GetControlForDate(Day day, HTMLmode mode)
        {
            return TextboxOrPlainText(mode != HTMLmode.ViewHistory, day.date, datePlaceholder, typeDateFunction, true);
        }

        static string TextboxOrPlainText(bool textbox, string value, string placeholder, string function, bool border)
        {
            return textbox
                ? string.Format(textboxWithProperties, value, placeholder, string.IsNullOrEmpty(function) ? "" : string.Format(oninputFunction, function), border ? "" : borderlessClass)
                : string.IsNullOrEmpty(value)
                ? empty
                : value;
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