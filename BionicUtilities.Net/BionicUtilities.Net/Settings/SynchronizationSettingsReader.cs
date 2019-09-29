using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hell.LogDown.Resources;
using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
  public class SynchronizationSettingsReader : DefaultSettingsReader<SynchronizationSettingsSection>, ISynchronizationSettingsReader
  {
    public SynchronizationSettingsReader(string rootSectionName, string rootSectionGroupName) : base(rootSectionName, rootSectionGroupName)
    {
    }

    public bool TryReadSection(out SynchronizationSettingsSection section)
    {
      section = new SynchronizationSettingsSection();
      if (GetApplicationConfiguration().GetSectionGroup(SynchronizationSettingsResources.SectionGroupName)
              ?.Sections[SynchronizationSettingsResources.SectionName] is SynchronizationSettingsSection
            settingsSection)
      {
        section = settingsSection;
        return true;
      }
      return false;
    }
  }
}
