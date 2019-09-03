# BionicLibrary.Net
Reusable utility and class library for WPF

## Contains 
* `BaseViewModel`
* `AsyncRelayCommand<T>`
* Extension Methods for WPF e.g.
  * `TryFindVisualParentElement : bool` 
  * `TryFindVisualChildElement<TChild> : bool`
  * `TryFindVisualChildElementByName : bool`
  * `FindVisualChildElements<TChildren> : IEnumerable<TChildren>`
  
### BaseViewModel 
implementing `INotifyPropertyChanged` and `INotifyDataErrorInfo`

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
### AsyncRelayComand&lt;T&gt; 
Reusable generic command class that encapsulates `ICommand` and allows asynchronous executionExample without validation

```c#
    // ICommand property
    public IRelayCommand<string> StringAsyncCommand => new RelayCommand<string>(ProcessStringAsync);
    
    // Execute asynchronously
    await StringAsyncCommand.ExecuteAsync("String value");
    
    // Execute synchronously
    StringAsyncCommand.Execute("String value");
    
```
