using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BionicCode.BionicNuGetDeploy.Main.Settings
{
  public static class AppSettingsConnector
  {
    public static bool TryReadString(string key, out string value)
    {
      value = default;
      NameValueCollection appSettings = ConfigurationManager.AppSettings;
      if (appSettings[key] == null)
      {
        return false;
      }
      value = appSettings[key];
      return true;
    }

    public static bool TryReadInt(string key, out int value)
    {
      value = -1;
      NameValueCollection appSettings = ConfigurationManager.AppSettings;

      if (double.TryParse(appSettings[key], out double doubleValue))
      {
        value = Convert.ToInt32(doubleValue);
        return true;
      }

      return false;
    }

    public static bool TryReadDouble(string key, out double value)
    {
      NameValueCollection appSettings = ConfigurationManager.AppSettings;
      return double.TryParse(appSettings[key], out value);
    }

    public static bool TryReadBool(string key, out bool value)
    {
      NameValueCollection appSettings = ConfigurationManager.AppSettings;
      return bool.TryParse(appSettings[key], out value);
    }

    public static void WriteString(string key, string value)
    {
      AppSettingsConnector.AddUpdateAppSettings(key, value);
    }

    public static void WriteInt(string key, int value)
    {
      AppSettingsConnector.AddUpdateAppSettings(key, value);
    }

    public static void WriteDouble(string key, double value)
    {
      AppSettingsConnector.AddUpdateAppSettings(key, value);
    }

    public static void WriteBool(string key, bool value)
    {
      AppSettingsConnector.AddUpdateAppSettings(key, value);
    }


    private static void AddUpdateAppSettings<TValue>(string key, TValue value)
    {
      Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      KeyValueConfigurationCollection settings = configFile.AppSettings.Settings;
      if (settings[key] == null)
      {
        settings.Add(key, Convert.ToString(value));
      }
      else
      {
        settings[key].Value = Convert.ToString(value);
      }

      configFile.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
    }
  }
}
