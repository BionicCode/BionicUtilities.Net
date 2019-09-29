using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Hell.GlobalRepository;
using Hell.GlobalRepository.LogFileAbstraction;
using Hell.LogDown.Settings.View.Generic;

namespace Hell.LogDown.Settings
{
  public static class SettingsController
  {
    #region Constructors

    static SettingsController()
    {
      SettingsController.RegisteredSettings = new Dictionary<Guid, string>();
    }

    #endregion

    #region API

    public static void RegisterSetting<TValue>(ISettingsPageData<TValue> setting, string propertyNameOfApplicationSettingsManager, BindingMode mode = BindingMode.TwoWay, bool reportChanges = true)
    {
      if (!SettingsController.RegisteredSettings.ContainsKey(setting.Guid))
      {
        SettingsController.RegisteredSettings.Add(setting.Guid, propertyNameOfApplicationSettingsManager);
      }

      if (mode == BindingMode.TwoWay || mode == BindingMode.Default || mode == BindingMode.OneWay)
      {
        ApplicationSettingsManager.Instance.PropertyChanged += (s, e) => SettingsController.UpdateSetting(e.PropertyName, setting);
      }

      if (mode == BindingMode.TwoWay || mode == BindingMode.Default || mode == BindingMode.OneWayToSource)
      {
        setting.ValueChanged += (s, e) => SettingsController.UpdateSettingsSource(e, setting.Guid, reportChanges);
      }
    }

    public static void RegisterItemsSetting<TValue>(ISettingsItemsPageData<TValue> setting, string propertyNameOfApplicationSettingsManager, BindingMode mode = BindingMode.TwoWay, bool reportChanges = true)
    {
      if (!SettingsController.RegisteredSettings.ContainsKey(setting.Guid))
      {
        SettingsController.RegisteredSettings.Add(setting.Guid, propertyNameOfApplicationSettingsManager);
      }

      if (mode == BindingMode.TwoWay || mode == BindingMode.Default || mode == BindingMode.OneWay)
      {
        ApplicationSettingsManager.Instance.PropertyChanged += (s, e) => SettingsController.UpdateItemsSetting(e.PropertyName, setting);
      }

      if (mode == BindingMode.TwoWay || mode == BindingMode.Default || mode == BindingMode.OneWayToSource)
      {
        setting.ValuesChanged += (s, e) => SettingsController.UpdateSettingItemsSource(e, setting.Guid, reportChanges);
      }
    }

    private static void UpdateSettingItemsSource<TValue>(ValuesChangedEventArgs<TValue> newValues, Guid settingGuid, bool reportChanges)
    {
      if (newValues == null)
      {
        return;
      }
      if (reportChanges)
      {
        // Set property directly
        PropertyInfo settingsPropertyInfo = SettingsController.GetSettingsPropertySource(SettingsController.RegisteredSettings[settingGuid]);
        settingsPropertyInfo.SetValue(ApplicationSettingsManager.Instance, newValues.Values);
      }
      else
      {
        // Invoke property setter method
        MethodInfo settingsSetterMethodInfo =
          SettingsController.GetSettingsMethodSource(SettingsController.RegisteredSettings[settingGuid]);
        settingsSetterMethodInfo.Invoke(ApplicationSettingsManager.Instance, new object[] {newValues, false});
      }
    }

    private static void UpdateSettingsSource<TValue>(ValueChangedEventArgs<TValue> newValue, Guid settingGuid, bool reportChanges)
    {
      if (newValue == null)
      {
        return;
      }
      if (reportChanges)
      {
        // Set property directly
        PropertyInfo settingsPropertyInfo = SettingsController.GetSettingsPropertySource(SettingsController.RegisteredSettings[settingGuid]);
        var oldPropertyValue = settingsPropertyInfo.GetValue(ApplicationSettingsManager.Instance);

        if (oldPropertyValue != null && !newValue.Value.Equals(oldPropertyValue) || oldPropertyValue == null)
        {
          settingsPropertyInfo.SetValue(ApplicationSettingsManager.Instance, newValue.Value);
        }
      }
      else
      {
        // Invoke property setter method
        MethodInfo settingsSetterMethodInfo =
          SettingsController.GetSettingsMethodSource(SettingsController.RegisteredSettings[settingGuid]);
        settingsSetterMethodInfo.Invoke(ApplicationSettingsManager.Instance, new object[] {newValue.Value, false});
      }
    }

    private static MethodInfo GetSettingsMethodSource(string settingsPropertyName)
    {
      string settingsSetterMethodName = "Set" + settingsPropertyName;
      return ApplicationSettingsManager.Instance.GetType().GetMethod(
        settingsSetterMethodName, BindingFlags.Public | BindingFlags.Instance);
    }

    private static void UpdateSetting<TValue>(string propertyName, ISettingsPageData<TValue> setting)
    {
      if (!SettingsController.RegisteredSettings[setting.Guid].Equals(propertyName, StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      var sourcePropertyValue = (TValue) SettingsController.GetSettingsPropertySource(propertyName).GetValue(ApplicationSettingsManager.Instance);
      if (!sourcePropertyValue.Equals(setting.DisplaySettingValue))
      {
        setting.DisplaySettingValue = sourcePropertyValue;
      }
    }

    private static void UpdateItemsSetting<TValue>(string propertyName, ISettingsItemsPageData<TValue> setting)
    {
      if (!SettingsController.RegisteredSettings[setting.Guid].Equals(propertyName, StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      var sourcePropertyValue = (ObservableCollection<TValue>) SettingsController.GetSettingsPropertySource(propertyName).GetValue(ApplicationSettingsManager.Instance);
      if (!sourcePropertyValue.Equals(setting.DisplaySettingValues))
      {
        setting.DisplaySettingValues = sourcePropertyValue;
      }
    }

    #endregion

    private static PropertyInfo GetSettingsPropertySource(string propertyNameOfApplicationSettingsManager)
    {
      return ApplicationSettingsManager.Instance.GetType().GetProperty(
        propertyNameOfApplicationSettingsManager, BindingFlags.GetProperty |
        BindingFlags.Public | BindingFlags.Instance);
    }

    private static Dictionary<Guid, string> RegisteredSettings { get; set; }
  }
}
