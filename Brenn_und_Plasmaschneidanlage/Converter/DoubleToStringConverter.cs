using System;
using System.Globalization;
using System.Windows.Data;

namespace Brenn_und_Plasmaschneidanlage.Converter
{
    [ValueConversion(typeof(Double),typeof(String))]
    class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value , Type targetType , object parameter , CultureInfo culture)
        {
            string param = parameter as string;
            if (param != null)
                return ((double)value).ToString(param);
            else
                throw new InvalidCastException();
        }

        public object ConvertBack(object value , Type targetType , object parameter , CultureInfo culture)
        {
            if (Double.TryParse(value as string , out double amount))
                return amount;
            else
                throw new InvalidCastException();
        }
    }
}
