#region Info
// //  
// WpfTestRange.Main
#endregion

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BionicUtilities.NetStandard.ViewModel
{
  public interface IViewModel : INotifyPropertyChanged
  {
    bool TrySetValue<TValue>(TValue value, ref TValue targetBackingField, [CallerMemberName] string propertyName = null);
  }
}