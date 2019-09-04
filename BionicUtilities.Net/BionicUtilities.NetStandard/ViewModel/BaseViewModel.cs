using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BionicLibrary.NetStandard.ViewModel
{
  public abstract class BaseViewModel : IViewModel, INotifyDataErrorInfo
  {
    protected BaseViewModel()
    {
      this.Errors = new Dictionary<string, IEnumerable<string>>();
    }

    public bool TrySetValue<TValue>(TValue value, ref TValue targetBackingField, [CallerMemberName] string propertyName = null)
    {
      if (value.Equals(targetBackingField))
      {
        return false;
      }

      targetBackingField = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    public bool TrySetValue<TValue>(TValue value, Func<TValue, (bool IsValid, IEnumerable<string> ErrorMessages)> validationDelegate, ref TValue targetBackingField, [CallerMemberName] string propertyName = null)
    {
      if (value.Equals(targetBackingField))
      {
        return false;
      }

      targetBackingField = value;
      OnPropertyChanged(propertyName);

      return IsValueValid(value, validationDelegate, propertyName);
    }

    protected virtual bool IsValueValid<TValue>(TValue value, Func<TValue, (bool IsValid, IEnumerable<string> ErrorMessages)> validationDelegate, [CallerMemberName] string propertyName = null)
    {
      this.Errors.Remove(propertyName);
      (bool IsValid, IEnumerable<string> ErrorMessages) validationResult = validationDelegate(value);
      if (!validationResult.IsValid)
      {
        this.Errors.Add(propertyName, validationResult.ErrorMessages);
      }

      OnErrorsChanged(propertyName);
      return validationResult.IsValid;
    }

    protected virtual bool PropertyHasError([CallerMemberName] string propertyName = null) =>
      this.Errors.ContainsKey(propertyName);

      /// <summary>
      /// Event fired whenever a child property changes its value.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Method called to fire a <see cref="PropertyChanged"/> event.
      /// </summary>
      /// <param name="propertyName"> The property name. </para
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      #region Implementation of INotifyDataErrorInfo

      /// <inheritdoc />
      public IEnumerable GetErrors(string propertyName) =>
        string.IsNullOrWhiteSpace(propertyName) 
          ? this.Errors.SelectMany(entry => entry.Value) 
          : this.Errors.TryGetValue(propertyName, out IEnumerable<string> errors) 
            ? errors 
            : new List<string>();

      /// <inheritdoc />
      public bool HasErrors => this.Errors.Any();

      /// <inheritdoc />
      public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

      #endregion

      private Dictionary<string, IEnumerable<string>> Errors { get; set; }

      protected virtual void OnErrorsChanged(string propertyName)
      {
        this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
      }
  }
}