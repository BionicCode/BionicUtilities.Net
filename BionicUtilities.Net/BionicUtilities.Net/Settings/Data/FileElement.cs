using System.Configuration;
using System.IO;

namespace Hell.LogDown.Settings.Data
{
  public class FileElement : ConfigurationElement
  {
    public FileElement()
    {
    }

    public FileElement(FileInfo fileInfo)
    {
      this.FileInfo = fileInfo;
      this.FileName = this.FileInfo.Name;
      this.FilePath = fileInfo.FullName;
    }
    
    [ConfigurationProperty("fileName", DefaultValue = " ", IsRequired = false)]
    public string FileName
    {
      get { return (string) this["fileName"]; }
      set { this["fileName"] = value; }
    }

    [ConfigurationProperty("filePath", DefaultValue = " ", IsRequired = true)]
    public string FilePath
    {
      get { return (string) this["filePath"]; }
      set { this["filePath"] = value; }
    }

    public FileInfo FileInfo { get; set; }
  }
}
