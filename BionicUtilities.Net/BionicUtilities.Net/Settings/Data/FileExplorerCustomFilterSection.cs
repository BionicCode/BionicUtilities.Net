using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hell.LogDown.Settings.Data
{
  public class FileExplorerCustomFilterSection : ConfigurationSection
  {
    public FileExplorerCustomFilterSection()
    {
      base["filters"] = string.Empty;
    }

    public FileExplorerCustomFilterSection(string filters)
    {
      base["filters"] = filters;
    }

    [ConfigurationProperty("isCustomFilterEnabled", DefaultValue = false)]
    public bool IsCustomFilterEnabled
    {
      get => (bool) base["isCustomFilterEnabled"];
      set => base["isCustomFilterEnabled"] = value;
    }

    [ConfigurationProperty("filters", DefaultValue = "*")]
    public string Filters { get => (string) base["filters"]; set => base["filters"] = value; }
  }

  public class FileExplorerDefaultFilterSection : ConfigurationSection
  {
    public FileExplorerDefaultFilterSection()
    {
      base["filters"] = new KeyValueConfigurationCollection();
    }

    [ConfigurationProperty("filters", IsDefaultCollection = true)]
    [ConfigurationCollection(typeof(KeyValueConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
    public KeyValueConfigurationCollection Filters => base["filters"] as KeyValueConfigurationCollection;
  }
}
