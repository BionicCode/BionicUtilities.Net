using System;
using System.Collections.ObjectModel;
using Hell.GlobalRepository;

namespace Hell.LogDown.Settings.View.Generic
{
  public interface ISettingsItemsPageData<TValue> : ISettingsPageData<TValue>
  {
    event EventHandler<ValuesChangedEventArgs<TValue>> ValuesChanged;
    ObservableCollection<TValue> DisplaySettingValues { get; set; }
  }
}