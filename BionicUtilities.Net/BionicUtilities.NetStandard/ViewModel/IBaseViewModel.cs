#region Info
// //  
// BionicUtilities.NetStandard
#endregion

using System;
using BionicUtilities.NetStandard.Generic;

namespace BionicUtilities.NetStandard.ViewModel
{
  /// <summary>
  /// PropertyChanged event handler that supports standard property changed signature events with additional old value and new value parameters.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  /// <param name="sender">The event source.</param>
  /// <param name="propertyName">The name of the property that has changed.</param>
  /// <param name="oldValue">The value before the change.</param>
  /// <param name="newValue">The value after the change.</param>
  public delegate void PropertyValueChangedEventHandler<TValue>(
    object sender,
    PropertyValueChangedArgs<TValue> propertyChangedArgs);

  public interface IBaseViewModel : IViewModel
  {
    /// <summary>
    /// PropertyChanged implementation that sends old value and new value of the change and raises the INotifyPropertyChanged event.
    /// </summary>
    event PropertyValueChangedEventHandler<object> PropertyValueChanged;
  }

  public class PropertyValueChangedArgs<TValue>
  {
    public PropertyValueChangedArgs(string propertyName, TValue oldValue, TValue newValue)
    {
      this.PropertyName = propertyName;
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }

    public string PropertyName { get; } 
    public TValue OldValue { get; } 
    public TValue NewValue { get; } 
  }
}