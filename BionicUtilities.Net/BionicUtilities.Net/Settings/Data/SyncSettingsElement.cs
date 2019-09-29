using System.Configuration;
using Hell.LogDown.Components.LogDocument.SyncModes;

namespace Hell.LogDown.Settings.Data
{
  public class SyncSettingsElement : ConfigurationElement
  {
    [ConfigurationProperty("synchronizationModes", DefaultValue = SynchronizationModes.None, IsRequired = false)]
    public SynchronizationModes SynchronizationModes
    {
      get { return (SynchronizationModes) this[nameof(this.SynchronizationModes).ToLowerInvariant()]; }
      set { this[nameof(this.SynchronizationModes).ToLowerInvariant()] = value; }
    }

    [ConfigurationProperty("synchronizationIsEnabled", DefaultValue = SynchronizationModes.None, IsRequired = false)]
    public bool SynchronizationIsEnabled
    {
      get { return (bool) this[nameof(this.SynchronizationIsEnabled).ToLowerInvariant()]; }
      set { this[nameof(this.SynchronizationIsEnabled).ToLowerInvariant()] = value; }
    }
  }
}
