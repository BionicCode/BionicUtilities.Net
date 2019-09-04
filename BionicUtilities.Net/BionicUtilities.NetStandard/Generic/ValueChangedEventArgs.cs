#region Info
// //  
// WpfTestRange.Main
#endregion

using System;

namespace BionicUtilities.NetStandard.Generic
{
public class ValueChangedEventArgs<TValue> : EventArgs
{
  public ValueChangedEventArgs(TValue newValue, TValue oldValue)
  {
    this.NewValue = newValue;
    this.OldValue = oldValue;
  }

  public TValue NewValue { get; set; }
  public TValue OldValue { get; set; }
}
}