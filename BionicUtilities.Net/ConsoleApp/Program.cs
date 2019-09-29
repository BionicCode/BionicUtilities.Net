using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BionicUtilities.NetStandard;
using BionicUtilities.NetStandard.Generic;
using BionicUtilities.NetStandard.ViewModel;

namespace ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      var instanc = new NullObjectCreator<INullObject>().CreateNullInstance();
    }
  }
}
