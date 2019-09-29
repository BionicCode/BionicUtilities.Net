using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hell.GlobalRepository;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings.View.Generic
{
  public abstract class SettingsPageData<TValue> : ISettingsPageData<TValue>, ISettingsPageData 
  {
    protected SettingsPageData()
    {
      this.Guid = Guid.NewGuid();
      this.DisplaySettingValue = default(TValue);
    }

    #region Implementation of ISettingsPageData<TValue>

    public event EventHandler<ValueChangedEventArgs<TValue>> ValueChanged;
    protected virtual void OnValueChanged()
    {
      this.ValueChanged?.Invoke(this, new ValueChangedEventArgs<TValue>(this.DisplaySettingValue));
    }

    private Guid guid;   
    public Guid Guid
    {
      get => this.guid;
      private set 
      { 
        this.guid = value; 
        OnPropertyChanged();
      }
    }

    private string titel;
    public string Title
    {
      get => this.titel;
      set
      {
        if (value == this.titel)
        { return; }
        this.titel = value;
        OnPropertyChanged();
      }
    }

    private string description;
    public string Description
    {
      get => this.description;
      set
      {
        if (value == this.Description)
        { return; }
        this.description = value;
        OnPropertyChanged();
      }
    }

    private string displaySettingKey;
    public string DisplaySettingKey
    {
      get => this.displaySettingKey;
      set
      {
        if (value == this.displaySettingKey)
        { return; }
        this.displaySettingKey = value;
        OnPropertyChanged();
      }
    }

    private TValue displaySettingValue;
    public TValue DisplaySettingValue
    {
      get => this.displaySettingValue;
      set
      {
        if (object.Equals(value, this.displaySettingValue))
        {
          return;
        }

        this.displaySettingValue = value;
        OnPropertyChanged();
        OnValueChanged();
      }
    }

    private TValue defaultDisplaySettingValue;
    public TValue DefaultDisplaySettingValue
    {
      get => this.defaultDisplaySettingValue;
      set
      {
        if (object.Equals(value, this.defaultDisplaySettingValue))
        {
          return;
        }

        this.defaultDisplaySettingValue = value;
        OnPropertyChanged();
      }
    }

    private bool affectsPerformance;
    public bool AffectsPerformance
    {
      get => this.affectsPerformance;
      set
      {
        if (value == this.affectsPerformance)
        { return; }
        this.affectsPerformance = value;
        OnPropertyChanged();
      }
    }

    #endregion

    #region Implementation of INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
