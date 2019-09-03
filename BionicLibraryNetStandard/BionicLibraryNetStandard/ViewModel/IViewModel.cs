#region Info
// //  
// WpfTestRange.Main
#endregion

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library
{
  public interface IViewModel : INotifyPropertyChanged
  {
    bool TrySetValue<TValue>(TValue value, ref TValue targetBackingField, [CallerMemberName] string propertyName = null);
  }
}