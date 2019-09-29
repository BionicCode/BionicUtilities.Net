using Hell.LogDown.Settings.Data;

namespace Hell.LogDown.Settings
{
  public interface ISynchronizationSettingsReader
  {
    bool TryReadSection(out SynchronizationSettingsSection section);
  }
}