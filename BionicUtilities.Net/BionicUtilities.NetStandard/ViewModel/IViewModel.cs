#region Info
// //  
// WpfTestRange.Main
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BionicUtilities.NetStandard.ViewModel
{
  public interface IViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
  {
    bool PropertyHasError([CallerMemberName] string propertyName = null);
    IEnumerable<string> GetPropertyErrors([CallerMemberName] string propertyName = null);
  }
}