using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Hell.GlobalRepository.Contracts;
using JetBrains.Annotations;
using Xceed.Wpf.Toolkit;

namespace Hell.LogDown.Settings.View
{
  public class ColorInfo : INotifyPropertyChanged, IColorInfo
  {
    public ColorInfo()
    {
      this.ColorPicker = new ColorPicker();
      this.ColorPicker.SelectedColorChanged += UpdateChangedColor;
      this.IsNull = false;
      this.Color = Colors.Black;
    }

    public ColorInfo(Color color) : this()
    {
      this.Color = color;
    }

    protected ColorInfo(bool isNullObject) : this(Colors.Transparent)
    {
      this.IsNull = true;
    }

    private void UpdateChangedColor(object sender, RoutedPropertyChangedEventArgs<Color?> e)
    {
      if (e.NewValue?.Equals(this.Color) ?? true)
      {
        return;
      }

      this.Color = e.NewValue.Value;
    }

    private Color color;   
    public Color Color
    {
      get => this.color;
      set 
      { 
        this.color = value; 
        OnPropertyChanged();
      }
    }

    private ColorPicker colorPicker;   
    public ColorPicker ColorPicker
    {
      get => this.colorPicker;
      private set
      {
        this.colorPicker = value; 
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <inheritdoc />
    public bool IsNull { get; }

    /// <inheritdoc />
    IColorInfo INullObject<IColorInfo>.NullObject
    {
      get => ColorInfo.NullObject;
    }

    /// <inheritdoc />
    public static IColorInfo NullObject => new ColorInfo(true);
  }
}
