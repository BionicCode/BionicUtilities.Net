using System.Linq;

namespace BionicUtilities.NetStandard
{
  public class ArgumentsValidator
  {
    public static bool ArgsAreNull(params object[] argsToValidate)
    {
      return argsToValidate.Any((arg) => arg == null);
    }
  }
}
