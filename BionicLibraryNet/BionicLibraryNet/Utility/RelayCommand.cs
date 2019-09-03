using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BionicLibrary.Net.Utility
{


/// <summary>
/// An implementation independent ICommand implementation.
/// Enables instant creation of an ICommand without implementing the ICommand interface for each command.
/// The individual Execute() an CanExecute() members are supplied via delegates.
/// <seealso cref="System.Windows.Input.ICommand"/>
/// </summary>
/// <remarks>The type of <c>RelayCommand</c> actually is a <see cref="System.Windows.Input.ICommand"/></remarks>
public class RelayCommand : IRelayCommand
{
  protected readonly Func<Task> ExecuteAsyncNoParam;
  protected readonly Action ExecuteNoParam;
  protected readonly Func<bool> CanExecuteNoParam;
  private readonly Func<object, Task> executeAsync;
  private readonly Action<object> execute;
  private readonly Predicate<object> canExecute;
  /// <summary>
  /// Raised when RaiseCanExecuteChanged is called.
  /// </summary>
  public event EventHandler CanExecuteChanged
  {
    add { CommandManager.RequerySuggested += value; }
    remove { CommandManager.RequerySuggested -= value; }
  }
  /// <summary>
  /// Creates a new command that can always execute.
  /// </summary>
  /// <param name="execute">The execution logic.</param>
  public RelayCommand(Action<object> execute)
      : this(execute, (param) => true)
  {
  }
  /// <summary>
  /// Creates a new command that can always execute.
  /// </summary>
  /// <param name="executeNoParam">The execution logic.</param>
  public RelayCommand(Action executeNoParam)
      : this(executeNoParam, () => true)
  {
  }
  /// <summary>
  /// Creates a new command that can always execute.
  /// </summary>
  /// <param name="executeAsync">The awaitable execution logic.</param>
  public RelayCommand(Func<object, Task> executeAsync)
      : this(executeAsync, (param) => true)
  {
  }
  /// <summary>
  /// Creates a new command that can always execute.
  /// </summary>
  /// <param name="executeAsyncNoParam">The awaitable execution logic.</param>
  public RelayCommand(Func<Task> executeAsyncNoParam)
      : this(executeAsyncNoParam, () => true)
  {
  }
  /// <summary>
  /// Creates a new command.
  /// </summary>
  /// <param name="executeNoParam">The execution logic.</param>
  /// <param name="canExecuteNoParam">The execution status logic.</param>
  public RelayCommand(Action executeNoParam, Func<bool> canExecuteNoParam)
  {
    this.ExecuteNoParam = executeNoParam ?? throw new ArgumentNullException(nameof(executeNoParam));
    this.CanExecuteNoParam = canExecuteNoParam;
  }
  /// <summary>
  /// Creates a new command.
  /// </summary>
  /// <param name="execute">The execution logic.</param>
  /// <param name="canExecute">The execution status logic.</param>
  public RelayCommand(Action<object> execute, Predicate<object> canExecute)
  {
    this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
    this.canExecute = canExecute;
  }
  /// <summary>
  /// Creates a new command.
  /// </summary>
  /// <param name="executeAsyncNoParam">The awaitable execution logic.</param>
  /// <param name="canExecuteNoParam">The execution status logic.</param>
  public RelayCommand(Func<Task> executeAsyncNoParam, Func<bool> canExecuteNoParam)
  {
    this.ExecuteAsyncNoParam = executeAsyncNoParam ?? throw new ArgumentNullException(nameof(executeAsyncNoParam));
    this.CanExecuteNoParam = canExecuteNoParam;
  }
  /// <summary>
  /// Creates a new command.
  /// </summary>
  /// <param name="executeAsync">The awaitable execution logic.</param>
  /// <param name="canExecute">The execution status logic.</param>
  public RelayCommand(Func<object, Task> executeAsync, Predicate<object> canExecute)
  {
    this.executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
    this.canExecute = canExecute;
  }
  /// <summary>
  /// Determines whether this RelayCommand can execute in its current state.
  /// </summary>
  /// <returns>true if this command can be executed; otherwise, false.</returns>
  public bool CanExecute()
  {
    return this.CanExecuteNoParam == null || this.CanExecuteNoParam();
  }

  /// <summary>
  /// Executes the RelayCommand on the current command target.
  /// </summary>
  public async void Execute()
  {
    if (this.ExecuteAsyncNoParam != null)
    {
      await ExecuteAsync();
      return;
    }
    this.ExecuteNoParam();
  }

  /// <summary>
  /// Executes the RelayCommand on the current command target.
  /// </summary>
  public async Task ExecuteAsync()
  {
    if (this.ExecuteAsyncNoParam != null)
    {
      await this.ExecuteAsyncNoParam();
      return;
    }
 
    await Task.Run(() => this.ExecuteNoParam());
  }
  /// <summary>
  /// Determines whether this RelayCommand can execute in its current state.
  /// </summary>
  /// <param name="parameter">
  /// Data used by the command. If the command does not require data to be passed, 
  /// this object can be set to null.
  /// </param>
  /// <returns>true if this command can be executed; otherwise, false.</returns>
  public bool CanExecute(object parameter)
  {
    return this.canExecute == null || this.canExecute(parameter);
  }

  /// <summary>
  /// Executes the RelayCommand on the current command target.
  /// </summary>
  /// <param name="parameter">
  /// Data used by the command. If the command does not require data to be passed, 
  /// this object can be set to null.
  /// </param>
  public async void Execute(object parameter)
  {
    if (parameter == null)
    {
      Execute();
      return;
    }

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
  public async Task ExecuteAsync(object parameter)
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