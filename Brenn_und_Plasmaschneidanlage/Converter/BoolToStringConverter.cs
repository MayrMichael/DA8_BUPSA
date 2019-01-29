using System;
using System.Windows.Data;
using System.Globalization;

namespace Brenn_und_Plasmaschneidanlage.Converter
{
    class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object Convert(object value , Type targetType , object parameter , CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value == true)
                    return TrueValue;
                else
                    return FalseValue;
            return FalseValue;
        }

        public object ConvertBack(object value , Type targetType , object parameter , CultureInfo culture)
        {
            if (value != null)
                if (value.ToString() == TrueValue)
                    return true;
                else
                    return false;
            else
                return null;
        }
    }
}
