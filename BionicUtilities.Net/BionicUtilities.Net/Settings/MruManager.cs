using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        this.maxMostRecentlyUsedCount = 1;
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

    /// <summary>
    /// Adds a file with the specified path to the MRU table.
    /// </summary>
    /// <param name="filePath">The path to the file which is to add to the MRU table.</param>
    /// <remarks>Checks if the file exists. Does nothing if file doesn't exist. When the number of files in the MRU table exceeds the limit set by <see cref="MaxMostRecentlyUsedCount"/> the entry with the least recent access is removed from the table.</remarks>
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
        if (this.InternalMostRecentlyUsedFiles.Count >= this.MaxMostRecentlyUsedCount)
        {
          this.InternalMostRecentlyUsedFiles.RemoveAt(0);
        }

        var mostRecentlyUsedFileItem = new MostRecentlyUsedFileItem(new FileInfo(filePath));
        this.InternalMostRecentlyUsedFiles.Add(mostRecentlyUsedFileItem);
      }

      this.MostRecentlyUsedFile = this.InternalMostRecentlyUsedFiles.Last();

      string mruListString = string.Join(MruManager.MostRecentlyUsedKeyStringSeparator, this.InternalMostRecentlyUsedFiles.Select(mruItem => mruItem.FullName));
      AppSettingsConnector.WriteString(MruManager.MostRecentlyUsedKey, mruListString);
    }

    private ReadOnlyObservableCollection<MostRecentlyUsedFileItem> mostRecentlyUsedFiles;   
    /// <summary>
    /// A <see cref="ReadOnlyObservableCollection{T}"/> collection of <see cref="MostRecentlyUsedFileItem"/> which contains the MRU files.
    /// </summary>
    public ReadOnlyObservableCollection<MostRecentlyUsedFileItem> MostRecentlyUsedFiles
    {
      get => this.mostRecentlyUsedFiles;
      private set => TrySetValue(value, ref this.mostRecentlyUsedFiles);
    }

    private MostRecentlyUsedFileItem mostRecentlyUsedFile;   
    /// <summary>
    /// Gets the MRU file which is the last file added to the MRU table.
    /// </summary>
    public MostRecentlyUsedFileItem MostRecentlyUsedFile
    {
      get => this.mostRecentlyUsedFile;
      private set => TrySetValue(value, ref this.mostRecentlyUsedFile);
    }

    private int maxMostRecentlyUsedCount;   
    /// <summary>
    /// The maximum number of files that are kept in the MRU table.
    /// </summary>
    /// <remarks>When the limit is exceeded, the least recent used file will be removed from the MRU table every time a new file is added.</remarks>
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

    private (bool IsValid, IEnumerable<string> ErrorMessages) IsMruCountValid(int count)
    {
      bool isValid = count > 0 && count < 100;
      (bool IsValid, IEnumerable<string> ErrorMessages) result = (isValid,
        isValid ? new List<string>() : new List<string>() {"Value must be between 1 and 100"});
      return result;
    }

    private ObservableCollection<MostRecentlyUsedFileItem> InternalMostRecentlyUsedFiles { get; }
  }
}
