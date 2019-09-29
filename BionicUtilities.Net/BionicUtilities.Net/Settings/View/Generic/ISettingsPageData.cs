using System;
using Hell.GlobalRepository;

namespace Hell.LogDown.Settings.View.Generic
{
  public interface ISettingsPageData<TValue> : ISettingsPageData
  {
    event EventHandler<ValueChangedEventArgs<TValue>> ValueChanged;
    TValue DisplaySettingValue { get; set; }
    TValue DefaultDisplaySettingValue { get; set; }
  }
}
