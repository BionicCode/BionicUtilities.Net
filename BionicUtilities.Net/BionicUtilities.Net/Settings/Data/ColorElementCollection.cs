using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace Hell.LogDown.Settings.Data
{
  public class ColorElementCollection : ConfigurationElementCollection
  {
    public ColorElementCollection()
    {
    }

    public ColorElementCollection(IEnumerable<ColorElement> colorElements)
    {
      this.AddRange(colorElements);
    }

    #region Overrides of ConfigurationElementCollection

    protected override ConfigurationElement CreateNewElement() => new ColorElement();

    protected override object GetElementKey(ConfigurationElement element) =>
      (element as ColorElement)?.GetHashCode();

    #endregion


    public ColorElement this[int index]
    {
      get { return (ColorElement) BaseGet(index); }
      set
      {
        if (BaseGet(index) != null)
        {
          BaseRemoveAt(index);
        }

        BaseAdd(index, value);
      }
    }

    public new ColorElement this[string name] => (ColorElement) BaseGet(name);
    public int IndexOf(ColorElement element) => BaseIndexOf(element);
    public void Add(ColorElement element) => BaseAdd(element);
    public void AddRange(IEnumerable<ColorElement> elements) => elements.ToList().ForEach(BaseAdd);

    // BaseAdd parameter false: allow duplicate values (same key but different values)
    protected override void BaseAdd(ConfigurationElement element) => BaseAdd(element, false);

    public void Remove(ColorElement element)
    {
      if (element == null)
      {
        return;
      }

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



