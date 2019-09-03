using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BionicLibrary.Net.Converter
{
  [ValueConversion(typeof(object), typeof(object))]
  class InvertValueConverter : IValueConverter
  {
    #region Implementation of IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double)
      {
        return (double)value * -1;
      }

      if (value is int)
      {
        return (int)value * -1;
      }

      if (value is bool)
      {
        return (bool)value ^ true;
      }

      if (value is Visibility)
      {
        Visibility visibilityValue = (Visibility)value;
        return visibilityValue.Equals(Visibility.Hidden) || visibilityValue.Equals(Visibility.Collapsed)
          ? Visibility.Visible
          : Visibility.Collapsed;
      }

      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert(value, targetType, parameter, culture);
    }

    #endregion
  }
}
