using System.IO;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicUtilities.Net.Settings
{
  /// <summary>
  /// An immutable item that represents a Most Recently Used file (MRU) table entry.
  /// </summary>
  public class MostRecentlyUsedFileItem : BaseViewModel
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="fileInfo">The underlying <see cref="FileInfo"/> of the item.</param>
    public MostRecentlyUsedFileItem(FileInfo fileInfo)
    {
      this.FileInfo = fileInfo;
    }

    /// <summary>
    /// Return the underlying <see cref="FileInfo"/> of this instance.
    /// </summary>
    public FileInfo FileInfo { get; }
    /// <summary>
    /// Returns the file name including the extension.
    /// </summary>
    public string Name => this.FileInfo.Name;
    /// <summary>
    /// Returns the full file path of the file.
    /// </summary>
    public string FullName => this.FileInfo.FullName;
  }
}
