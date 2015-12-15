/*
<License>
Copyright 2015 Virtium Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http ://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</License>
 */

using System.Windows;
using System.Windows.Input;

namespace BuildEventer.Behaviors
{
    public static class CommonBehavior
    {
        #region Load
        public static DependencyProperty LoadedCommandProperty = DependencyProperty.RegisterAttached
            (
                "LoadedCommand",
                typeof(ICommand),
                typeof(CommonBehavior),
                new PropertyMetadata(null, OnLoadedCommandChanged)
            );

        public static ICommand GetLoadedCommand(DependencyObject dependencyObject)
        {
            return (ICommand)dependencyObject.GetValue(LoadedCommandProperty);
        }

        public static void SetLoadedCommand(DependencyObject depObj, ICommand value)
        {
            depObj.SetValue(LoadedCommandProperty, value);
        }

        private static void OnLoadedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
            if ((null != frameworkElement) && (e.NewValue is ICommand))
            {
                frameworkElement.Loaded += (o, args) =>
                {
                    (e.NewValue as ICommand).Execute(null);
                };
            }
        }
        #endregion

        #region Focus
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached
            (
                "IsFocused",
                typeof(bool),
                typeof(CommonBehavior),
                new UIPropertyMetadata(false, OnIsFocusedPropertyChanged)
            );

        public static bool GetIsFocused(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFocusedProperty, value);
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            UIElement uie = (UIElement)dependencyObject;
            if (false == ((bool)e.NewValue))
            {
                return;
            }
            uie.Focus();
            uie.LostFocus += UieOnLostFocus;
            uie.UpdateLayout();
        }

        private static void UieOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            UIElement uie = sender as UIElement;
            if (null == uie)
            {
                return;
            }
            uie.LostFocus -= UieOnLostFocus;
            uie.SetValue(IsFocusedProperty, false);
        }
        #endregion

        #region DialogResult
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached
            (
                "DialogResult",
                typeof(bool?),
                typeof(CommonBehavior),
                new PropertyMetadata(DialogResultChanged)
            );

        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }

        public static bool? GetDialogResult(Window target)
        {
            return (bool?)target.GetValue(DialogResultProperty);
        }

        private static void DialogResultChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Window window = dependencyObject as Window;
            if (null != window)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }
        #endregion DialogResult
    }
}
