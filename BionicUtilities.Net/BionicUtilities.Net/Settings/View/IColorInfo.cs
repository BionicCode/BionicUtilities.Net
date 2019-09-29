using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using Hell.GlobalRepository.Contracts;
using Xceed.Wpf.Toolkit;

namespace Hell.LogDown.Settings.View
{
  public interface IColorInfo : INullObject<IColorInfo>, INotifyPropertyChanged
  {
    Color Color { get; set; }
    ColorPicker ColorPicker { get; }
  }
}