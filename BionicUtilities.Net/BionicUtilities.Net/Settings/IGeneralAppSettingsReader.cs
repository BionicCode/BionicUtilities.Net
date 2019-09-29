namespace Hell.LogDown.Settings
{
  public interface IGeneralAppSettingsReader
  {
    bool TryReadValue(string key, out string value);
  }
}