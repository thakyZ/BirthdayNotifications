using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Windows.Media.Animation;

namespace BirthdayNotifications.Utils {
  #nullable enable
  internal static class Extensions {
    /// <summary>
    /// Returns full visual ancestry, starting at the leaf.
    /// <para>If element is not of <see cref="Visual"/> or <see cref="Visual3D"/> the
    /// logical ancestry is used.</para>
    /// </summary>
    /// <param name="leaf"></param>
    /// <returns></returns>
    public static IEnumerable<DependencyObject> GetVisualAncestry(this DependencyObject? leaf) {
      while (leaf is not null) {
        yield return leaf;
        leaf = leaf is Visual || leaf is Visual3D
            ? VisualTreeHelper.GetParent(leaf)
            : LogicalTreeHelper.GetParent(leaf);
      }
    }

    public static void ListBoxMouseButtonEvent(ListBoxItem sender) {
      var senderElement = (UIElement)sender;

      if (!GetIsToggle(senderElement)) return;

      var point = senderElement.PointToScreen(new Point(0, sender.Height / 2));
      var result = VisualTreeHelper.HitTest(senderElement, point);

      if (result is null) return;

      ListBoxItem? listBoxItem = null;
      Ripple? ripple = null;
      foreach (var dependencyObject in result.VisualHit.GetVisualAncestry().TakeWhile(_ => listBoxItem is null)) {
        listBoxItem = dependencyObject as ListBoxItem;
        if (ripple is null)
          ripple = dependencyObject as Ripple;
      }

      if (listBoxItem is null || !listBoxItem.IsEnabled) return;

      listBoxItem.SetCurrentValue(ListBoxItem.IsSelectedProperty, !listBoxItem.IsSelected);

      if (ripple != null && listBoxItem.IsSelected) {
        VisualStateManager.GoToState(ripple, "MousePressed", true);
      }
    }

    private static void PlayRipple(Ripple ripple, ListBoxItem sender) {
      // adjust the transition scale time according to the current animated scale
      var scaleTrans = ripple.Template.FindName("ScaleTransform", ripple) as ScaleTransform;
      if (scaleTrans != null) {
        double currentScale = scaleTrans.ScaleX;
        var newTime = TimeSpan.FromMilliseconds(300 * (1.0 - currentScale));

        // change the scale animation according to the current scale
        var scaleXKeyFrame = ripple.Template.FindName("MousePressedToNormalScaleXKeyFrame", ripple) as EasingDoubleKeyFrame;
        if (scaleXKeyFrame != null) {
          scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
        }
        var scaleYKeyFrame = ripple.Template.FindName("MousePressedToNormalScaleYKeyFrame", ripple) as EasingDoubleKeyFrame;
        if (scaleYKeyFrame != null) {
          scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(newTime);
        }
      }

      VisualStateManager.GoToState(ripple, "Normal", true);
    }

    public static readonly DependencyProperty IsToggleProperty = DependencyProperty.RegisterAttached(
            "IsToggle", typeof(bool), typeof(ListBoxAssist), new FrameworkPropertyMetadata(default(bool)));

    public static void SetIsToggle(DependencyObject element, bool value)
        => element.SetValue(IsToggleProperty, value);

    public static bool GetIsToggle(DependencyObject element)
        => (bool)element.GetValue(IsToggleProperty);
  }
  #nullable disable
}