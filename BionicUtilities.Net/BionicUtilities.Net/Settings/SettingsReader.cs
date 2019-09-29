using System.Configuration;

namespace Hell.LogDown.Settings
{
  /// <summary>
  /// An abstract writer base class that knows how to perform write operations to the settings file.<para/>
  /// 
  /// </summary>
  /// <typeparam name="TRootSection">The section type this writer is responsible for. Must extend <see cref="ConfigurationSection"/> of</typeparam>
  public abstract class SettingsReader<TRootSection> : ISettingsReader<TRootSection> where TRootSection : ConfigurationSection, new()
  {
    protected SettingsReader(string rootSectionName, string rootSectionGroupName)
    {
      //Configuration appConfig = GetApplicationConfiguration();
      //AddInitialSectionGroup(appConfig, rootSectionGroupName);
      //AddInitialSection(rootSectionGroupName, rootSectionName);
    }

    private void AddInitialSectionGroup(Configuration appConfig, string sectionGroupName)
    {
      this.RootSectionGroup = appConfig.GetSectionGroup(sectionGroupName);

      // Add a new ConfigurationSectionGroup with the requested name only if it doesn't exist
      if (this.RootSectionGroup == null)
      {
        this.RootSectionGroup = new ConfigurationSectionGroup();
        appConfig.SectionGroups.Add(sectionGroupName, this.RootSectionGroup);
        appConfig.Save(ConfigurationSaveMode.Full);
      }
    }

    private void AddInitialSection(string sectionGroupName, string sectionName, ConfigurationSection newSection = null)
    {
      var appConfig = GetApplicationConfiguration();
      ConfigurationSectionGroup sectionGroup = appConfig.GetSectionGroup(sectionGroupName);

      // Add a new ConfigurationSection with the requested name only if it doesn't exist
      this.RootSection = sectionGroup?.Sections[sectionName] as TRootSection;
      if (this.RootSection == null)
      {
        this.RootSection = new TRootSection();
        sectionGroup?.Sections.Add(sectionName, this.RootSection);
        appConfig.Save(ConfigurationSaveMode.Full);
        ConfigurationManager.RefreshSection(sectionName);
      }
    }
    
    protected virtual Configuration GetApplicationConfiguration(ConfigurationUserLevel configurationUserLevel = ConfigurationUserLevel.None) => ConfigurationManager.OpenExeConfiguration(configurationUserLevel) as Configuration;

    public abstract TSection ReadSection<TSection>(string sectionName) where TSection : ConfigurationSection, new();
    public abstract bool TryReadSection<TSubGroupSection>(string sectionName, out TSubGroupSection sectionResult) where TSubGroupSection : ConfigurationSection, new();
    public abstract bool TryReadSection<TSubGroupSection>(string sectionGroupName, string sectionName, out TSubGroupSection sectionResult) where TSubGroupSection : ConfigurationSection, new();
    public abstract bool TryReadSectionGroup(string sectionGroupName, out ConfigurationSectionGroup sectiongroupResult);

    protected TRootSection RootSection { get; set; }
    protected ConfigurationSectionGroup RootSectionGroup { get; set; }
  }
}
