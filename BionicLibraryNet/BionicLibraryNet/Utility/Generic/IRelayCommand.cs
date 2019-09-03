#region Info
// //  
// Library
#endregion

using System.Threading.Tasks;
using System.Windows.Input;

namespace BionicLibrary.Net.Utility.Generic
{
  public interface IRelayCommand<TParam> : IRelayCommand, ICommand
  {
    bool CanExecute(TParam parameter);
    /// <summary>
    /// Executes the RelayCommand on the current command target.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, 
    /// this object can be set to null.
    /// </param>
    void Execute(TParam parameter);
    /// <summary>
    /// Executes the RelayCommand on the current command target.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, 
    /// this object can be set to null.
    /// </param>
    Task ExecuteAsync(TParam parameter);
  }
}