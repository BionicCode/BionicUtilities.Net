using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace BionicUtilities.NetStandard.ViewModel
{
  public abstract class BaseViewModel : IViewModel
  {
    protected BaseViewModel()
    {
      this.Errors = new Dictionary<string, IEnumerable<string>>();
    }

    protected virtual bool TrySetValue<TValue>(TValue value, ref TValue targetBackingField, [CallerMemberName] string propertyName = null)
    {
      if (value.Equals(targetBackingField))
      {
        return false;
      }

      targetBackingField = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    /// <summary>
    ///  Sets the value of the referenced property and executes a validation delegate.
    /// </summary>
    /// <typeparam name="TValue">The generic value type parameter</typeparam>
    /// <param name="value">The new value which is to be set to the property.</param>
    /// <param name="validationDelegate">The callback that is used to validate the new value.</param>
    /// <param name="targetBackingField">The reference to the backing field.</param>
    /// <param name="propertyName">The name of the property to set. Default name is the property that called this method.</param>
    /// <param name="isRejectInvalidValueEnabled">When <c>true</c> the invalid value is not stored to the backing field.<br/> Use this to ensure that the view model in a valid state.</param>
    /// <param name="isThrowExceptionOnValidationErrorEnabled">Enable throwing an <exception cref="ArgumentException"></exception> if validation failed. Use this when <c>ValidatesOnExceptions</c> on a <c>Binding</c> is set to <c>true</c></param>
    /// <exception cref="ArgumentException">Thrown on validation failed</exception>
    /// <returns>Returns <c>true</c> if the new value doesn't equal the old value and the new value is valid. Returns <c>false</c> if the new value equals the old value or the validation has failed.</returns>
    /// <remarks>This property setter supports invalid value rejection, which means values are only assigned to the backing field if they are valid which is when the <paramref name="validationDelegate"/> return <c>true</c>.<br/> To support visual validation error feed back and proper behavior in <c>TwoWay</c> binding scenarios, <br/> it is recommended to set <paramref name="isThrowExceptionOnValidationErrorEnabled"/> to <c>true</c> and set the validation mode of the binding to <c>Binding.ValidatesOnExceptions</c>.<br/>If not doing so, the binding target will clear the new value and show the last valid value instead.</remarks>
    protected virtual bool TrySetValue<TValue>(TValue value, Func<TValue, (bool IsValid, IEnumerable<string> ErrorMessages)> validationDelegate, ref TValue targetBackingField, [CallerMemberName] string propertyName = null, bool isRejectInvalidValueEnabled = true, bool isThrowExceptionOnValidationErrorEnabled = false)
    {
      bool isValueValid = IsValueValid(value, validationDelegate, propertyName);
      if (isThrowExceptionOnValidationErrorEnabled && PropertyHasError(propertyName))
      {
        throw new ArgumentException(string.Empty);
      }

      if ((!isValueValid && isRejectInvalidValueEnabled) || value.Equals(targetBackingField))
      {
        return false;
      }

      targetBackingField = value;
      OnPropertyChanged(propertyName);
      return true;
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

    public virtual bool PropertyHasError([CallerMemberName] string propertyName = null) =>
      this.Errors.ContainsKey(propertyName);

    /// <inheritdoc />
    public IEnumerable<string> GetPropertyErrors(string propertyName = null) => GetErrors(propertyName).Cast<string>();

    /// <summary>
      /// Event fired whenever a child property changes its value.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      /// <summary>
      /// Method called to fire a <see cref="PropertyChanged"/> event.
      /// </summary>
      /// <param name="propertyName"> The property name. </param>
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