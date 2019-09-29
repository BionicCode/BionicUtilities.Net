using System.Configuration;

namespace Hell.LogDown.Settings
{
  public interface IFileExplorerSettingsReader
  {
    TSection ReadSection<TSection>(string sectionName) where TSection : ConfigurationSection, new();
  }
}