#region Info
// //  
// BionicCode.BionicNuGetDeploy.Main
#endregion

using System.Collections.ObjectModel;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicUtilities.Net.Settings
{
  public interface IMruManager : IViewModel
  {
    void AddMostRecentlyUsedFile(string filePath);
    ReadOnlyObservableCollection<MostRecentlyUsedFileItem> MostRecentlyUsedFiles { get; }
    MostRecentlyUsedFileItem MostRecentlyUsedFile { get; }
    int MaxMostRecentlyUsedCount { get; set; }
  }
}