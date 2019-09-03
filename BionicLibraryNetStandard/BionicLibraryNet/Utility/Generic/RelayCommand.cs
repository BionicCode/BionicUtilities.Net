using System;
using System.Threading.Tasks;

namespace BionicLibraryNet.Utility.Generic
{
  /// <summary>
  /// An implementation independent ICommand implementation.
  /// Enables instant creation of an ICommand without implementing the ICommand interface for each command.
  /// The individual Execute() an CanExecute() members are supplied via delegates.
  /// <seealso cref="System.Windows.Input.ICommand"/>
  /// </summary>
  /// <remarks>The type of <c>RelayCommand</c> actually is a <see cref="System.Windows.Input.ICommand"/></remarks>
  public class RelayCommand<TParam> : RelayCommand, IRelayCommand<TParam>
  {
    private readonly Func<TParam, Task> executeAsync;
    private readonly Action<TParam> execute;
    private readonly Predicate<TParam> canExecute;
    /// <summary>
    /// Creates a new command that can always executeNoParam.
    /// </summary>
    /// <param name="executeAsyncNoParam">The awaitable execution logic.</param>
    public RelayCommand(Func<Task> executeAsyncNoParam) : this(executeAsyncNoParam, () => true)
    {
    }
    /// <summary>
    /// Creates a new command that can always executeNoParam.
    /// </summary>
    /// <param name="executeAsync">The awaitable execution logic.</param>
    public RelayCommand(Func<TParam, Task> executeAsync)
        : this(executeAsync, (param) => true)
    {
    }
    /// <summary>
    /// Creates a new command that can always executeNoParam.
    /// </summary>
    /// <param name="executeNoParam">The awaitable execution logic.</param>
    public RelayCommand(Action executeNoParam)
        : this(executeNoParam, () => true)
    {
    }
    /// <summary>
    /// Creates a new command that can always executeNoParam.
    /// </summary>
    /// <param name="execute">The awaitable execution logic.</param>
    public RelayCommand(Action<TParam> execute)
        : this(execute, (param) => true)
    {
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="executeNoParam"></param>
    /// <param name="canExecuteNoParam">The execution status logic.</param>
    public RelayCommand(Action executeNoParam, Func<bool> canExecuteNoParam) : base(executeNoParam, canExecuteNoParam)
    {
    }
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<TParam> execute, Predicate<TParam> canExecute) : base((param) => execute((TParam) param), (param) => canExecute((TParam) param))
    {
      this.execute = execute;
      this.canExecute = canExecute;
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="executeAsyncNoParam"></param>
    /// <param name="canExecuteNoParam"></param>
    public RelayCommand(Func<Task> executeAsyncNoParam, Func<bool> canExecuteNoParam) : base(executeAsyncNoParam, canExecuteNoParam)
    {
    }
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="executeAsync">The awaitable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Func<TParam, Task> executeAsync, Predicate<TParam> canExecute) : base((param) => executeAsync((TParam) param), (param) => canExecute((TParam) param))
    {
      this.executeAsync = executeAsync;
      this.canExecute = canExecute;
    }
   
    /// <summary>
    /// Determines whether this RelayCommand can executeNoParam in its current state.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, 
    /// this object can be set to null.
    /// </param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public bool CanExecute(TParam parameter)
    {
      return base.CanExecute(parameter);
    }
    /// <summary>
    /// Executes the RelayCommand on the current command target.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, 
    /// this object can be set to null.
    /// </param>
    public async void Execute(TParam parameter)
    {
      if (this.executeAsync != null)
      {
        await ExecuteAsync(parameter);
        return;
      }
      this.execute(parameter);
    }

    /// <summary>
    /// Executes the RelayCommand on the current command target.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed, 
    /// this object can be set to null.
    /// </param>
    public async Task ExecuteAsync(TParam parameter)
    {
      if (this.executeAsync != null)
      {
        await this.executeAsync(parameter);
        return;
      }

      await Task.Run(() => this.execute(parameter));
    }
  }
}