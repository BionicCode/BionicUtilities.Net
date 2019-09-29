using System.Configuration;

namespace Hell.LogDown.Settings
{
  public class DefaultSettingsReader<TRootSection> : SettingsReader<TRootSection> where TRootSection : ConfigurationSection, new()
  {
    public DefaultSettingsReader(string rootSectionName, string rootSectionGroupName) : base(rootSectionName, rootSectionGroupName)
    {
    }

    #region Overrides of SettingsReader<FileExplorerSection>

    public override TSection ReadSection<TSection>(string sectionName)
    {
      return GetApplicationConfiguration().GetSection(sectionName) as TSection;
    }

    public override bool TryReadSection<TSubGroupSection>(string sectionName, out TSubGroupSection section)
    {
      section = GetApplicationConfiguration().GetSection(sectionName) as TSubGroupSection;
      return section != null;
    }

    public override bool TryReadSection<TSubGroupSection>(string sectionGroupName, string sectionName, out TSubGroupSection sectionResult)
    {
      sectionResult = GetApplicationConfiguration().GetSectionGroup(sectionGroupName).Sections[sectionName] as TSubGroupSection;
      return sectionResult != null;
    }

    public override bool TryReadSectionGroup(string sectionGroupName, out ConfigurationSectionGroup sectiongroupResult)
    {
      sectiongroupResult = GetApplicationConfiguration().GetSectionGroup(sectionGroupName);
      return sectionGroupName != null;
    }

    #endregion
  }
}