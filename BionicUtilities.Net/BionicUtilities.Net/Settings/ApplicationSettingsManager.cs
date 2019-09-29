using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using Hell.GlobalRepository.Command;
using Hell.GlobalRepository.ExtensionMethods;
using Hell.LogDown.Components.LogDocument;
using Hell.LogDown.Components.LogDocument.SyncModes;
using Hell.LogDown.IO;
using Hell.LogDown.Resources;
using Hell.LogDown.Settings.Data;
using Hell.LogDown.Settings.View;
using JetBrains.Annotations;

namespace Hell.LogDown.Settings
{
  public sealed class ApplicationSettingsManager : INotifyPropertyChanged
  {
    private const double DefaultWindowHeight = 700d;
    private const double DefaultWindowWidth = 1460d;
    private const string DefaultCustomFileExplorerExtensionFilter = "*";
    private const int MaxConcurrentDocumentsDefaultFallbackValue = 2;

    private ApplicationSettingsManager()
    {
      this.CommandLineApplicationPath = string.Empty;
      this.IsDocumentShowingHiddenTags = false;
      this.DefaultFilterIdValueCache = new Dictionary<string, bool>();
      this.IsAutoOpenSpecificFiles = true;
      this.SpecificAutoOpenFileNames = new ObservableCollection<string>();
      this.MergeRange = TimeSpan.FromMinutes(10);
      this.DocumentIdColorPresets = new ObservableCollection<IColorInfo>() { new ColorInfo(Colors.Teal), new ColorInfo(Colors.DarkGreen), new ColorInfo(Colors.DarkRed), new ColorInfo(Colors.Goldenrod), new ColorInfo(Colors.DarkSlateBlue), new ColorInfo(Colors.OrangeRed) };
      this.MergeRangeInLines = 200;
      this.LiveMergeRangeInLines = 100;
      this.ItemsControlPageSize = 2000;
      this.FileChunkSizeInLines = 1000;
      this.MaxNumberOfThreadsAllowed = Environment.ProcessorCount * 2;
      this.DeleteExtractedFilesOnAppClosingIsEnabled = true;
      this.HintTextIsEnabled = true;
      this.SynchronizationMode = SynchronizationModes.SyncByDateTime;
      this.IsSynchronizationEnabled = true;
      this.IsSyncAllOpenDocumentsEnabled = true;
      this.FileExplorerSettingsWriter = SettingsFileHandler.Instance.FileExplorerSettingsWriter;
      this.FileExplorerSettingsReader = SettingsFileHandler.Instance.FileExplorerSettingsReader;
      this.RecentFilesLimit = 30;
      this.RecentFiles = new ObservableCollection<FileInfo>();
      this.IsRestoringMainWindowPositionOnStart = true;
      this.IsLiveSearchEnabled = true;
      this.MainWindowState = WindowState.Normal;
    }

    public void InitializeApplicationSettings()
    {
      InitializeGeneralSettings();
      InitializeFileExplorerSettings();
      InitializeDocumentSynchronizationSettings();
    }

    private void InitializeDocumentSynchronizationSettings()
    {
      if (SettingsFileHandler.Instance.DocumentSynchronizationSettingsReader.TryReadSection(
        out SynchronizationSettingsSection settingsSection))
      {
        if (bool.TryParse(settingsSection.General[SynchronizationSettingsResources.IsSyncAllOpenDocumentsEnabled]?.Value, out bool isEnabled))
        {
          this.IsSyncAllOpenDocumentsEnabledCache = this.IsSyncAllOpenDocumentsEnabled = isEnabled;
        }

        if (Enum.TryParse(settingsSection.General[SynchronizationSettingsResources.SynchronizationMode]?.Value, out SynchronizationModes modes))
        {
          this.SynchronizationModeCache = this.SynchronizationMode = modes == SynchronizationModes.None ? SynchronizationModes.SyncByDateTime : modes;
        }
        else
        {
          this.SynchronizationModeCache = this.SynchronizationMode = SynchronizationModes.SyncByDateTime;
        }

        List<ColorElement> valueSettingsElements = settingsSection.MergedDocumentColors?.Cast<ColorElement>().ToList();
        if (valueSettingsElements?.Any() ?? false)
        {
          this.DocumentIdColorPresets?.Clear();
          this.DocumentIdColorPresets.AddRange(valueSettingsElements.ToList().Select(settingsValue => new ColorInfo(Color.FromArgb(byte.Parse(settingsValue.A), byte.Parse(settingsValue.R), byte.Parse(settingsValue.G), byte.Parse(settingsValue.B)))));
        }

        if (TimeSpan.TryParse(settingsSection.FileMerge[SynchronizationSettingsResources.MergeRangeInTime]?.Value, out TimeSpan timeRange))
        {
          this.MergeRange = timeRange;
        }

        if (Int32.TryParse(settingsSection.FileMerge[SynchronizationSettingsResources.LiveMergeRangeInLines]?.Value, out int liveMergeLineCount))
        {
          this.LiveMergeRangeInLines = liveMergeLineCount;
        }

        if (Int32.TryParse(settingsSection.FileMerge[SynchronizationSettingsResources.MergeRangeInLines]?.Value, out int mergeLineCount))
        {
          this.MergeRangeInLines = mergeLineCount;
        }
      }
    }

    public void SetSynchronizationMode(SynchronizationModes synchronizationModes, bool makeValueVisible = true)
    {
      this.SynchronizationModeCache = synchronizationModes;
      if (makeValueVisible)
      {
        this.SynchronizationMode = synchronizationModes;
      }
    }

    public void SetIsSyncAllOpenDocumentsEnabled(bool isEnabled, bool makeValueVisible = true)
    {
      this.IsSyncAllOpenDocumentsEnabledCache = isEnabled;
      if (makeValueVisible)
      {
        this.IsSyncAllOpenDocumentsEnabled = isEnabled;
      }
    }

    public void UpdateBinding(string propertyName)
    {
      OnPropertyChanged(propertyName);
    }
    
    private void InitializeGeneralSettings()
    {
      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
        GlobalSettingsResources.MaxConcurrentDocuments,
        out string concurrencyStringValue) && int.TryParse(concurrencyStringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int concurrencyValue))
      {
        this.MaxConcurrentOpeningDocuments = concurrencyValue < 1
          ? ApplicationSettingsManager.MaxConcurrentDocumentsDefaultFallbackValue
          : Math.Abs(concurrencyValue);
      }
      else
      {
        this.MaxConcurrentOpeningDocuments = ApplicationSettingsManager.MaxConcurrentDocumentsDefaultFallbackValue;
      }

      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
        GlobalSettingsResources.ExternalFileEditorPath,
        out string externalEditorPath))
      {
        this.CommandLineApplicationPath = externalEditorPath ?? string.Empty;
      }

      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
        GlobalSettingsResources.ExternalFileEditorCommandLineArgs,
        out string externalFileEditorCommandLineArgs))
      {
        this.CommandLineArguments = externalFileEditorCommandLineArgs ?? string.Empty;
      }

      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
        GlobalSettingsResources.IsRestoringMainWindowPositionOnStart,
        out string value))
      {
        if (Boolean.TryParse(value, out bool isRestoring))
        {
          this.IsRestoringMainWindowPositionOnStart = isRestoring;
        }
      }

      if (this.IsRestoringMainWindowPositionOnStart)
      {
        if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
              GlobalSettingsResources.MainWindowHeight,
              out value) 
            && double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double windowHeight))
        {
          this.MainWindowHeight = windowHeight > 0d ? windowHeight : ApplicationSettingsManager.DefaultWindowHeight;
        }
        else
        {
          this.MainWindowHeight = ApplicationSettingsManager.DefaultWindowHeight;
        }

        if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
              GlobalSettingsResources.MainWindowWidth,
              out value) 
            && double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double windowWidth))
        {
          this.MainWindowWidth = windowWidth > 0d ? windowWidth : ApplicationSettingsManager.DefaultWindowWidth;
        }
        else
        {
          this.MainWindowWidth = ApplicationSettingsManager.DefaultWindowWidth;
        }

        if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
              GlobalSettingsResources.MainWindowState,
              out value) 
            && Enum.TryParse(value, out WindowState windowState))
        {
          this.MainWindowState = windowState;
        }
      }

      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
        GlobalSettingsResources.IsMainToolBarAutoHideEnabled,
        out value))
      {
        if (Boolean.TryParse(value, out bool isEnabled))
        {
          this.IsMainToolBarAutoHideEnabled = isEnabled;
        }
      }
      
      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(
        GlobalSettingsResources.DocumentIsShowingHiddenTags,
        out value))
      {
        if (Boolean.TryParse(value, out bool isEnabled))
        {
          this.IsDocumentShowingHiddenTags = isEnabled;
        }
      }
      
      if (SettingsFileHandler.Instance.GeneralSettingsReader.TryReadValue(GlobalSettingsResources.IsLiveSearchEnabled,
        out value))
      {
        if (Boolean.TryParse(value, out bool isEnabled))
        {
          this.IsLiveSearchEnabled = isEnabled;
        }
      }
    }

    private void InitializeFileExplorerSettings()
    {
      IEnumerable<FileElement> configElements = this.FileExplorerSettingsReader
        .ReadSection<FileExplorerRecentFilesSection>(FileExplorerSettingsResources.RecentFileSectionName)?.RecentFiles?
        .Cast<FileElement>();
      List<FileInfo> recentFileInfos = configElements?
        .Select(configElement => new FileInfo(configElement.FilePath))?
        .ToList();
      recentFileInfos?.RemoveAll(fileInfo => !fileInfo.Exists);
      this.RecentFiles = recentFileInfos == null
        ? new ObservableCollection<FileInfo>()
        : new ObservableCollection<FileInfo>(recentFileInfos);

      this.RecentFilesLimit = this.FileExplorerSettingsReader
        .ReadSection<FileExplorerRecentFilesSection>(FileExplorerSettingsResources.RecentFileSectionName)
        ?.MaxFiles ?? this.RecentFilesLimit;

      this.DefaultFilterIdValueCache = 
        this.FileExplorerSettingsReader.ReadSection<FileExplorerDefaultFilterSection>(FileExplorerSettingsResources.DefaultFiltersSectionName)
                                         ?.Filters
                                         .Cast<KeyValueConfigurationElement>()
                                         .ToDictionary(
                                           (keyValueElement) => keyValueElement.Key,
                                           (keyValueElement) =>
                                           {
                                             if (bool.TryParse(keyValueElement.Value, out bool value))
                                             {
                                               return value;
                                             }
                                             return true;
                                           }) 
                                       ?? new Dictionary<string, bool>();

      // Initialize on first start (empty settings)
      if (!this.DefaultFilterIdValueCache.Any())
      {
        this.DefaultFilterIdValueCache.Add(nameof(this.ExplorerIsShowingAnyFiles), true);
        this.DefaultFilterIdValueCache.Add(nameof(this.ExplorerIsShowingArchiveFiles), true);
        this.DefaultFilterIdValueCache.Add(nameof(this.ExplorerIsShowingLogFiles), true);
        this.DefaultFilterIdValueCache.Add(nameof(this.ExplorerIsShowingTxtFiles), true);
        this.DefaultFilterIdValueCache.Add(nameof(this.ExplorerIsShowingIniFiles), true);
      }

      this.DefaultFilterIdValueCache.Keys.ToList().ForEach(
        (propertyName) =>
        {
          switch (propertyName)
          {
            case nameof(this.ExplorerIsShowingArchiveFiles):
              this.ExplorerIsShowingArchiveFiles = this.DefaultFilterIdValueCache[propertyName];
              break;
            case nameof(this.ExplorerIsShowingAnyFiles):
              this.ExplorerIsShowingAnyFiles = this.DefaultFilterIdValueCache[propertyName];
              break;
            case nameof(this.ExplorerIsShowingIniFiles):
              this.ExplorerIsShowingIniFiles = this.DefaultFilterIdValueCache[propertyName];
              break;
            case nameof(this.ExplorerIsShowingTxtFiles):
              this.ExplorerIsShowingTxtFiles = this.DefaultFilterIdValueCache[propertyName];
              break;
            case nameof(this.ExplorerIsShowingLogFiles):
              this.ExplorerIsShowingLogFiles = this.DefaultFilterIdValueCache[propertyName];
              break;
          }
        });

      this.IsCustomFileFilteringEnabled = this.FileExplorerSettingsReader.ReadSection<FileExplorerCustomFilterSection>(FileExplorerSettingsResources.CustomFiltersSectionName)
                                         ?.IsCustomFilterEnabled ?? false;

      this.CustomExplorerFilterValue = this.FileExplorerSettingsReader.ReadSection<FileExplorerCustomFilterSection>(FileExplorerSettingsResources.CustomFiltersSectionName)
                                         ?.Filters;

      if (string.IsNullOrWhiteSpace(this.CustomExplorerFilterValue))
      {
        this.CustomExplorerFilterValue = ApplicationSettingsManager.DefaultCustomFileExplorerExtensionFilter;
      }

      this.DeleteExtractedFilesOnAppClosingIsEnabled = this.FileExplorerSettingsReader
        .ReadSection<FileExplorerRecentFilesSection>(FileExplorerSettingsResources.RecentFileSectionName)
        ?.IsDeleteTemporaryFilesEnabled ?? true;

      this.SpecificAutoOpenFileNames.AddRange(this.FileExplorerSettingsReader
        .ReadSection<StringValuesSection>(Resources.FileExplorerSettingsResources.AutoOpenFilesSectionName)
        ?.AutoOpenFiles?.Cast<ValueSettingsElement>()?.Select((settingsElement) => settingsElement.Value) ?? new string[0]);

      this.AreFileExplorerSettingsInitialized = true;
      OnFileExplorerSettingsInitialized();
    }

    public void SavePendingChanges()
    {
      SaveFileExplorerSettings();
      SaveGeneralSettings();
      SaveDocumentSynchronizationSettings();
    }

    private void SaveDocumentSynchronizationSettings()
    {
      if (!Application.Current.Dispatcher.CheckAccess())
      {
        Application.Current.Dispatcher.InvokeAsync(SaveDocumentSynchronizationSettings);
        return;
      }

      SettingsFileHandler.Instance.DocumentSynchronizationSettingsWriter.WriteFileIdColorEntry(this.DocumentIdColorPresets.ToList().Select(color => new ColorElement(
        color.Color.R.ToString(),
        color.Color.G.ToString(),
        color.Color.B.ToString(),
        color.Color.A.ToString())));

      SettingsFileHandler.Instance.DocumentSynchronizationSettingsWriter.WriteFileMergeSettingsEntry(SynchronizationSettingsResources.MergeRangeInTime, this.MergeRange.ToString());

      SettingsFileHandler.Instance.DocumentSynchronizationSettingsWriter.WriteFileMergeSettingsEntry(SynchronizationSettingsResources.MergeRangeInLines, this.MergeRangeInLines.ToString());

      SettingsFileHandler.Instance.DocumentSynchronizationSettingsWriter.WriteFileMergeSettingsEntry(SynchronizationSettingsResources.LiveMergeRangeInLines, this.LiveMergeRangeInLines.ToString());

      SettingsFileHandler.Instance.DocumentSynchronizationSettingsWriter.WriteGeneralSettingsEntry(SynchronizationSettingsResources.IsSyncAllOpenDocumentsEnabled, this.IsSyncAllOpenDocumentsEnabledCache.ToString());

      SettingsFileHandler.Instance.DocumentSynchronizationSettingsWriter.WriteGeneralSettingsEntry(SynchronizationSettingsResources.SynchronizationMode, this.SynchronizationModeCache.ToString());
    }

    private void SaveGeneralSettings()
    {
      if (!Application.Current.Dispatcher.CheckAccess())
      {
        Application.Current.Dispatcher.InvokeAsync(SaveGeneralSettings);
        return;
      }

      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.MaxConcurrentDocuments, this.MaxConcurrentOpeningDocuments.ToString("D"));

      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.MainWindowState, Application.Current.MainWindow?.WindowState.ToString("G") ?? WindowState.Normal.ToString("G"));

      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.IsLiveSearchEnabled, this.IsLiveSearchEnabled.ToString());

      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.ExternalFileEditorPath, this.CommandLineApplicationPath);

      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.ExternalFileEditorCommandLineArgs, this.CommandLineArguments);

      if (Application.Current.MainWindow != null)
      {
        SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.MainWindowHeight, Application.Current.MainWindow.ActualHeight.ToString(NumberFormatInfo.InvariantInfo));
        SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.MainWindowWidth, Application.Current.MainWindow.ActualWidth.ToString(NumberFormatInfo.InvariantInfo));
      }
      
      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.IsRestoringMainWindowPositionOnStart, this.IsRestoringMainWindowPositionOnStart.ToString());
      
      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.IsMainToolBarAutoHideEnabled, ApplicationSettingsManager.Instance.IsMainToolBarAutoHideEnabled.ToString());
      
      SettingsFileHandler.Instance.GeneralSettingsWriter.WriteEntry(GlobalSettingsResources.DocumentIsShowingHiddenTags, ApplicationSettingsManager.Instance.IsDocumentShowingHiddenTags.ToString());
    }

    private void SaveFileExplorerSettings()
    {
      if (!Application.Current.Dispatcher.CheckAccess())
      {
        Application.Current.Dispatcher.InvokeAsync(SaveFileExplorerSettings);
        return;
      }

      if (this.FileExplorerSettingIsPending)
      {
        if (this.RecentFilesLimit > 0 && this.RecentFiles.Any())
        {
          SettingsFileHandler.Instance.FileExplorerSettingsWriter?.WriteRecentFilesData(
            this.RecentFiles.Distinct(EqualityComparer<FileInfo>.Default),
            this.RecentFilesLimit);
        }
        else
        {
          SettingsFileHandler.Instance.FileExplorerSettingsWriter?.ClearRecentFilesSection();
        }

        SettingsFileHandler.Instance.FileExplorerSettingsWriter?.WriteAreCustomFileFiltersEnabled(this.IsCustomFileFilteringEnabled);
        SettingsFileHandler.Instance.FileExplorerSettingsWriter?.WriteDefaultFileFilters(this.DefaultFilterIdValueCache);
        SettingsFileHandler.Instance.FileExplorerSettingsWriter?.WriteCustomFileFilters(this.CustomExplorerFilterValue);

        SettingsFileHandler.Instance.FileExplorerSettingsWriter?.WriteIsDeleteTemporaryFilesEnabled(this.DeleteExtractedFilesOnAppClosingIsEnabled);
        SettingsFileHandler.Instance.FileExplorerSettingsWriter?.WriteRecentFilesLimit(this.RecentFilesLimit);
        this.FileExplorerSettingsWriter.WriteAutoOpenFilesData(this.SpecificAutoOpenFileNames);

        this.FileExplorerSettingIsPending = false;
      }
    }

    private void SetFlagToNewStateOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.IsAutoOpenSpecificFiles = this.SpecificAutoOpenFileNames.Any();
    }

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private static ApplicationSettingsManager _instance;
    private static readonly object _syncLock = new object();
    private bool FileExplorerSettingIsPending { get; set; }
    private IFileExplorerSettingsWriter FileExplorerSettingsWriter { get; set; }
    private IFileExplorerSettingsReader FileExplorerSettingsReader { get; set; }


    public ICommand ClearRecentFilesCommand =>
      new RelayCommand((param) => this.RecentFiles.Clear(), (param) => this.RecentFiles.Any());
    public ICommand AddNewMergeColorCommand =>
      new RelayCommand((param) => this.DocumentIdColorPresets?.Add(new ColorInfo()), (param) => true);
    public ICommand RemoveMergeColorCommand =>
      new RelayCommand((param) => this.DocumentIdColorPresets?.Remove(param as IColorInfo), (param) => param is IColorInfo);

    public static ApplicationSettingsManager Instance
    {
      get
      {
        if (ApplicationSettingsManager._instance == null)
        {
          lock (ApplicationSettingsManager._syncLock)
          {
            if (ApplicationSettingsManager._instance == null)
            {
              ApplicationSettingsManager._instance = new ApplicationSettingsManager();
            }
          }
        }

        return ApplicationSettingsManager._instance;
      }
      private set { ApplicationSettingsManager._instance = value; }
    }

    #region FileExplorer Settings

    private bool deleteExtractedFilesOnAppClosingIsEnabled;
    public bool DeleteExtractedFilesOnAppClosingIsEnabled
    {
      get { return this.deleteExtractedFilesOnAppClosingIsEnabled; }
      set
      {
        this.deleteExtractedFilesOnAppClosingIsEnabled = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
      }
    }

    private bool hintTextIsEnabled;
    public bool HintTextIsEnabled
    {
      get { return this.hintTextIsEnabled; }
      set
      {
        this.hintTextIsEnabled = value;
        OnPropertyChanged();
      }
    }

    private bool isSynchronizationEnabled;
    public bool IsSynchronizationEnabled
    {
      get => this.isSynchronizationEnabled && !this.SynchronizationMode.Equals(SynchronizationModes.None) 
             || this.IsSyncAllOpenDocumentsEnabled;
      set
      {
        this.isSynchronizationEnabled = value;
        OnPropertyChanged();
      }
    }

    private bool isCustomFileFilteringEnabled;
    public bool IsCustomFileFilteringEnabled
    {
      get => this.isCustomFileFilteringEnabled;
      set
      {
        this.isCustomFileFilteringEnabled = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
      }
    }

    private string customExplorerFilterValue;   
    public string CustomExplorerFilterValue
    {
      get { return this.customExplorerFilterValue; }
      set 
      { 
        this.customExplorerFilterValue = value; 
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
      }
    }

    private bool explorerIsShowingAnyFiles;
    public bool ExplorerIsShowingAnyFiles
    {
      get => this.explorerIsShowingAnyFiles;
      set
      {
        this.explorerIsShowingAnyFiles = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
        this.DefaultFilterIdValueCache[nameof(this.ExplorerIsShowingAnyFiles)] = this.ExplorerIsShowingAnyFiles;
      }
    }

    private bool explorerIsShowingTxtFiles;
    public bool ExplorerIsShowingTxtFiles
    {
      get => this.explorerIsShowingTxtFiles;
      set
      {
        this.explorerIsShowingTxtFiles = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
        this.DefaultFilterIdValueCache[nameof(this.ExplorerIsShowingTxtFiles)] = this.ExplorerIsShowingTxtFiles;
      }
    }

    private bool explorerIsShowingIniFiles;
    public bool ExplorerIsShowingIniFiles
    {
      get => this.explorerIsShowingIniFiles;
      set
      {
        this.explorerIsShowingIniFiles = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
        this.DefaultFilterIdValueCache[nameof(this.ExplorerIsShowingIniFiles)] = this.ExplorerIsShowingIniFiles;
      }
    }

    private bool explorerIsShowingLogFiles;
    public bool ExplorerIsShowingLogFiles
    {
      get => this.explorerIsShowingLogFiles;
      set
      {
        this.explorerIsShowingLogFiles = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
        this.DefaultFilterIdValueCache[nameof(this.ExplorerIsShowingLogFiles)] = this.ExplorerIsShowingLogFiles;
      }
    }

    private bool explorerIsShowingArchiveFiles;
    public bool ExplorerIsShowingArchiveFiles
    {
      get => this.explorerIsShowingArchiveFiles;
      set
      {
        this.explorerIsShowingArchiveFiles = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
        this.DefaultFilterIdValueCache[nameof(this.ExplorerIsShowingArchiveFiles)] = this.ExplorerIsShowingArchiveFiles;
      }
    }

    private ObservableCollection<FileInfo> recentFiles;
    public ObservableCollection<FileInfo> RecentFiles
    {
      get { return this.recentFiles; }
      set
      {
        if (this.RecentFiles != null)
        {
          this.RecentFiles.CollectionChanged -= HandleRecentFileCollectionChanged;
        }

        this.recentFiles = value;
        if (this.RecentFiles != null)
        {
          this.RecentFiles.CollectionChanged += HandleRecentFileCollectionChanged;
        }

        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
      }
    }

    public bool HasRecentFiles => this.RecentFiles?.Any() ?? false;

    private void HandleRecentFileCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      List<FileInfo> distinctFileInfos = this.RecentFiles.Distinct().ToList();
      if (distinctFileInfos.Count() != this.RecentFiles.Count)
      {
        this.RecentFiles = new ObservableCollection<FileInfo>(distinctFileInfos);
      }

      this.FileExplorerSettingIsPending = true;
    }

    private int recentFilesLimit;
    public int RecentFilesLimit
    {
      get => this.recentFilesLimit;
      set
      {
        this.recentFilesLimit = value;
        this.FileExplorerSettingIsPending = true;
        OnPropertyChanged();
      }
    }

    private bool isAutoOpenSpecificFiles;
    public bool IsAutoOpenSpecificFiles
    {
      get => this.isAutoOpenSpecificFiles;
      set
      {
        this.isAutoOpenSpecificFiles = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<string> specificAutoOpenFileNames;
    public ObservableCollection<string> SpecificAutoOpenFileNames
    {
      get => this.specificAutoOpenFileNames;
      set
      {
        if (this.SpecificAutoOpenFileNames != null)
        {
          this.SpecificAutoOpenFileNames.CollectionChanged -= SetFlagToNewStateOnCollectionChanged;
        }
        this.FileExplorerSettingIsPending = true;
        this.specificAutoOpenFileNames = value;
        this.SpecificAutoOpenFileNames.CollectionChanged += SetFlagToNewStateOnCollectionChanged;

        OnPropertyChanged();
      }
    }

    private Dictionary<string, bool> DefaultFilterIdValueCache { get; set; }

    #endregion

    private SynchronizationModes SynchronizationModeCache { get; set; }
    private SynchronizationModes synchronizationMode;
    public SynchronizationModes SynchronizationMode
    {
      get { return this.synchronizationMode; }
      set
      {
        this.synchronizationMode = value;
        OnPropertyChanged();
      }
    }

    

    private ObservableCollection<IColorInfo> documentIdColorPresets;
    public ObservableCollection<IColorInfo> DocumentIdColorPresets
    {
      get => this.documentIdColorPresets;
      set
      {
        //if (object.ReferenceEquals(this.DocumentIdColorPresets, LineIdColorFactory.DefaultColorsReservoir))
        //{
        //  return;
        //}

        this.documentIdColorPresets = value;
        //UpdateLineIdColorFactoryWithNewColors();
        OnPropertyChanged();
      }
    }

    public void UpdateLineIdColorFactoryWithNewColors()
    {
      LineIdColorFactory.DefaultColorsReservoir = this.DocumentIdColorPresets;
    }

    private TimeSpan mergeRange;   
    public TimeSpan MergeRange
    {
      get => this.mergeRange;
      set 
      { 
        this.mergeRange = value; 
        OnPropertyChanged();
      }
    }

    private int itemsControlPageSize;
    /// <summary>
    /// Number of items preloaded to be displayed in a virtualized ItemsControl
    /// </summary>
    public int ItemsControlPageSize
    {
      get => this.itemsControlPageSize;
      set
      {
        this.itemsControlPageSize = value;
        OnPropertyChanged();
      }
    }

    private int mergeRangeInLines;   
    public int MergeRangeInLines
    {
      get => this.mergeRangeInLines;
      set 
      { 
        this.mergeRangeInLines = value; 
        OnPropertyChanged();
      }
    }

    private int liveMergeRangeInLines;   
    public int LiveMergeRangeInLines
    {
      get => this.liveMergeRangeInLines;
      set 
      { 
        this.liveMergeRangeInLines = value; 
        OnPropertyChanged();
      }
    }

    private int fileChunkSizeInLines;
    public int FileChunkSizeInLines
    {
      get => this.fileChunkSizeInLines;
      set
      {
        this.fileChunkSizeInLines = value;
        OnPropertyChanged();
      }
    }

    private int maxNumberOfThreads;
    public int MaxNumberOfThreadsAllowed
    {
      get => this.maxNumberOfThreads;
      set
      {
        this.maxNumberOfThreads = value;
        OnPropertyChanged();
      }
    }

    private bool IsSyncAllOpenDocumentsEnabledCache { get; set; }
    private bool isSyncAllOpenDocumentsEnabled;   
    public bool IsSyncAllOpenDocumentsEnabled
    {
      get => this.isSyncAllOpenDocumentsEnabled;
      set 
      {
        if (value.Equals(this.isSyncAllOpenDocumentsEnabled))
        {
          return;
        }

        this.isSyncAllOpenDocumentsEnabled = value; 
        OnPropertyChanged();
      }
    }

    #region general settings

    private int maxConcurrentOpeningDocuments;   
    public int MaxConcurrentOpeningDocuments
    {
      get { return this.maxConcurrentOpeningDocuments; }
      set 
      { 
        this.maxConcurrentOpeningDocuments = value; 
        OnPropertyChanged();
      }
    }

    private string commandLineApplicationPath;   
    public string CommandLineApplicationPath
    {
      get { return this.commandLineApplicationPath; }
      set 
      { 
        this.commandLineApplicationPath = value; 
        OnPropertyChanged();
      }
    }

    private string commandLineArguments;   
    public string CommandLineArguments
    {
      get { return this.commandLineArguments; }
      set 
      { 
        this.commandLineArguments = value; 
        OnPropertyChanged();
      }
    }

    private bool isDocumentShowingHiddenTags;   
    public bool IsDocumentShowingHiddenTags
    {
      get { return this.isDocumentShowingHiddenTags; }
      set 
      { 
        this.isDocumentShowingHiddenTags = value; 
        OnPropertyChanged();
      }
    }

    private bool isRestoringMainWindowPositionOnStart;
    public bool IsRestoringMainWindowPositionOnStart
    {
      get => this.isRestoringMainWindowPositionOnStart;
      set
      {
        this.isRestoringMainWindowPositionOnStart = value;
        OnPropertyChanged();
      }
    }

    private WindowState mainWindowState;
    public WindowState MainWindowState
    {
      get => this.mainWindowState;
      set
      {
        this.mainWindowState = value;
        OnPropertyChanged();
      }
    }

    private double mainWindowHeight;   
    public double MainWindowHeight
    {
      get { return this.mainWindowHeight; }
      set 
      { 
        this.mainWindowHeight = value; 
        OnPropertyChanged();
      }
    }

    private double mainWindowWidth;   
    public double MainWindowWidth
    {
      get { return this.mainWindowWidth; }
      set 
      { 
        this.mainWindowWidth = value; 
        OnPropertyChanged();
      }
    }

    private bool isMainToolBarAutoHideEnabled;
    public bool IsMainToolBarAutoHideEnabled
    {
      get => this.isMainToolBarAutoHideEnabled;
      set
      {
        this.isMainToolBarAutoHideEnabled = value;
        OnPropertyChanged();
      }
    }

    #endregion

    private bool isLiveSearchEnabled;   
    public bool IsLiveSearchEnabled
    {
      get => this.isLiveSearchEnabled;
      set 
      { 
        this.isLiveSearchEnabled = value; 
        OnPropertyChanged();
      }
    }

    public bool AreFileExplorerSettingsInitialized { get; private set; }
    public event EventHandler FileExplorerSettingsInitialized;

    private void OnFileExplorerSettingsInitialized()
    {
      this.FileExplorerSettingsInitialized?.Invoke(this, EventArgs.Empty);
    }
  }
}
