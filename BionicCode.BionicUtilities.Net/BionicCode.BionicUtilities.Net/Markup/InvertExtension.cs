using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BionicCode.BionicUtilities.Net.Core.Wpf.Markup
{
  public class BindingResolver : FrameworkElement
  {
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value",
      typeof(object),
      typeof(BindingResolver),
      new PropertyMetadata(default(object)));

    public object Value
    {
      get => GetValue(BindingResolver.ValueProperty);
      set => SetValue(BindingResolver.ValueProperty, value);
    }
  }

  public class InvertExtension : MarkupExtension
  {
    public object Value { get; }
    private BindingResolver BindingResolver { get; }
    private MarkupExtension WrappedExtension { get; }

    public InvertExtension() => this.BindingResolver = new BindingResolver();

    public InvertExtension(MarkupExtension extension) : this() => this.WrappedExtension = extension;

    public InvertExtension(object value) : this() => this.Value = value;

    #region Overrides of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      object valueToInvert;
      if (this.Value is MarkupExtension wrappedMarkupExtension)
      {
        valueToInvert = GetValueToInvertFromMarkupExtension(wrappedMarkupExtension, serviceProvider);
      }
      else
      {
        valueToInvert = this.Value;
      }

      var provideValueTargetService = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
      Type targetPropertyType = (provideValueTargetService.TargetProperty as DependencyProperty).PropertyType;

      return TryInvertValue(valueToInvert, out object invertedValue)
        ? targetPropertyType.Equals(typeof(string))
          ? invertedValue.ToString()
          : invertedValue
        : valueToInvert;
    }

    protected object GetValueToInvertFromMarkupExtension(MarkupExtension wrappedMarkupExtension,
      IServiceProvider serviceProvider)
    {
      object wrappedExtensionValue = wrappedMarkupExtension.ProvideValue(serviceProvider);
      if (wrappedExtensionValue is BindingExpression bindingExpression)
      {
        var provideValueTargetService = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        object targetObject = provideValueTargetService?.TargetObject;
        this.BindingResolver.DataContext = (targetObject as FrameworkElement)?.DataContext ?? targetObject;
        BindingOperations.SetBinding(
          this.BindingResolver,
          BindingResolver.ValueProperty,
          bindingExpression.ParentBinding);
        return this.BindingResolver.Value;
      }

      return wrappedExtensionValue;
    }

    private bool TryInvertValue(object value, out object invertedValue)
    {
      if (TryConvertStringToNumber(value, out object number))
      {
        value = number;
      }

      switch (value)
      {
        case bool boolValue:
          invertedValue = !boolValue;
          break;
        case int intValue:
          invertedValue = intValue * -1;
          break;
        case decimal decimalValue:
          invertedValue = decimalValue * decimal.MinusOne;
          break;
        case double doubleValue:
          invertedValue = double.IsNaN(doubleValue)
            ? doubleValue
            : double.IsNegativeInfinity(doubleValue)
              ? double.PositiveInfinity
              : double.IsPositiveInfinity(doubleValue)
                ? double.NegativeInfinity
                : doubleValue * -1.0;
          break;
        case float floatValue:
          invertedValue = float.IsNaN(floatValue)
            ? floatValue
            : float.IsNegativeInfinity(floatValue)
              ? float.PositiveInfinity
              : float.IsPositiveInfinity(floatValue)
                ? float.NegativeInfinity
                : floatValue * -1.0;
          break;
        case byte byteValue:
          invertedValue = ~byteValue;
          break;
        case Visibility visibilityValue:
          invertedValue = visibilityValue.Equals(Visibility.Hidden) || visibilityValue.Equals(Visibility.Collapsed)
            ? Visibility.Visible
            : Visibility.Collapsed;
          break;
        default:
          invertedValue = value;
          break;
      }

      return invertedValue != value;
    }

    private bool TryConvertStringToNumber(object value, out object numericValue)
    {
      numericValue = null;
      if (!(value is string stringValue))
      {
        return false;
      }

      if (int.TryParse(stringValue, out int integer))
      {
        numericValue = integer;
      }
      else
      {
        if (double.TryParse(stringValue, out double doubleValue))
        {
          numericValue = doubleValue;
        }
      }

      return numericValue != null;
    }

    #endregion
  }
}