using Matchmaker.Data;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Matchmaker.UserInterface.StringConverters
{
    public static partial class EnumStringConverter
    {
        public static void AddAttributesToEnumsAndStructs()
        {
            TypeDescriptor.AddAttributes(typeof(PositionPreference), new TypeConverterAttribute(typeof(PositionPreferenceConverter)));
            TypeDescriptor.AddAttributes(typeof(Position), new TypeConverterAttribute(typeof(PositionConverter)));
            TypeDescriptor.AddAttributes(typeof(Grade), new TypeConverterAttribute(typeof(GradeConverter)));
            TypeDescriptor.AddAttributes(typeof(TeamSize), new TypeConverterAttribute(typeof(TeamSizeConverter)));
        }
    }

    abstract class EnumStringConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => StringToEnum(value.ToString());
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => EnumToString((T)value);
        public abstract T StringToEnum(string value);
        public virtual string EnumToString(T value) => value.ToString();
    }
}
