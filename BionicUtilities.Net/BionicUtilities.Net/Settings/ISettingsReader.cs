using System.Configuration;

namespace Hell.LogDown.Settings
{
  public interface ISettingsReader<out TRootSection> where TRootSection : ConfigurationSection
  {
    bool TryReadSection<TSubGroupSection>(string sectionName, out TSubGroupSection sectionResult) where TSubGroupSection : ConfigurationSection, new();
    bool TryReadSection<TSubGroupSection>(string sectionGroupName, string sectionName, out TSubGroupSection sectionResult) where TSubGroupSection : ConfigurationSection, new();
    bool TryReadSectionGroup(string sectionGroupName, out ConfigurationSectionGroup sectiongroupResult);
    TSection ReadSection<TSection>(string sectionName) where TSection : ConfigurationSection, new();
  }
}