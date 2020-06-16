#region Info

// //  
// BionicCode.BionicUtilities.Net.Core.Wpf

#endregion

namespace BionicCode.BionicUtilities.NetStandard
{
  public interface IValueInverter
  {
    bool TryInvertValue(object value, out object invertedValue);
    object InvertValue(object value);
  }
}