using System.Collections.Generic;

namespace BionicUtilities.NetStandard.IO
{
  public interface IFilePathFilter
  {
    IEnumerable<string> FilterFilePathsFromFolder(string folderPath, FileExtensions fileExtensionsToCollect, bool isIncludingSubdirectories);
    IEnumerable<string> FilterFilePathsFromMixedPathsIncludingFolders(IEnumerable<string> pathEntries, FileExtensions fileExtensionsToCollect, bool isIncludingSubdirectories);
    IEnumerable<string> FilterFilePathsFromMixedPathsIgnoringFolders(IEnumerable<string> pathEntries, FileExtensions fileExtensionsToCollect);
  }
}
