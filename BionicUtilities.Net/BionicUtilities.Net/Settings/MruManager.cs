using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BionicUtilities.NetStandard.Generic;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicUtilities.Net.Settings
{
  /// <summary>
  /// API that manages a MRU (Most Recently Used files) table which is stored in the AppSettings file.
  /// </summary>
  public class MruManager : BaseViewModel, IMruManager
  {
    private const string MaxRecentlyUsedCountKey = "mruCount";
    private const string MostRecentlyUsedKey = "mru";
    private const string MostRecentlyUsedKeyStringSeparator = ";";

    /// <summary>
    /// Default constructor
    /// </summary>
    public MruManager()
    {
      if (!(AppSettingsConnector.TryReadString(MruManager.MaxRecentlyUsedCountKey, out string mruCount) &&
            int.TryParse(mruCount, out this.maxMostRecentlyUsedCount)))
      {
        this.maxMostRecentlyUsedCount = 10;
      }

      IEnumerable<MostRecentlyUsedFileItem> mru = new List<MostRecentlyUsedFileItem>();
      if (AppSettingsConnector.TryReadString(MruManager.MostRecentlyUsedKey, out string fileList))
      {
        mru = fileList.Split(
            new[] {MruManager.MostRecentlyUsedKeyStringSeparator},
            StringSplitOptions.RemoveEmptyEntries)
          .Where(File.Exists)
          .Select(validPath => new MostRecentlyUsedFileItem(new FileInfo(validPath)));
      }

      this.InternalMostRecentlyUsedFiles = new ObservableCollection<MostRecentlyUsedFileItem>(mru);
      this.MostRecentlyUsedFiles = new ReadOnlyObservableCollection<MostRecentlyUsedFileItem>(this.InternalMostRecentlyUsedFiles);
      AddMostRecentlyUsedFile(this.InternalMostRecentlyUsedFiles.LastOrDefault()?.FullName ?? string.Empty);
    }

    /// <inheritdoc />
    public void AddMostRecentlyUsedFile(string filePath)
    {
      if (!File.Exists(filePath))
      {
        return;
      }

      MostRecentlyUsedFileItem existingMruItem;
      if ((existingMruItem = this.InternalMostRecentlyUsedFiles.FirstOrDefault(mruItem => mruItem.FullName.Equals(filePath, StringComparison.OrdinalIgnoreCase))) != null)
      {
        int indexOfExistingMruItem = this.InternalMostRecentlyUsedFiles.IndexOf(existingMruItem);
        this.InternalMostRecentlyUsedFiles.Move(indexOfExistingMruItem, this.InternalMostRecentlyUsedFiles.Count - 1);
      }
      else
      {
        MostRecentlyUsedFileItem oldItem = null;
        if (this.InternalMostRecentlyUsedFiles.Count >= this.MaxMostRecentlyUsedCount)
        {
          oldItem = this.InternalMostRecentlyUsedFiles.FirstOrDefault();
          this.InternalMostRecentlyUsedFiles.RemoveAt(0);
        }

        var newItem = new MostRecentlyUsedFileItem(new FileInfo(filePath));
        this.InternalMostRecentlyUsedFiles.Add(newItem);

        OnFileAdded(oldItem, newItem);
      }

      this.MostRecentlyUsedFile = this.InternalMostRecentlyUsedFiles.Last();

      SaveInternalMruListToSettingsFile();
    }

    private void SaveInternalMruListToSettingsFile()
    {
      string mruListString = string.Join(
        MruManager.MostRecentlyUsedKeyStringSeparator,
        this.InternalMostRecentlyUsedFiles.Select(mruItem => mruItem.FullName));
      AppSettingsConnector.WriteString(MruManager.MostRecentlyUsedKey, mruListString);
    }

    /// <inheritdoc />
    public void Clear()
    {
      this.InternalMostRecentlyUsedFiles.Clear();
      SaveInternalMruListToSettingsFile();
    }

    private ReadOnlyObservableCollection<MostRecentlyUsedFileItem> mostRecentlyUsedFiles;
    /// <inheritdoc />  
    public ReadOnlyObservableCollection<MostRecentlyUsedFileItem> MostRecentlyUsedFiles
    {
      get => this.mostRecentlyUsedFiles;
      private set => TrySetValue(value, ref this.mostRecentlyUsedFiles);
    }

    private MostRecentlyUsedFileItem mostRecentlyUsedFile;
    /// <inheritdoc />
    public MostRecentlyUsedFileItem MostRecentlyUsedFile
    {
      get => this.mostRecentlyUsedFile;
      private set => TrySetValue(value, ref this.mostRecentlyUsedFile);
    }

    private int maxMostRecentlyUsedCount;
    /// <inheritdoc />
    public int MaxMostRecentlyUsedCount
    {
      get => this.maxMostRecentlyUsedCount;
      set
      {
        if (TrySetValue(value, IsMruCountValid, ref this.maxMostRecentlyUsedCount))
        {
          AppSettingsConnector.WriteString(MruManager.MaxRecentlyUsedCountKey, this.MaxMostRecentlyUsedCount.ToString());
        }
      }
    }

    /// <inheritdoc />
    public event EventHandler<ValueChangedEventArgs<MostRecentlyUsedFileItem>> FileAdded;

    private (bool IsValid, IEnumerable<string> ErrorMessages) IsMruCountValid(int count)
    {
      bool isValid = count > 0 && count < 100;
      (bool IsValid, IEnumerable<string> ErrorMessages) result = (isValid,
        isValid ? new List<string>() : new List<string>() {"Value must be between 1 and 100"});
      return result;
    }

    private ObservableCollection<MostRecentlyUsedFileItem> InternalMostRecentlyUsedFiles { get; }

    protected virtual void OnFileAdded(MostRecentlyUsedFileItem oldItem, MostRecentlyUsedFileItem newItem)
    {
      this.FileAdded?.Invoke(this, new ValueChangedEventArgs<MostRecentlyUsedFileItem>(oldItem, newItem));
    }
  }
}
