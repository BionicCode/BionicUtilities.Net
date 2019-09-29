using System;
using System.Configuration;
using Hell.LogDown.Settings.Data;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings
{
  /// <summary>
  /// An abstract writer base class that knows how to perform write operations to the settings file.<para/>
  /// 
  /// </summary>
  /// <typeparam name="TRootSection">The section type this writer is responsible for. Must extend <see cref="ConfigurationSection"/> of</typeparam>
  public abstract class SettingsWriter<TRootSection> : ISettingsWriter<TRootSection> where TRootSection : ConfigurationSection, new()
  {
    protected SettingsWriter()
    {
      this.RootSectionGroup = null;
    }

    protected SettingsWriter([NotNull] string rootSectionGroupName)
    {
      var appConfig = GetApplicationConfiguration();
      this.RootSectionGroup = new ConfigurationSectionGroup();
      AddSectionGroup(rootSectionGroupName, appConfig, this.RootSectionGroup);
    }

    protected SettingsWriter(string rootSectionName, string rootSectionGroupName) : this(rootSectionGroupName)
    {
      Configuration configuration = GetApplicationConfiguration();
      AddSection(rootSectionGroupName, rootSectionName, configuration, new ConfigurationSectionGroup());
      SaveSection(configuration, rootSectionName);
    }

    public abstract void AddSection(string sectionName, Configuration applicationConfiguration, string sectionGroupName = null);
    public abstract void AddSection(string sectionName, TRootSection newSection, Configuration applicationConfiguration, string sectionGroupName = null);
    public abstract void AddSection<TParentSection>(string sectionName, TParentSection newSection, Configuration applicationConfiguration, string sectionGroupName = null) where TParentSection : ConfigurationSection, new();
    public abstract ConfigurationSectionGroup CreateSubSectionGroup(string sectionGroupName, string parentSectionGroupName, Configuration applicationConfiguration, ConfigurationSectionGroup sectionGroup = null);
    public abstract void WriteSection(TRootSection section);
    public event EventHandler<DataChangedEventArgs> SettingsChanged;

    public void AddSectionGroup([NotNull] string sectionGroupName, Configuration appConfig, ConfigurationSectionGroup sectionGroup = null)
    {
      // Add a new ConfigurationSectionGroup with the requested name only if it doesn't exist
      if (appConfig.GetSectionGroup(sectionGroupName) != null)
      {
        return;
      }

      appConfig.SectionGroups.Add(sectionGroupName, sectionGroup ?? new ConfigurationSectionGroup());
    }

    public void AddSection([NotNull] string sectionName, string sectionGroupName, Configuration applicationConfiguration, ConfigurationSectionGroup sectionGroup = null)
    {
      if (applicationConfiguration == null)
      {
        applicationConfiguration = GetApplicationConfiguration();
      }
      
      if (string.IsNullOrWhiteSpace(sectionGroupName))
      {
        if (applicationConfiguration.GetSection(sectionName) == null)
        {
          applicationConfiguration.Sections.Add(sectionName, new TRootSection());
        }

        return;
      }

      if (applicationConfiguration.GetSectionGroup(sectionGroupName) == null)
      {
        AddSectionGroup(sectionGroupName, applicationConfiguration, sectionGroup ?? new ConfigurationSectionGroup());
        sectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      }

      TRootSection section = sectionGroup?.Sections[sectionName] as TRootSection;
      if (section == null)
      {
        sectionGroup?.Sections.Add(sectionName, new TRootSection());
      }
    }

    /// <summary>
    /// Default: ConfigurationSaveMode.Full
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="configurationSaveMode">optioanl. Defaullt is <code>ConfigurationSaveMode.Full</code></param>
    public virtual void SaveSection(Configuration configuration, string sectionName = null, ConfigurationSaveMode configurationSaveMode = ConfigurationSaveMode.Full)
    {
      configuration.Save(configurationSaveMode);
      if (!String.IsNullOrWhiteSpace(sectionName))
      {
        ConfigurationManager.RefreshSection(sectionName);
      }
    }
    
    protected Configuration GetApplicationConfiguration(ConfigurationUserLevel configurationUserLevel = ConfigurationUserLevel.None) => ConfigurationManager.OpenExeConfiguration(configurationUserLevel) as Configuration;

    //protected TRootSection RootSection { get; set; }
    protected ConfigurationSectionGroup RootSectionGroup { get; set; }

    protected virtual void NotifySettingsChanged<TArgs>(DataChangedEventArgs e) where TArgs : DataChangedEventArgs
    {
      this.SettingsChanged?.Invoke(this, e);
    }
  }
}
