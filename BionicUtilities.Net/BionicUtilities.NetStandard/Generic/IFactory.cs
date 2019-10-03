using System;
using System.Collections.Generic;
using System.Text;

namespace BionicUtilities.NetStandard.Generic
{
  public interface IFactory<out TCreate>
  {
    TCreate Create();
    TCreate Create(params object[] args);
  }
}
