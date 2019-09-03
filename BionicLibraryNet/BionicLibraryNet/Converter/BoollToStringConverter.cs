using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BionicLibrary.Net.Converter
{
  [ValueConversion(typeof(bool), typeof(string))]
  class BoollToStringConverter : DependencyObject, IValueConverter
  {
    public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(
      "TrueValue",
      typeof(string),
      typeof(BoollToStringConverter),
      new PropertyMetadata("True"));

    public string TrueValue
    {
      get => (string) GetValue(BoollToStringConverter.TrueValueProperty);
      set => SetValue(BoollToStringConverter.TrueValueProperty, value);
    }

    public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(
      "FalseValue",
      typeof(string),
      typeof(BoollToStringConverter),
      new PropertyMetadata("False"));

    public string FalseValue
    {
      get => (string) GetValue(BoollToStringConverter.FalseValueProperty);
      set => SetValue(BoollToStringConverter.FalseValueProperty, value);
    }


    public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(
      "NullValue",
      typeof(string),
      typeof(BoollToStringConverter),
      new PropertyMetadata("Unset"));

    public string NullValue { get { return (string) GetValue(BoollToStringConverter.NullValueProperty); } set { SetValue(BoollToStringConverter.NullValueProperty, value); } }


    #region Implementation of IValueConverter

    public object Convert(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (value is bool isTrue)
      {
        return isTrue
            ? this.TrueValue
            : this.FalseValue;
      }

      return this.NullValue;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return (value as string)?.Equals(this.TrueValue, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    #endregion
  }
}