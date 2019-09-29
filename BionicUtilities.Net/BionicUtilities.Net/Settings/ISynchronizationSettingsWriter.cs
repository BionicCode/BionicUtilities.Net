using System.Collections.Generic;
using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
  public interface ISynchronizationSettingsWriter
  {
    void WriteFileIdColorEntry(IEnumerable<ColorElement> colorElements);
    void WriteFileMergeSettingsEntry(string key, string value);
    void WriteGeneralSettingsEntry(string key, string value);
  }
}