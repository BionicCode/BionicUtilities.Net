using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hell.LogDown.Settings.Data
{
  public class SynchronizationSettingsSection : ConfigurationSection
  {
    public SynchronizationSettingsSection()
    {
      //this.AutoOpenFiles = new 
    }

    [ConfigurationProperty("fileMerge", IsDefaultCollection = true, IsRequired = true)]
    [ConfigurationCollection(
      typeof(NameValueConfigurationCollection),
      AddItemName = "add",
      ClearItemsName = "clear",
      RemoveItemName = "remove")]
    public NameValueConfigurationCollection FileMerge => base["fileMerge"] as NameValueConfigurationCollection;

    [ConfigurationProperty("mergedDocumentColors", IsDefaultCollection = true, IsRequired = true)]
    [ConfigurationCollection(
      typeof(ColorElementCollection),
      AddItemName = "add",
      ClearItemsName = "clear",
      RemoveItemName = "remove")]
    public ColorElementCollection MergedDocumentColors => base["mergedDocumentColors"] as ColorElementCollection;

    [ConfigurationProperty("general", IsDefaultCollection = true, IsRequired = true)]
    [ConfigurationCollection(
      typeof(NameValueConfigurationCollection),
      AddItemName = "add",
      ClearItemsName = "clear",
      RemoveItemName = "remove")]
    public NameValueConfigurationCollection General => base["general"] as NameValueConfigurationCollection;
  }
}
