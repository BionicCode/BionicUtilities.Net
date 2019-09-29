using System;
using System.Collections.Generic;
using System.IO;
using Hell.LogDown.Settings.Data;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings
{
  public interface IFileExplorerSettingsWriter
  {
    void WriteRecentFilesData(IEnumerable<FileInfo> newFileInfos, int maxRecentFiles);
    void WriteRecentFilesData(FileInfo newFileInfo, int maxRecentFiles);
    void WriteSection(FileExplorerRecentFilesSection newSection);
    void WriteAutoOpenFilesData([NotNull] IEnumerable<string> autoOpenFileNames);
    void WriteIsDeleteTemporaryFilesEnabled(bool isDeleteTemporaryFilesEnabled);
    void WriteRecentFilesLimit(int maxFiles);
    void WriteDefaultFileFilters(Dictionary<string, bool> filterValues);
    void WriteCustomFileFilters(string filterValues);
    void WriteAreCustomFileFiltersEnabled(bool isEnabled);
    void ClearRecentFilesSection();
    event EventHandler<DataChangedEventArgs<IEnumerable<FileInfo>>> RecentFilesChanged;
  }
}