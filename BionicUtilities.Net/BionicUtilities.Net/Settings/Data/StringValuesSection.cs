using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hell.LogDown.Settings.Data
{
  public class StringValuesSection : ConfigurationSection
  {
    public StringValuesSection()
    {
    }

    public StringValuesSection(IEnumerable<string> stringConfigurationElements)
    {
      base["autoOpenFiles"] = new ValueSettingsElementCollection(stringConfigurationElements.Select((stringValue) => new ValueSettingsElement(stringValue)));
    }

    [ConfigurationProperty("autoOpenFiles", IsDefaultCollection = true)]
    [ConfigurationCollection(typeof(ValueSettingsElementCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
    public ValueSettingsElementCollection AutoOpenFiles
    {
      get => base["autoOpenFiles"] as ValueSettingsElementCollection;
      set => base["autoOpenFiles"] = value;
    }
  }
}
