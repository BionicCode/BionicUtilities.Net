# BionicLibrary.Net
Reusable utility and class library for WPF

## [NuGet package](https://www.nuget.org/packages/BionicUtilities.Net/)

## [Class Reference](https://rawcdn.githack.com/BionicCode/BionicLibraryNet/343b21a9daf5095f7f544abb0aa9672c671d66cc/BionicLibraryNet/BionicLibraryNet/Documentation/index.html)

## Contains 
* [`BaseViewModel`](https://github.com/BionicCode/BionicLibraryNet#baseviewmodel)
* [`AsyncRelayCommand<T>`](https://github.com/BionicCode/BionicLibraryNet#asyncrelaycomandt)
* Extension Methods for WPF e.g.
  * `TryFindVisualParentElement<TParent> : bool` 
  * `TryFindVisualParentElementByName : bool` 
  * `TryFindVisualChildElement<TChild> : bool`
  * `TryFindVisualChildElementByName : bool`
  * `FindVisualChildElements<TChildren> : IEnumerable<TChildren>`
  * `ICollection.AddRange<T>`
* ValueConverters
  * `ValueChangedEventArgs<T>`
  * `BoolToStringConverter`
  * `BooleanMultiValueConverter`
  * `FilePathTruncateConverter`
  * `InvertValueConverter`
* Collections
  * `ObservablePropertyChangedCollection<T>`
* MarkupExtensions
  * PrimitiveTypeExtension  
* [`Profiler`](https://github.com/BionicCode/BionicLibraryNet#Profiler)
  
  
### BaseViewModel 
implements `INotifyPropertyChanged` and `INotifyDataErrorInfo`

Example with validation

```c#
    private string name;
    public string Name
    {
      get => this.name;
      set
      {
        if (TrySetValue(
          value,
          (stringValue) =>
          {
            var messages = new List<string>() {"Name must start with an underscore"};
            return (stringValue.StartsWith("_"), messages);
          },
          ref this.name))
        {
          DoSomething(this.name);
        }
      }
    }
```
Example without validation

```c#
    private string name;
    public string Name
    {
      get => this.name;
      set
      {
        if (TrySetValue(value, ref this.name))
        {
          DoSomething(this.name);
        }
      }
    }
```
----

### AsyncRelayComand&lt;T&gt; 
Reusable generic command class that encapsulates `ICommand` and allows asynchronous execution.
When used with a `Binding` the command will execute asynchronously when an awaitable execute handler is assigned to the command.

```c#
    // ICommand property
    public IRelayCommand<string> StringAsyncCommand => new AsyncRelayCommand<string>(ProcessStringAsync);
    
    // Execute asynchronously
    await StringAsyncCommand.ExecuteAsync("String value");
    
    // Execute synchronously
    StringAsyncCommand.Execute("String value");
    
```

### Profiler
Static helper methods to measure performance e.g. the execution time of a code portion.

```c#
    // Specify a custom output
    Profiler.LogPrinter = (timeSpan) => PrintToFile(timeSpan);
    
    // Measure the average execution time of a specified number of iterations.
    TimeSpan elapsedTime = Profiler.LogAverageTime(() => ReadFromDatabase(), 1000);
    
    // Measure the execution times of a specified number of iterations.
    List<TimeSpan> elapsedTime = Profiler.LogTimes(() => ReadFromDatabase(), 1000);
    
    // Measure the execution time.
    TimeSpan elapsedTime = Profiler.LogTime(() => ReadFromDatabase());
```

