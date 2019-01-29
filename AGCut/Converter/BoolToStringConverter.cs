using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AGCut.Converter
{
    [ValueConversion(typeof(Boolean),typeof(String))]
    class BoolToStringConverter : IValueConverter
    {

        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value == true)
                    return TrueValue;
                else
                    return FalseValue;
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
