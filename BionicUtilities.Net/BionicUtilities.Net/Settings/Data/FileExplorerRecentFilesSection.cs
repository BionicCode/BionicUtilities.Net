using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Hell.LogDown.Settings.Data
{
  public class FileExplorerRecentFilesSection : ConfigurationSection
  {
    public FileExplorerRecentFilesSection()
    {
      //this.AutoOpenFiles = new 
    }

    public FileExplorerRecentFilesSection(IEnumerable<FileInfo> recentFileInfos)
    {
      base["recentFiles"] = new FileElementConfigurationElementCollection(recentFileInfos.Select((fileInfo) => new FileElement(fileInfo)));
    }

    [ConfigurationProperty("maxFiles", DefaultValue = 15)]
    [IntegerValidator(MinValue = 0, MaxValue = int.MaxValue, ExcludeRange = false)]
    public int MaxFiles { get { return (int) base["maxFiles"]; } set { base["maxFiles"] = value; } }

    [ConfigurationProperty("isDeleteTemporaryFilesEnabled", DefaultValue = true)]
    public bool IsDeleteTemporaryFilesEnabled { get { return (bool) this["isDeleteTemporaryFilesEnabled"]; } set { this["isDeleteTemporaryFilesEnabled"] = value; } }

    [ConfigurationProperty("recentFiles", IsDefaultCollection = false)]
    [ConfigurationCollection(typeof(FileElementConfigurationElementCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
    public new FileElementConfigurationElementCollection RecentFiles => base["recentFiles"] as FileElementConfigurationElementCollection;
  }
} 
