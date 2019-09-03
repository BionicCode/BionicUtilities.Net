using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BionicLibraryNet.Extensions
{
  public static class HelperExtensions
  {
    public static bool TryFindVisualParentElement<TParent>(this DependencyObject child, out TParent resultElement) where TParent : DependencyObject
    {
      resultElement = null;

      if (child == null)
        return false;

      DependencyObject parentElement = VisualTreeHelper.GetParent(child);

      if (parentElement is TParent parent)
      {
        resultElement = parent;
        return true;
      }

      return parentElement.TryFindVisualParentElement(out resultElement);
    }

    public static bool TryFindVisualChildElement<TChild>(this DependencyObject parent, out TChild resultElement) where TChild : DependencyObject
    {
      resultElement = null;
      for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); childIndex++)
      {
        DependencyObject childElement = VisualTreeHelper.GetChild(parent, childIndex);

        if (childElement is Popup popup)
        {
          childElement = popup.Child;
        }

        if (childElement is TChild child)
        {
          resultElement = child;
          return true;
        }

        if (childElement.TryFindVisualChildElement(out resultElement))
        {
          return true;
        }
      }

      return false;
    }

    public static bool TryFindVisualChildElementByName(this DependencyObject parent, string childElementName, out FrameworkElement resultElement)
    {
      resultElement = null;
      for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); childIndex++)
      {
        DependencyObject childElement = VisualTreeHelper.GetChild(parent, childIndex);

        if (childElement is Popup popup)
        {
          childElement = popup.Child;
        }

        if (childElement is FrameworkElement uiElement && uiElement.Name.Equals(childElementName, StringComparison.OrdinalIgnoreCase))
        {
          resultElement = uiElement;
          return true;
        }

        if (childElement.TryFindVisualChildElementByName(childElementName, out resultElement))
        {
          return true;
        }
      }

      return false;
    }

    public static IEnumerable<TChildren> FindVisualChildElements<TChildren>(this DependencyObject parent) where  TChildren : DependencyObject
    {
      for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(parent); childIndex++)
      {
        DependencyObject childElement = VisualTreeHelper.GetChild(parent, childIndex);

        if (childElement is Popup popup)
        {
          childElement = popup.Child;
        }

        if (childElement is TChildren element)
        {
          yield return element;
        }

        foreach (TChildren visualChildElement in childElement.FindVisualChildElements<TChildren>())
        {
          yield return visualChildElement;
        }
      }
    }
  }
}
