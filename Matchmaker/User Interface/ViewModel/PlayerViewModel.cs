using Matchmaker.Data;
using System;

namespace Matchmaker.UserInterface.ViewModel
{
    public class PlayerViewModel : IFormattable
    {
        public readonly Player player;

        public PlayerViewModel(Player player)
        {
            this.player = player;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) return ToString();
            return format
                .Replace("%name", string.IsNullOrEmpty(Name) ? TagNumber : Name)
                .Replace("%tag", string.IsNullOrEmpty(TagNumber) ? "(visitor)" : TagNumber)
                .Replace("%position", PositionPrimary.ToString())
                .Replace("%visitor", Visitor ? "Visitor" : "Member");
        }

        public string TagNumber

        {
            get => player.TagNumber;
            set => player.TagNumber = value;
        }

        public string Name
        {
            get => player.Name;
            set => player.Name = value;
        }

        public Position PositionPrimary
        {
            get => player.PositionPrimary;
            set => player.PositionPrimary = value;
        }

        public Position PositionSecondary
        {
            get => player.PositionSecondary;
            set => player.PositionSecondary = value;
        }

        public Grade GradePrimary
        {
            get => player.GradePrimary;
            set => player.GradePrimary = value;
        }

        public Grade GradeSecondary
        {
            get => player.GradeSecondary;
            set => player.GradeSecondary = value;
        }

        public TeamSize PreferredTeamSizes
        {
            get => player.PreferredTeamSizes;
            set => player.PreferredTeamSizes = value;
        }

        public bool Visitor => player.Visitor;
    }
}
