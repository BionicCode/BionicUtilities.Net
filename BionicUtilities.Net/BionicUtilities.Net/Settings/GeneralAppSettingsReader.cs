using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hell.LogDown.Resources;
using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
    public class GeneralAppSettingsReader : DefaultSettingsReader<GeneralSettingsSection>, IGeneralAppSettingsReader
  {
      public GeneralAppSettingsReader(string rootSectionName, string rootSectionGroupName) : base(rootSectionName, rootSectionGroupName)
      {
      }

      public bool TryReadValue(string key, out string value)
      {
        value = string.Empty;
        if (GetApplicationConfiguration().GetSectionGroup(GlobalSettingsResources.SectionGroupName)
                ?.Sections[GlobalSettingsResources.SectionName] is GeneralSettingsSection
              settingsSection && settingsSection.Entries.AllKeys.Contains(key))
        {
          value = settingsSection.Entries[key].Value;
          return true;
        }
        return false;
      }
    }
}
