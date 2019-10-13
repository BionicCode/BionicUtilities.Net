# BionicUtilities.Net
Reusable utility and class library for WPF.

## [NuGet package](https://www.nuget.org/packages/BionicUtilities.Net/)

## [Class Reference](https://rawcdn.githack.com/BionicCode/BionicUtilities.Net/92232f5e172d52bc88cc1806454d2f011a2b79b7/BionicUtilities.Net/Documentation/Help/index.html)

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
* EventArgs
  * [`ValueChangedEventArgs<T>`](https://github.com/BionicCode/BionicUtilities.Net#valuechangedeventargst)
* ValueConverters
  * `BoolToStringConverter`
  * `BooleanMultiValueConverter`
  * `FilePathTruncateConverter`
  * `InvertValueConverter`
* Collections
  * `ObservablePropertyChangedCollection<T>`
* MarkupExtensions
  * `PrimitiveTypeExtension`
* [`Profiler`](https://github.com/BionicCode/BionicLibraryNet#Profiler)
* [`AppSettingsConnector`](https://github.com/BionicCode/BionicLibraryNet#AppSettingsConnector) - A defaul API to the AppSettings that provides strongly typed reading and writing (e.g. `boo`, `int`, `double`, `string`) of key-value pair values
* [`Most Recently Used (MRU) file manager`](https://github.com/BionicCode/BionicUtilities.Net/blob/master/README.md#mru-most-recently-used-file-manager) - API that maintains an MRU table stored in the Application Settings file. 
  
  
### `BaseViewModel`
implements `INotifyPropertyChanged` and `INotifyDataErrorInfo`

#### Example with validation

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
#### Example without validation

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

### `AsyncRelayComand<T>` 
Reusable generic command class that encapsulates `ICommand` and allows asynchronous execution.
When used with a `Binding` the command will execute asynchronously when an awaitable execute handler is assigned to the command.

#### Example

```c#
// ICommand property
public IAsyncRelayCommand<string> StringAsyncCommand => new AsyncRelayCommand<string>(ProcessStringAsync);
    
// Execute asynchronously
await StringAsyncCommand.ExecuteAsync("String value");
    
// Execute synchronously
StringAsyncCommand.Execute("String value");
    
```

### `Profiler`
Static helper methods to measure performance e.g. the execution time of a code portion.

#### Example

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
### `ValueChangedEventArgs<T>`
Generic `EventArgs` implementation that provides value change information like `OldValue` and `NewValue`.

#### Example

```c#
// Specify a named ValueTuple as event argument
event EventHandler<ValueChangedEventArgs<(bool HasError, string Message)>> Completed;    
    
protected virtual void RaiseCompleted((bool HasError, string Message) oldValue, (bool HasError, string Message) newValue)
{
  this.Completed?.Invoke(this, new ValueChangedEventArgs<(bool HasError, string Message)>(oldValue, newValue));
}

private void OnCompleted(object sender, ValueChangedEventArgs<(bool HasError, string Message)> e)
{
  (bool HasError, string Message) newValue = e.NewValue;
  if (newValue.HasError)
  {
    this.TaskCompletionSource.TrySetException(new InvalidOperationException(newValue.Message));
  }
  this.TaskCompletionSource.TrySetResult(true);
}
```
### `AppSettingsConnector` 
A static default API to the AppSettings that provides strongly typed reading and writing (e.g. `boo`, `int`, `double`, `string`) of key-value pair values.

#### Example

```c#
// Write the Most Recently Used file count to the AppSettings file
AppSettingsConnector.WriteInt("mruCount", 10);

// If key exists read the Most Recently Used file count from the AppSettings file
if (TryReadInt("mruCount", out int mruCount))
{
  this.MruCount = mruCount;
}
```

### MRU (Most Recently Used) file manager
The `MruManager` maintains a collection of `MostRecentlyUsedFileItem` elrecentlyements that map to a recently used file. Once the max number of recently used files is reached and a new file is added, the `MruManager` automatically removes the least used file to free the MRU table.

#### Example: Save file to MRU list

```c#

var mruManager = new MruManager();
mruManager.MaxMostRecentlyUsedCount = 30;

var openFileDialog = new OpenFileDialog();
bool? result = openFileDialog.ShowDialog();

// Process open file dialog box results
if (result == true)
{
  string filename = openFileDialog.FileName;

  // Save the picked file in the MRU list
  mruManager.AddMostRecentlyUsedFile(filename);
}
```
#### Example: Read file from MRU list

```c#

var mruManager = new MruManager();
mruManager.MaxMostRecentlyUsedCount = 30;

MostRecentlyUsedFileItem lastUsedFile = mruManager.MostRecentlyUsedFile;

// Since the list is a ReadOnlyObservableCollection you can directly bind to it 
// and receive CollectionChanged notifications, which will automatically update the binding target
ReadOnlyObservableCollection<MostRecentlyUsedFileItem> mruList = mruManager.MostRecentlyUsedFiles;
```
