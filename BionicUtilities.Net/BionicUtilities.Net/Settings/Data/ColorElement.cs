using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hell.LogDown.Components.LogDocument.SyncModes;

namespace Hell.LogDown.Settings.Data
{
  public class ColorElement : ConfigurationElement, IEqualityComparer<ColorElement>, IEquatable<ColorElement>
  {
    public ColorElement()
    {

    }

    public ColorElement(string r, string g, string b, string a)
    {
      this.R = r;
      this.G = g;
      this.B = b;
      this.A = a;
    }


    [ConfigurationProperty("r", DefaultValue = "", IsRequired = false)]
    public string R
    {
      get => (string) this["r"];
      set => this["r"] = value;
    }

    [ConfigurationProperty("g", DefaultValue = "", IsRequired = false)]
    public string G
    {
      get => (string) this["g"];
      set => this["g"] = value;
    }

    [ConfigurationProperty("b", DefaultValue = "", IsRequired = false)]
    public string B
    {
      get => (string) this["b"];
      set => this["b"] = value;
    }

    [ConfigurationProperty("a", DefaultValue = "", IsRequired = false)]
    public string A
    {
      get => (string) this["a"];
      set => this["a"] = value;
    }

    #region Overrides of ConfigurationElement

    public override bool Equals(object compareTo)
    {
      return compareTo is ColorElement comparable && Equals(comparable);
    }

    #region Equality members

    public bool Equals(ColorElement other)
    {
      return Equals(other, this);
    }

    public override int GetHashCode()
    {
      return this.A.GetHashCode()
             * this.R.GetHashCode()
             * this.G.GetHashCode()
             * this.B.GetHashCode();
    }

    #endregion

    #endregion

    #region Implementation of IEqualityComparer<in ColorElement>

    public bool Equals(ColorElement x, ColorElement y)
    {
      return x.A.Equals(y.A, StringComparison.OrdinalIgnoreCase)
             && x.R.Equals(y.R, StringComparison.OrdinalIgnoreCase)
             && x.G.Equals(y.G, StringComparison.OrdinalIgnoreCase)
             && x.B.Equals(y.B, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(ColorElement obj)
    {
      return obj.GetHashCode();
    }
  }

  #endregion
}

