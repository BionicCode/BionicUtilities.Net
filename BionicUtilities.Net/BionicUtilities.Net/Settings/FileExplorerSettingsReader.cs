using Hell.LogDown.Resources;
using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
  public class FileExplorerSettingsReader : DefaultSettingsReader<FileExplorerRecentFilesSection>, IFileExplorerSettingsReader
  {
    public FileExplorerSettingsReader(string rootSectionName, string rootSectionGroupName) : base(rootSectionName, rootSectionGroupName)
    {
    }

    #region Overrides of DefaultSettingsReader<FileExplorerSection>

    public override TSection ReadSection<TSection>(string sectionName)
    {
      return GetApplicationConfiguration().GetSectionGroup(FileExplorerSettingsResources.SectionGroupName)?.Sections[sectionName] as TSection;
    }

    #endregion
  }
}