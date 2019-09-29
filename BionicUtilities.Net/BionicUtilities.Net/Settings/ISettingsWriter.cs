using System;
using System.Configuration;

namespace Hell.LogDown.Settings
{
  public interface ISettingsWriter<in TRootSection> where TRootSection : ConfigurationSection, new()
  {
    void AddSection(string sectionName, Configuration applicationConfiguration, string sectionGroupName = null);
    void AddSection(string sectionName, string sectionGroupName, Configuration applicationConfiguration, ConfigurationSectionGroup newSection = null);
    void AddSection(string sectionName, TRootSection newSection, Configuration applicationConfiguration, string sectionGroupName = null);
    void AddSection<TParentSection>(string sectionName, TParentSection newSection, Configuration applicationConfiguration, string sectionGroupName = null) where TParentSection : ConfigurationSection, new();
    void AddSectionGroup(string sectionGroupname, Configuration appConfig, ConfigurationSectionGroup newSectionGroup = null);
    ConfigurationSectionGroup CreateSubSectionGroup(string sectionGroupName, string parenetSectionGroupName, Configuration applicationConfiguration, ConfigurationSectionGroup sectionGroup = null);
    void WriteSection(TRootSection section);
    event EventHandler<DataChangedEventArgs> SettingsChanged;

    void SaveSection(Configuration configuration,
      string sectionName = null,
      ConfigurationSaveMode configurationSaveMode = ConfigurationSaveMode.Full);
  }
}