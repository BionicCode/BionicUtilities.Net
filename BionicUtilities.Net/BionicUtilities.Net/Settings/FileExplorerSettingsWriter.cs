using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Hell.LogDown.Resources;
using Hell.LogDown.Settings.Data;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings
{
  public class FileExplorerSettingsWriter : DefaultSettingsWriter<FileExplorerRecentFilesSection>, IFileExplorerSettingsWriter
  {
    public FileExplorerSettingsWriter(string rootSectionGroupName, params KeyValuePair<string, FileExplorerRecentFilesSection>[] sectionKeyValues) : base(rootSectionGroupName, sectionKeyValues)
    {
    }

    public void WriteSection(FileExplorerRecentFilesSection newSection, int maxRecentFiles)
    {
      if (newSection is FileExplorerRecentFilesSection)
      {
        WriteRecentFilesData((newSection as FileExplorerRecentFilesSection).RecentFiles.Cast<FileElement>().Select((configElement) => new FileInfo(configElement.FilePath)), maxRecentFiles);
      }
      else
      {
        base.WriteSection(newSection);
      }
    }

    public void WriteDefaultFileFilters(Dictionary<string, bool> filterValues)
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      string sectionGroupName = FileExplorerSettingsResources.SectionGroupName;
      string sectionName = FileExplorerSettingsResources.DefaultFiltersSectionName;

      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
      }

      var fileExplorerDefaultFilterSection =
        configurationSectionGroup.Sections[sectionName] as FileExplorerDefaultFilterSection;
      if (configurationSectionGroup.Sections[sectionName] == null)
      {
        fileExplorerDefaultFilterSection = new FileExplorerDefaultFilterSection();
        AddSection(
          sectionName,
          fileExplorerDefaultFilterSection,
          applicationConfiguration,
          sectionGroupName);
      }
      else
      {
        fileExplorerDefaultFilterSection?.Filters.Clear();
      }

      filterValues
        .ToList()
        .ForEach(
          (filterValue) =>
          {
            fileExplorerDefaultFilterSection?
              .Filters.Add(filterValue.Key, filterValue.Value.ToString());
          });

      SaveSection(applicationConfiguration, sectionName);
    }

    public void WriteCustomFileFilters(string filterValues)
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      string sectionGroupName = FileExplorerSettingsResources.SectionGroupName;
      string sectionName = FileExplorerSettingsResources.CustomFiltersSectionName;

      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
      }

      if (configurationSectionGroup.Sections[sectionName] == null)
      {
        var fileExplorerSection = new FileExplorerCustomFilterSection(filterValues);
        AddSection(
          sectionName,
          fileExplorerSection,
          applicationConfiguration,
          sectionGroupName);
      }
      else
      {
        (configurationSectionGroup
            .Sections[sectionName] as FileExplorerCustomFilterSection)
          .Filters = filterValues;
      }
      SaveSection(applicationConfiguration, sectionName);
    }

    public void WriteAreCustomFileFiltersEnabled(bool isEnabled)
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      string sectionGroupName = FileExplorerSettingsResources.SectionGroupName;
      string sectionName = FileExplorerSettingsResources.CustomFiltersSectionName;

      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
      }

      if (configurationSectionGroup.Sections[sectionName] == null)
      {
        var fileExplorerSection = new FileExplorerCustomFilterSection() {IsCustomFilterEnabled = isEnabled};
        AddSection(
          sectionName,
          fileExplorerSection,
          applicationConfiguration,
          sectionGroupName);
      }
      else
      {
        (configurationSectionGroup
            .Sections[sectionName] as FileExplorerCustomFilterSection)
          .IsCustomFilterEnabled = isEnabled;
      }
      SaveSection(applicationConfiguration, sectionName);
    }

    public void WriteRecentFilesLimit(int maxFiles)
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      string sectionGroupName = FileExplorerSettingsResources.SectionGroupName;
      string sectionName = FileExplorerSettingsResources.RecentFileSectionName;
      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
      }
      
      if (configurationSectionGroup.Sections[sectionName] == null)
      {
        var fileExplorerSection = new FileExplorerRecentFilesSection() { MaxFiles = maxFiles };
        AddSection(
          sectionName,
          fileExplorerSection, applicationConfiguration,
          sectionGroupName);
      }
      else
      {
        (configurationSectionGroup
            .Sections[sectionName] as FileExplorerRecentFilesSection)
          .MaxFiles = maxFiles;
      }
      SaveSection(applicationConfiguration, sectionName);
    }

    public void WriteIsDeleteTemporaryFilesEnabled(bool isDeleteTemporaryFilesEnabled)
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      string sectionGroupName = FileExplorerSettingsResources.SectionGroupName;
      string sectionName = FileExplorerSettingsResources.RecentFileSectionName;
      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      if (configurationSectionGroup == null)
      {
        configurationSectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(sectionGroupName, applicationConfiguration, configurationSectionGroup);
      }
      
      if (configurationSectionGroup.Sections[sectionName] == null)
      {
        var fileExplorerSection = new FileExplorerRecentFilesSection() { IsDeleteTemporaryFilesEnabled = isDeleteTemporaryFilesEnabled };
        AddSection(
          sectionName,
          fileExplorerSection, applicationConfiguration,
          sectionGroupName);
      }
      else
      {
        (configurationSectionGroup
            .Sections[sectionName] as FileExplorerRecentFilesSection)
          .IsDeleteTemporaryFilesEnabled = isDeleteTemporaryFilesEnabled;
      }
      SaveSection(applicationConfiguration, sectionName);
    }

    public void ClearRecentFilesSection()
    {
      Configuration applicationConfiguration = GetApplicationConfiguration();
      string sectionGroupName = FileExplorerSettingsResources.SectionGroupName;
      string sectionName = FileExplorerSettingsResources.RecentFileSectionName;
      ConfigurationSectionGroup configurationSectionGroup = applicationConfiguration.GetSectionGroup(sectionGroupName);
      FileExplorerRecentFilesSection fileExplorerSection = null;
      if (configurationSectionGroup == null)
      {
        fileExplorerSection = applicationConfiguration.GetSection(sectionName) as FileExplorerRecentFilesSection;
      }
      else
      {
        fileExplorerSection = configurationSectionGroup.Sections[sectionName] as FileExplorerRecentFilesSection;
      }
      
      if (fileExplorerSection == null)
      {
        return;
      }
      
      fileExplorerSection.RecentFiles.Clear();
      SaveSection(applicationConfiguration, sectionName);
    }

    public void WriteRecentFilesData([NotNull] FileInfo newFileInfo, int maxRecentFiles)
    {
      WriteRecentFilesData(new List<FileInfo>() {newFileInfo}, maxRecentFiles);
    }

    public void WriteRecentFilesData([NotNull] IEnumerable<FileInfo> newFileInfos, int maxRecentFiles)
    {
      List<FileInfo> newRecentFileInfos = newFileInfos.ToList();
      Configuration applicationConfiguration = GetApplicationConfiguration();
      var sectionGroup =
        applicationConfiguration.GetSectionGroup(FileExplorerSettingsResources.SectionGroupName);
      if (sectionGroup == null)
      {
        sectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(
          FileExplorerSettingsResources.SectionGroupName,
          applicationConfiguration,
          sectionGroup);
      }
      
      if (sectionGroup.Sections[FileExplorerSettingsResources.RecentFileSectionName] == null)
      {
        var fileExplorerSection = new FileExplorerRecentFilesSection(newRecentFileInfos);
        AddSection(FileExplorerSettingsResources.RecentFileSectionName, fileExplorerSection, applicationConfiguration, FileExplorerSettingsResources.SectionGroupName);
      }
      else
      {
        var recentFileElementCollection = ((FileExplorerRecentFilesSection) sectionGroup.Sections[FileExplorerSettingsResources.RecentFileSectionName]).RecentFiles;
        
        // Adhere to recent files limit and remove overflowing items
        int currentFilesCount = recentFileElementCollection?.Count ?? 0;
        var overFlow = newRecentFileInfos.Count + currentFilesCount - maxRecentFiles;
        if (overFlow > 0)
        {
          while (overFlow > 0 && recentFileElementCollection.Count > 0)
          {
            recentFileElementCollection?.RemoveAt(0);
            overFlow--;
          }

          // Remove duplicates
          foreach (var recentInfo in recentFileElementCollection)
          {
            FileInfo recentFileInfo = recentInfo as FileInfo;
            if (newRecentFileInfos.Contains(recentFileInfo))
            {
              newRecentFileInfos.Remove(recentFileInfo);
            }
          }
          
          // Remove overflowing new items
          while (overFlow > 0 && newRecentFileInfos.Count > 0)
          {
            newRecentFileInfos.RemoveAt(0);
            overFlow--;
          }
        }

        // Write remaning to settings
        newRecentFileInfos.ForEach((recentFileInfo) => recentFileElementCollection?.Add(new FileElement(recentFileInfo)));
      }

      SaveSection(applicationConfiguration, FileExplorerSettingsResources.RecentFileSectionName);

      NotifyOnRecentFilesChanged(new DataChangedEventArgs<IEnumerable<FileInfo>>(newRecentFileInfos));
    }

    public void WriteAutoOpenFilesData([NotNull] IEnumerable<string> autoOpenFileNames)
    {
      List<string> fileNames = autoOpenFileNames.ToList();
      Configuration applicationConfiguration = GetApplicationConfiguration();
      var sectionGroup =
        applicationConfiguration.GetSectionGroup(FileExplorerSettingsResources.SectionGroupName);
      if (sectionGroup == null)
      {
        sectionGroup = new ConfigurationSectionGroup();
        AddSectionGroup(
          FileExplorerSettingsResources.SectionGroupName,
          applicationConfiguration,
          sectionGroup);
      }
      
      if (sectionGroup.Sections[FileExplorerSettingsResources.AutoOpenFilesSectionName] == null)
      {
        var fileExplorerSection = new StringValuesSection(fileNames);
        AddSection(FileExplorerSettingsResources.AutoOpenFilesSectionName, fileExplorerSection, applicationConfiguration, FileExplorerSettingsResources.SectionGroupName);
      }
      else
      {
        ((StringValuesSection) sectionGroup.Sections[FileExplorerSettingsResources.AutoOpenFilesSectionName]).AutoOpenFiles = new ValueSettingsElementCollection(fileNames.Select((fileName) => new ValueSettingsElement(fileName)));
      }

      SaveSection(applicationConfiguration, FileExplorerSettingsResources.AutoOpenFilesSectionName);
    }

    protected virtual void NotifyOnRecentFilesChanged(DataChangedEventArgs<IEnumerable<FileInfo>> e)
    {
      this.RecentFilesChanged?.Invoke(this, e);
    }

    public event EventHandler<DataChangedEventArgs<IEnumerable<FileInfo>>> RecentFilesChanged;
  }
}
