using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Hell.LogDown.Settings.Data
{
  public class FileElementConfigurationElementCollection : ConfigurationElementCollection
  {
    public FileElementConfigurationElementCollection()
    {
    }

    public FileElementConfigurationElementCollection(IEnumerable<FileElement> fileElements)
    {
      this.AddRange(fileElements);
    }

    #region Overrides of ConfigurationElementCollection

    protected override ConfigurationElement CreateNewElement() => new FileElement();

    protected override object GetElementKey(ConfigurationElement element) => (element as FileElement)?.GetHashCode();

    #endregion


    public FileElement this[int index]
    {
      get
      {
        return (FileElement) BaseGet(index);
      }
      set
      {
        if (BaseGet(index) != null)
        {
          BaseRemoveAt(index);
        }
        BaseAdd(index, value);
      }
    }

    public new FileElement this[string name] => (FileElement) BaseGet(name);
    public int IndexOf(FileElement element) => BaseIndexOf(element);
    public void Add(FileElement element) => BaseAdd(element);
    public void AddRange(IEnumerable<FileElement> elements) => elements.ToList().ForEach(BaseAdd);

    // BaseAdd parameter false: allow duplicate values (same key but different values)
    protected override void BaseAdd(ConfigurationElement element) => BaseAdd(element, false);

    public void Remove(FileElement element)
    {
      if (BaseIndexOf(element) >= 0)
      {
        BaseRemove(element);
      }
    }

    public void RemoveAt(int index) => BaseRemoveAt(index);

    public void Remove(string name) => BaseRemove(name);

    public void Clear() => BaseClear();

    public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;
  }
}
