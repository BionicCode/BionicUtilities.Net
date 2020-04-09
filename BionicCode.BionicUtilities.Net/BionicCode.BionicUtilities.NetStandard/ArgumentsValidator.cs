using System.Linq;

namespace BionicCode.BionicUtilities.NetStandard
{
  public class ArgumentsValidator
  {
    public static bool ArgsAreNull(params object[] argsToValidate)
    {
      return argsToValidate.Any((arg) => arg == null);
    }
  }
}
