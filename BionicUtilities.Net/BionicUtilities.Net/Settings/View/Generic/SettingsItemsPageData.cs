using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Hell.GlobalRepository;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings.View.Generic
{
  public abstract class SettingsItemsPageData<TValue> : SettingsPageData<TValue>, ISettingsItemsPageData<TValue>
  {
    protected SettingsItemsPageData() : base()
    {
      this.DisplaySettingValues = new ObservableCollection<TValue>();
    }

    #region Implementation of INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    private ObservableCollection<TValue> displaySettingValues;
    public ObservableCollection<TValue> DisplaySettingValues
    {
      get => this.displaySettingValues;
      set
      {
        if (this.DisplaySettingValues != null)
        {
          this.DisplaySettingValues.CollectionChanged -= NotifyValueChanged;
          this.DisplaySettingValues.ToList().ForEach((underlyingValue) =>
          {
            if (underlyingValue is INotifyPropertyChanged propertyChangedValue)
            {
              propertyChangedValue.PropertyChanged -= OnUnderlyingValuesChanged;
            }
            else if (underlyingValue is INotifyCollectionChanged collectionChangedValue)
            {
              collectionChangedValue.CollectionChanged -= OnUnderlyingValuesChanged;
            }
          });
        }

        this.displaySettingValues = value;
        if (this.DisplaySettingValues != null)
        {
          this.DisplaySettingValues.CollectionChanged += NotifyValueChanged;
          this.DisplaySettingValues.ToList().ForEach((underlyingValue) =>
          {
            if (underlyingValue is INotifyPropertyChanged propertyChangedValue)
            {
              propertyChangedValue.PropertyChanged += OnUnderlyingValuesChanged;
            }
            else if (underlyingValue is INotifyCollectionChanged collectionChangedValue)
            {
              collectionChangedValue.CollectionChanged += OnUnderlyingValuesChanged;
            }
          });
        }

        OnValuesChanged();
        OnPropertyChanged();
      }
    }

    private void OnUnderlyingValuesChanged<TArgs>(object sender, TArgs e)
    {
      OnValuesChanged();
    }

    private void NotifyValueChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnValuesChanged();
    }

    protected virtual void OnValuesChanged()
    {
      this.ValuesChanged?.Invoke(this, new ValuesChangedEventArgs<TValue>(this.DisplaySettingValues));
    }

    public event EventHandler<ValuesChangedEventArgs<TValue>> ValuesChanged;

  }
}