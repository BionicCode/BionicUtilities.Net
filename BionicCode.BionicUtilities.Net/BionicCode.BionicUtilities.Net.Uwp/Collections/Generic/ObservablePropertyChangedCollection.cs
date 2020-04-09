using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace BionicCode.BionicUtilities.Net.Uwp.Collections.Generic
{
  /// <summary>
  /// Raises <see cref="ObservableCollection{T}.CollectionChanged"></see> event when the property of an item raised <see cref="INotifyPropertyChanged.PropertyChanged"/>. The change action for this particular notification is <see cref="NotifyCollectionChangedAction.Reset"/> with a reference to the notifying item and the item's index. The item must implement <see cref="INotifyPropertyChanged"/>.
  /// </summary>
  /// <typeparam name="TItem"></typeparam>
  [DebuggerDisplay("Count = {Count}")]
  [Serializable]
  public class ObservablePropertyChangedCollection<TItem> : ObservableCollection<TItem>
  {
    #region Overrides of ObservableCollection<TItem>

    /// <inheritdoc />
    protected override void InsertItem(int index, TItem item)
    {
      base.InsertItem(index, item);
      if (item is INotifyPropertyChanged propertyChangedItem)
      {
        propertyChangedItem.PropertyChanged += OnItemPropertyChanged;
      }
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
      if (index < this.Count)
      {
        TItem item = this.Items[index];
        if (item is INotifyPropertyChanged propertyChangedItem)
        {
          propertyChangedItem.PropertyChanged -= OnItemPropertyChanged;
        }
      }

      base.RemoveItem(index);
    }

    /// <inheritdoc />
    protected override void ClearItems()
    {
      this.Items.OfType<INotifyPropertyChanged>()
        .ToList()
        .ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);

      base.ClearItems();
    }

    /// <inheritdoc />
    protected override void SetItem(int index, TItem item)
    {
      if (index < this.Count)
      {
        if (this.Items[index] is INotifyPropertyChanged oldPropertyChangedItem)
        {
          oldPropertyChangedItem.PropertyChanged -= OnItemPropertyChanged;
        }

        if (item is INotifyPropertyChanged newPropertyChangedItem)
        {
          newPropertyChangedItem.PropertyChanged += OnItemPropertyChanged;
        }
      }

      base.SetItem(index, item);
    }

    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, sender, IndexOf((TItem)sender)));
    }

    #endregion
  }
}
