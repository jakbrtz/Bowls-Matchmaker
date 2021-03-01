using Matchmaker.Data;
using Matchmaker.UserInterface.StringConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Matchmaker.UserInterface
{
    public static class HTMLdocument
    {
        public static string format = Properties.Resources.table;
        public static HTMLelements elements = new HTMLelements();

        public static string GenerateDays(List<Day> history)
        {
            string result = "";
            foreach (Day day in history)
                result += GenerateDay(day, HTMLmode.ViewHistory) + Environment.NewLine;
            return result;
        }

        public static string GenerateDay(Day day, HTMLmode mode)
        {
            var split = format.Split(new[] { elements.repeatMarker }, StringSplitOptions.None);

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
                .Replace("%leadvisible%",   match.PositionShouldBeFilled(Position.Lead)   ? "" : elements.invisible)
                .Replace("%secondvisible%", match.PositionShouldBeFilled(Position.Second) ? "" : elements.invisible)
                .Replace("%thirdvisible%",  match.PositionShouldBeFilled(Position.Third)  ? "" : elements.invisible)
                .Replace("%skipvisible%",   match.PositionShouldBeFilled(Position.Skip)   ? "" : elements.invisible)
                ;
            return Substitute(result, day, mode);
        }

        static string GetName(Day day, int matchIndex, int teamIndex, Position position, HTMLmode mode)
        {
            Match match = day.matches[matchIndex];
            Team team = match.teams[teamIndex];
            if (!match.PositionShouldBeFilled(position)) return "";
            Player player = mode == HTMLmode.FixMatches ? team.players[(int)position] : team.Player(position);
            if (player == null && mode == HTMLmode.ViewHistory) return elements.deletedPlayer;
            string name = player?.Name ?? elements.noPlayerSelected;
            if (string.IsNullOrEmpty(name)) name = player.TagNumber;
            if (mode == HTMLmode.ViewHistory) return name;
            return string.Format(elements.hyperlinkedPlayer, matchIndex, teamIndex, (int)position, name);
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
            return TextboxOrPlainText(
                mode != HTMLmode.ViewHistory, 
                day.matches[matchIndex].rink, 
                elements.rinkPlaceHolder, 
                string.Format(elements.typeRinkFunction, matchIndex), 
                false);
        }

        static string GetControlForTeamSize(Day day, int matchIndex, HTMLmode mode)
        {
            Match match = day.matches[matchIndex];
            if (mode != HTMLmode.FixMatches) return EnumStringConverter.NameOfTeamSize(match.Size);
            string controls = string.Format(elements.dropDownTeamSizeStart, matchIndex);
            for (int size = Team.MinSize; size <= Team.MaxSize; size++)
                controls += string.Format(
                    match.Size == size ? elements.dropDownTeamSizeOptionSelected : elements.dropDownTeamSizeOptionNotSelected, 
                    size, 
                    EnumStringConverter.NameOfTeamSize(size));
            controls += elements.dropDownTeamSizeEnd;
            return controls;
        }

        static string GetControlForDelete(int matchIndex, HTMLmode mode)
        {
            if (mode != HTMLmode.FixMatches) return elements.empty;
            return string.Format(elements.deleteMatch, matchIndex);
        }

        public static string Substitute(string doc, Day day, HTMLmode mode)
        {
            return doc
                .Replace("%date%", GetControlForDate(day, mode));
        }

        static string GetControlForDate(Day day, HTMLmode mode)
        {
            return TextboxOrPlainText(mode != HTMLmode.ViewHistory, day.date, elements.datePlaceholder, elements.typeDateFunction, true);
        }

        static string TextboxOrPlainText(bool textbox, string value, string placeholder, string function, bool border)
        {
            return textbox
                ? string.Format(
                    elements.textboxWithProperties, 
                    value, 
                    placeholder, 
                    string.IsNullOrEmpty(function) ? "" : string.Format(elements.oninputFunction, function), 
                    border ? "" : elements.borderlessClass)
                : string.IsNullOrEmpty(value)
                ? elements.empty
                : value;
        }
    }

    public class HTMLelements
    {
        public string repeatMarker, invisible, hyperlinkedPlayer, typeRinkFunction, dropDownTeamSizeStart, dropDownTeamSizeOptionNotSelected,
         dropDownTeamSizeOptionSelected, dropDownTeamSizeEnd, empty, deleteMatch, typeDateFunction, oninputFunction, borderlessClass,
         textboxWithProperties, deletedPlayer, noPlayerSelected, rinkPlaceHolder, datePlaceholder;
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