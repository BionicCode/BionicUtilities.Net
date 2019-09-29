using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Hell.LogDown.Settings.Data
{
  public class ValueSettingsElementCollection : ConfigurationElementCollection
  {
    public ValueSettingsElementCollection()
    {
    }

    public ValueSettingsElementCollection(IEnumerable<ValueSettingsElement> valueSettingsElements)
    {
      this.AddRange(valueSettingsElements);
    }

    #region Overrides of ConfigurationElementCollection

    protected override ConfigurationElement CreateNewElement() => new ValueSettingsElement();

    protected override object GetElementKey(ConfigurationElement element) =>
      (element as ValueSettingsElement)?.GetHashCode();

    #endregion


    public ValueSettingsElement this[int index]
    {
      get { return (ValueSettingsElement) BaseGet(index); }
      set
      {
        if (BaseGet(index) != null)
        {
          BaseRemoveAt(index);
        }

        BaseAdd(index, value);
      }
    }

    public new ValueSettingsElement this[string name] => (ValueSettingsElement) BaseGet(name);
    public int IndexOf(ValueSettingsElement element) => BaseIndexOf(element);
    public void Add(ValueSettingsElement element) => BaseAdd(element);
    public void AddRange(IEnumerable<ValueSettingsElement> elements) => elements.ToList().ForEach(BaseAdd);

    // BaseAdd parameter false: allow duplicate values (same key but different values)
    protected override void BaseAdd(ConfigurationElement element) => BaseAdd(element, false);

    public void Remove(ValueSettingsElement element)
    {
      if (BaseIndexOf(element) >= 0)
      {
        BaseRemove(element);
      }
    }

    public void RemoveAt(int index) => BaseRemoveAt(index);

    public void Remove(string name) => BaseRemove(name);

    public void Clear() => BaseClear();

    public override ConfigurationElementCollectionType CollectionType =>
      ConfigurationElementCollectionType.AddRemoveClearMap;
  }
}



