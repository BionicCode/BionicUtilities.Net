using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace BionicLibrary.Net.Converter
{
  [ValueConversion(typeof(double), typeof(bool))]
  class IsGreaterThanValueConverter : IValueConverter
  {
    #region Implementation of IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool convert = Comparer.Default.Compare(value, parameter) > 0;
      return convert;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
      {
        return Binding.DoNothing;
      }

      if (double.TryParse(value.ToString(), out double numericValue) && double.TryParse(parameter.ToString(), out double numericReferenceValue))
      {
        return (double) value < (double) parameter;
      }

      return Binding.DoNothing;
    }

    #endregion
  }
}
