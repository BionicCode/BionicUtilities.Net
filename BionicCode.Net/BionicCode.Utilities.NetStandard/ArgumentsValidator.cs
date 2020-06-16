using System.Linq;

namespace BionicCode.Utilities.NetStandard
{
  public class ArgumentsValidator
  {
    public static bool ArgsAreNull(params object[] argsToValidate)
    {
      return argsToValidate.Any((arg) => arg == null);
    }
  }
}
