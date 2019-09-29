using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hell.GlobalRepository;
using Hell.GlobalRepository.Command;
using Hell.LogDown.Settings.View.Generic;
using Microsoft.Win32;

namespace Hell.LogDown.Settings.View
{
  public interface ICommandLineAppConfigurationSettingsData : ISettingsPageData<string>
  {
    ProcessStartInfo GetProcessStartInfo(FileInfo fileToOpen);
    ICommand SearchExecutableCommand { get; }
    string Arguments { get; set; }
    string ExecutablePath { get; set; }
    FileInfo Executable { get; set; }
  }

  public class CommandLineAppConfigurationSettingsData : ICommandLineAppConfigurationSettingsData
  {
    public CommandLineAppConfigurationSettingsData()
    {
      this.Arguments = string.Empty;
    }

    public CommandLineAppConfigurationSettingsData(FileInfo executable, string arguments)
    {
      this.Arguments = arguments;
      this.ExecutablePath = (executable?.Exists ?? false) ? executable.FullName : string.Empty;
      this.Executable = executable;
    }

    public ProcessStartInfo GetProcessStartInfo(FileInfo fileToOpen) => new ProcessStartInfo() { Arguments = this.Arguments, FileName = fileToOpen.FullName };

    private async Task OpenWindowsFileExplorerAsync()
    {
      var windowsExplorer = new OpenFileDialog()
      {
        CheckFileExists = true,
        CheckPathExists = true,
        AddExtension = true,
        Multiselect = false,
        Filter = "Executables|*.exe;*.cmd;*.bat|All Files|*.*"
      };

      if (windowsExplorer.ShowDialog() ?? false)
      {
        this.Executable = new FileInfo(windowsExplorer.FileName);
        this.ExecutablePath = windowsExplorer.FileName;
        ApplicationSettingsManager.Instance.CommandLineApplicationPath = windowsExplorer.FileName;
      }
    }

    /// <inheritdoc />
    public ICommand SearchExecutableCommand => new RelayCommand(async (param) => await OpenWindowsFileExplorerAsync(), (param) => true);

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <inheritdoc />
    public Guid Guid { get; }

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc />
    public string Description { get; set; }

    /// <inheritdoc />
    public string DisplaySettingKey { get; set; }

    /// <inheritdoc />
    public bool AffectsPerformance { get; set; }

    /// <inheritdoc />
    public event EventHandler<ValueChangedEventArgs<string>> ValueChanged;

    /// <inheritdoc />
    public string DisplaySettingValue { get; set; }

    /// <inheritdoc />
    public string DefaultDisplaySettingValue { get; set; }

    /// <inheritdoc />
    public string Arguments { get; set; }

    /// <inheritdoc />
    public FileInfo Executable { get; set; }

    private string executablePath;

    public string ExecutablePath
    {
      get => this.executablePath;
      set {
        this.executablePath = value;
        OnValueChanged(new ValueChangedEventArgs<string>(this.ExecutablePath));}
    }

    protected virtual void OnValueChanged(ValueChangedEventArgs<string> e)
    {
      this.ValueChanged?.Invoke(this, e);
    }
  }
}
