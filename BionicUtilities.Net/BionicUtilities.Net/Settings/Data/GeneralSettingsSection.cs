using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hell.LogDown.Settings.Data
{
    public class GeneralSettingsSection : ConfigurationSection
  {
    public GeneralSettingsSection()
    {
      //this.AutoOpenFiles = new 
    }

    public GeneralSettingsSection(NameValueConfigurationCollection entries)
    {
      base["entries"] = entries;
    }

    [ConfigurationProperty("entries", IsDefaultCollection = true, IsRequired = true)]
    [ConfigurationCollection(typeof(NameValueConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
    public NameValueConfigurationCollection Entries => base["entries"] as NameValueConfigurationCollection;
  }
}
