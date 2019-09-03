using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace BionicLibrary.Net.Converter
{
  class BooleanMultiValueConverter : IMultiValueConverter
  {
    #region Implementation of IMultiValueConverter

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values?.OfType<bool>().All((value) => value);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}
