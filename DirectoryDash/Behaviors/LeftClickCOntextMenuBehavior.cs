using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    namespace DirectoryDash.Behaviors
    {
        public static class LeftClickContextMenuBehavior
        {
            public static bool GetOpenOnLeftClick(DependencyObject obj)
                => (bool)obj.GetValue(OpenOnLeftClickProperty);

            public static void SetOpenOnLeftClick(DependencyObject obj, bool value)
                => obj.SetValue(OpenOnLeftClickProperty, value);

            public static readonly DependencyProperty OpenOnLeftClickProperty =
                DependencyProperty.RegisterAttached(
                    "OpenOnLeftClick",
                    typeof(bool),
                    typeof(LeftClickContextMenuBehavior),
                    new UIPropertyMetadata(false, OnOpenOnLeftClickChanged));

            private static void OnOpenOnLeftClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                if (d is UIElement element)
                {
                    if ((bool)e.NewValue)
                        element.PreviewMouseLeftButtonDown += Element_PreviewMouseLeftButtonDown;
                    else
                        element.PreviewMouseLeftButtonDown -= Element_PreviewMouseLeftButtonDown;
                }
            }

            private static void Element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                if (sender is FrameworkElement fe && fe.ContextMenu != null)
                {
                    fe.ContextMenu.PlacementTarget = fe;
                    fe.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    fe.ContextMenu.IsOpen = true;
                    e.Handled = true;
                }
            }
        }
    }
}
