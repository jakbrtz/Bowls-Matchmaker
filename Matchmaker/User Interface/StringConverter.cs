using Matchmaker.Data;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Matchmaker.UserInterface
{
    public static class StringConverter
    {
        public static void AddAttributesToEnumsAndStructs()
        {
            TypeDescriptor.AddAttributes(typeof(PositionPreference), new TypeConverterAttribute(typeof(PositionPreferenceConverter)));
            TypeDescriptor.AddAttributes(typeof(Position), new TypeConverterAttribute(typeof(PositionConverter)));
            TypeDescriptor.AddAttributes(typeof(Grade), new TypeConverterAttribute(typeof(GradeConverter)));
            TypeDescriptor.AddAttributes(typeof(TeamSize), new TypeConverterAttribute(typeof(TeamSizeConverter)));
        }
    }

    abstract class StringConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => StringToEnum(value.ToString());
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => EnumToString((T)value);
        public abstract T StringToEnum(string value);
        public virtual string EnumToString(T value) => value.ToString();
    }

    class PositionPreferenceConverter : StringConverter<PositionPreference>
    {
        public override PositionPreference StringToEnum(string value) => Enums.ParsePositionPreference(value);
    }

    class PositionConverter : StringConverter<Position>
    {
        public override Position StringToEnum(string value) => Enums.ParsePosition(value);
        public override string EnumToString(Position value) => Enums.ToUserFriendlyString(value);
    }

    class GradeConverter : StringConverter<Grade>
    {
        public override Grade StringToEnum(string value) => Enums.ParseGrade(value);
        public override string EnumToString(Grade value) => Enums.ToUserFriendlyString(value);
    }

    class TeamSizeConverter : StringConverter<TeamSize>
    {
        public override TeamSize StringToEnum(string value) => Enums.ParseTeamSize(value);
        public override string EnumToString(TeamSize value) => Enums.ToUserFriendlyString(value);
    }
}
