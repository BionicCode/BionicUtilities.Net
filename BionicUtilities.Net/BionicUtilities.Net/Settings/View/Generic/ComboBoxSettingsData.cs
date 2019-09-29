using System;
using System.Collections.ObjectModel;
using Hell.GlobalRepository;

namespace Hell.LogDown.Settings.View.Generic
{
  public class ComboBoxSettingsData<TValue> : SettingsItemsPageData<TValue>, IComboBoxSettingsData
  {
    private EventHandler<ValueChangedEventArgs<object>> valueChanged;
    private readonly object syncLock = new object();

    #region Implementation of ISettingsPageData<object>

    event EventHandler<ValueChangedEventArgs<object>> ISettingsPageData<Object>.ValueChanged
    {
      add {
        lock (this.syncLock)
        {
          this.valueChanged += value;
        }
      }
      remove
      {
        EventHandler<ValueChangedEventArgs<object>> eventHandler = this.valueChanged;
        if (eventHandler != null)
        {
          lock (this.syncLock)
          {
            if (eventHandler != null)
            {
              eventHandler -= value;
            }
          }
        }
      }
    }
  
    object ISettingsPageData<object>.DisplaySettingValue
    {
      get => this.DisplaySettingValue;
      set
      {
        if (value is TValue newValue)
        {
          this.DisplaySettingValue = newValue;
          OnPropertyChanged();
          OnValueChanged();
        }
      }
    }
  
    object ISettingsPageData<object>.DefaultDisplaySettingValue
    {
      get => this.DefaultDisplaySettingValue;
      set
      {
        if (value is TValue newValue)
        {
          this.DefaultDisplaySettingValue = newValue;
          OnPropertyChanged();
          OnValueChanged();
        }
      }
    }

    #endregion

    #region Implementation of ISettingsItemsPageData<object>


    private ObservableCollection<object> displaySettingValues;

    /// <inheritdoc />
    public new event EventHandler<ValuesChangedEventArgs<object>> ValuesChanged;

    ObservableCollection<object> ISettingsItemsPageData<object>.DisplaySettingValues
    {
      get => this.displaySettingValues;
      set
      {
        this.displaySettingValues = value;

        OnValuesChanged();
        OnPropertyChanged();
      }
    }

    #endregion
  }
}
