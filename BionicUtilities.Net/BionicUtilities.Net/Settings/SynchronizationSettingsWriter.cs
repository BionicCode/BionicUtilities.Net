using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hell.LogDown.Resources;
using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
  public class SynchronizationSettingsWriter : DefaultSettingsWriter<SynchronizationSettingsSection>, ISynchronizationSettingsWriter
  {
    public SynchronizationSettingsWriter(
      string rootSectionGroupName,
      params KeyValuePair<string, SynchronizationSettingsSection>[] sectionKeyValues) : base(
      rootSectionGroupName,
      sectionKeyValues)
    {
    }

    public void WriteGeneralSettingsEntry(string key, string value)
    {
      var newEntry = new NameValueConfigurationElement(key, value);
      Configuration applicationConfiguration = GetApplicationConfiguration();
      SynchronizationSettingsSection synchronizationSettingsSection = GetSection(applicationConfiguration);

      if (synchronizationSettingsSection.General.AllKeys.Contains(key))
      {
        synchronizationSettingsSection.General.Remove(key);
      }

      synchronizationSettingsSection.General.Add(newEntry);
      SaveSection(applicationConfiguration, SynchronizationSettingsResources.SectionName);
    }

    public void WriteFileMergeSettingsEntry(string key, string value)
    {
      var newEntry = new NameValueConfigurationElement(key, value);
      Configuration applicationConfiguration = GetApplicationConfiguration();
      SynchronizationSettingsSection synchronizationSettingsSection = GetSection(applicationConfiguration);

      if (synchronizationSettingsSection.FileMerge.AllKeys.Contains(key))
      {
        synchronizationSettingsSection.FileMerge.Remove(key);
      }

      synchronizationSettingsSection.FileMerge.Add(newEntry);
      SaveSection(applicationConfiguration, SynchronizationSettingsResources.SectionName);
    }

    public void WriteFileIdColorEntry(IEnumerable<ColorElement> colorElements)
    {
      var datas = colorElements?.ToList();
      if (!datas?.Any() ?? true)
      {
        return;
      }

      Configuration applicationConfiguration = GetApplicationConfiguration();
      SynchronizationSettingsSection synchronizationSettingsSection = GetSection(applicationConfiguration);

      datas?.ForEach((colorElementToSave) => synchronizationSettingsSection.MergedDocumentColors.Remove(
        synchronizationSettingsSection.MergedDocumentColors
          .Cast<ColorElement>()
          .FirstOrDefault((colorElementFromFile) => colorElementFromFile.Equals(colorElementToSave))));

      synchronizationSettingsSection.MergedDocumentColors.AddRange(datas);
      SaveSection(applicationConfiguration, SynchronizationSettingsResources.SectionName);
    }

    private SynchronizationSettingsSection GetSection(Configuration applicationConfiguration)
    {
      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(SynchronizationSettingsResources.SectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(
          SynchronizationSettingsResources.SectionGroupName,
          applicationConfiguration,
          configurationSectionGroup);
      }

      if (!(configurationSectionGroup
          .Sections[SynchronizationSettingsResources.SectionName] is SynchronizationSettingsSection synchronizationSettingsSection))
      {
        synchronizationSettingsSection = new SynchronizationSettingsSection();
        AddSection(SynchronizationSettingsResources.SectionName, synchronizationSettingsSection, applicationConfiguration, SynchronizationSettingsResources.SectionGroupName);
      }

      return synchronizationSettingsSection;
    }
  }
}
