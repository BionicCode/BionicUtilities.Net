using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hell.LogDown.Resources;
using Hell.LogDown.Settings;
using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
  public class GeneralAppSettingsWriter : DefaultSettingsWriter<GeneralSettingsSection>, IGeneralAppSettingsWriter
  {
    public GeneralAppSettingsWriter(
      string rootSectionGroupName,
      params KeyValuePair<string, GeneralSettingsSection>[] sectionKeyValues) : base(
      rootSectionGroupName,
      sectionKeyValues)
    {
    }

    public void WriteEntry(string key, string value)
    {
      var newEntry = new NameValueConfigurationElement(key, value);
      Configuration applicationConfiguration = GetApplicationConfiguration();
      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(GlobalSettingsResources.SectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(
          GlobalSettingsResources.SectionGroupName,
          applicationConfiguration,
          configurationSectionGroup);
      }
      var globalSettingsSection =
        configurationSectionGroup
          .Sections[GlobalSettingsResources.SectionName] as GeneralSettingsSection;
      
      if (globalSettingsSection == null)
      {
        globalSettingsSection = new GeneralSettingsSection();
        globalSettingsSection.Entries.Add(newEntry);
        AddSection(GlobalSettingsResources.SectionName, globalSettingsSection, applicationConfiguration, GlobalSettingsResources.SectionGroupName);
      }
      else
      {
        
        if (globalSettingsSection.Entries.AllKeys.Contains(key))
        {
          globalSettingsSection.Entries.Remove(key);
        }
        
        globalSettingsSection.Entries.Add(newEntry);
      }

      SaveSection(applicationConfiguration, GlobalSettingsResources.SectionName);
    }
  }
}
