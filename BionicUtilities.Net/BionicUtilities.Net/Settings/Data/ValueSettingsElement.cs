using System;
using System.Configuration;
using Hell.LogDown.Components.LogDocument.SyncModes;

namespace Hell.LogDown.Settings.Data
{
  public class ValueSettingsElement : ConfigurationElement
  {
    public ValueSettingsElement()
    {
      this.Value = string.Empty;
    }
    public ValueSettingsElement(string value)
    {
      this.Value = value;
    }

    [ConfigurationProperty("value", DefaultValue = "", IsRequired = false)]
    public string Value
    {
      get => (string) this["value"];
      set => this["value"] = value;
    }
  }
}
