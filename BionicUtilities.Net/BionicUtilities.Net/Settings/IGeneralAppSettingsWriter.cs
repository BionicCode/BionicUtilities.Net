namespace Hell.LogDown.Settings
{
  public interface IGeneralAppSettingsWriter
  {
    void WriteEntry(string key, string value);
  }
}