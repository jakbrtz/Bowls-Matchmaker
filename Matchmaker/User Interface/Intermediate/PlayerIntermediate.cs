using Matchmaker.Data;
using System;

namespace Matchmaker.UserInterface.Intermediate
{
    public class PlayerIntermediate : IFormattable
    {
        public readonly Player player;

        public PlayerIntermediate(Player player)
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

        public Grade GradePrimary
        {
            get => player.GradePrimary;
            set => player.GradePrimary = value;
        }

        public Position PositionSecondary
        {
            get => player.PositionSecondary;
            set
            {
                player.PositionSecondary = value;
                if (value != Position.None && GradeSecondary == Grade.None) GradeSecondary = GradePrimary;
            }
        }

        public Grade GradeSecondary
        {
            get 
            {
                if (PositionSecondary == Position.None)
                    return Grade.None;
                return player.GradeSecondary; 
            }
            set
            {
                player.GradeSecondary = value;
                if (value == Grade.None) PositionSecondary = Position.None;
                else if (PositionSecondary == Position.None) PositionSecondary = PositionPrimary == Position.Second ? Position.Lead : Position.Second;
            }
        }

        public TeamSize PreferredTeamSizes
        {
            get => player.PreferredTeamSizes;
            set => player.PreferredTeamSizes = value;
        }

        public bool Visitor => player.Visitor;
    }
}
