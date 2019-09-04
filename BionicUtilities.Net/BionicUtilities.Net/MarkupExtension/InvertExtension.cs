using System;
using System.Windows;
using System.Windows.Markup;

namespace BionicLibrary.Net.MarkupExtension
{
  [MarkupExtensionReturnType(typeof(object))]
  public class InvertExtension : System.Windows.Markup.MarkupExtension
  {
    public InvertExtension()
    {
      this.Value = null;
    }

    public InvertExtension(object value)
    {
      if (value is double)
      {
        var doubleValue = (double)value * -1;
        this.Value = doubleValue;
      }

      if (value is int)
      {
        var intValue = (int)value * -1;
        this.Value = intValue;
      }

      if (value is bool)
      {
        var booleanValue = (bool)value ^ true;
        this.Value = booleanValue;
      }

      if (value is Visibility visibilityValue)
      {
        Visibility booleanValue = visibilityValue.Equals(Visibility.Hidden) || visibilityValue.Equals(Visibility.Collapsed)
          ? Visibility.Visible
          : Visibility.Collapsed;
        this.Value = booleanValue;
      }
    }

    #region Overrides of MarkupExtension

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this.Value;
    }

    #endregion

    public object Value { get; set; }
  }
}