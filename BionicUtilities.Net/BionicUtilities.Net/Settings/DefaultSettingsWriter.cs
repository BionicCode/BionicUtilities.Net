using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings
{
  public class DefaultSettingsWriter<TRootSection> : SettingsWriter<TRootSection> where TRootSection : ConfigurationSection, new()
  {
    public DefaultSettingsWriter(
      string rootSectionGroupName,
      params KeyValuePair<string, TRootSection>[] sectionNames) : base(rootSectionGroupName)
    {
      sectionNames.ToList().ForEach(
        (sectionNameSectionPair) =>
        {
          Configuration configuration = GetApplicationConfiguration();
          AddSection(
            sectionNameSectionPair.Key,
            sectionNameSectionPair.Value,
            configuration,
            rootSectionGroupName);
          SaveSection(configuration);
        });
    }

    public DefaultSettingsWriter(params KeyValuePair<string, TRootSection>[] sectionNames)
    {
      sectionNames.ToList().ForEach(
        (sectionNameSectionPair) =>
        {
          Configuration applicationConfiguration = GetApplicationConfiguration();
          AddSection(sectionNameSectionPair.Key, sectionNameSectionPair.Value, applicationConfiguration);
          base.SaveSection(applicationConfiguration);
        });
    }

    #region Overrides of SettingsWriter<TRootSection>

    public override void AddSection(string sectionName, Configuration applicationConfiguration, string sectionGroupName = null)
    {
      AddSection(sectionName, null, applicationConfiguration, sectionGroupName);
    }

    public override void AddSection(string sectionName, TRootSection newSection, Configuration applicationConfiguration, string sectionGroupName = null)
    {
      if (string.IsNullOrWhiteSpace(sectionGroupName))
      {
        if (applicationConfiguration.GetSection(sectionName) == null)
        {
          applicationConfiguration.Sections.Add(sectionName, newSection ?? new TRootSection());
        }
      }
      else
      {
        ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
        if (configurationSectionGroup == null)
        {
          configurationSectionGroup = new ConfigurationSectionGroup();
          AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
        }

        if (configurationSectionGroup.Sections[sectionName] == null)
        {
          //applicationConfiguration.Save();
          configurationSectionGroup.Sections.Add(sectionName, newSection ?? new TRootSection());
        }
      }
    }

    public override void AddSection<TParentSection>(string sectionName, TParentSection newSection, Configuration applicationConfiguration, string sectionGroupName = null) 
    {
      if (string.IsNullOrWhiteSpace(sectionGroupName))
      {
        if (applicationConfiguration.GetSection(sectionName) == null)
        {
          applicationConfiguration.Sections.Add(sectionName, newSection ?? new TParentSection());
        }
      }
      else
      {
        ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
        if (configurationSectionGroup == null)
        {
          configurationSectionGroup = new ConfigurationSectionGroup();
          AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
        }

        if (configurationSectionGroup.Sections[sectionName] == null)
        {
          //applicationConfiguration.Save();
          configurationSectionGroup.Sections.Add(sectionName, newSection ?? new TParentSection());
        }
      }
    }

    public override ConfigurationSectionGroup CreateSubSectionGroup([NotNull] string sectionGroupName, [NotNull] string parentSectionGroupName, Configuration applicationConfiguration, ConfigurationSectionGroup sectionGroup = null)
    {
      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(parentSectionGroupName);

      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(parentSectionGroupName, applicationConfiguration, configurationSectionGroup);
      }

      if (configurationSectionGroup.SectionGroups[sectionGroupName] == null)
      {
        configurationSectionGroup.SectionGroups.Add(sectionGroupName, sectionGroup ?? new ConfigurationSectionGroup());
      }

      return configurationSectionGroup;
    }

    public override void WriteSection(TRootSection section)
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      
      // Refresh from file
      this.RootSectionGroup = applicationConfiguration.GetSectionGroup(this.RootSectionGroup.Name);

      if (this.RootSectionGroup == null)
      {
        ConfigurationSection configurationSection = applicationConfiguration.GetSection(section.SectionInformation.Name);
        if (configurationSection != null)
        {
          configurationSection = section;
        }
        else
        {
          applicationConfiguration.Sections.Add(section.SectionInformation.Name, section);
        }
        SaveSection(applicationConfiguration, section.SectionInformation.SectionName);
        return;
      }

      if (this.RootSectionGroup.Sections[section.SectionInformation.Name] != null)
      {
        this.RootSectionGroup.Sections.Remove(section.SectionInformation.Name);
      }

      AddSection(section.SectionInformation.Name, this.RootSectionGroup.Name, applicationConfiguration);
      SaveSection(applicationConfiguration, section.SectionInformation.SectionName);
    }

    #endregion
  }
}