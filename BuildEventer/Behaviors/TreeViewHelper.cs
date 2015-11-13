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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BuildEventer.Behaviors
{
    public class TreeViewHelper
    {
        #region SelectedItem
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.RegisterAttached
            (
                "SelectedItem",
                typeof(object),
                typeof(TreeViewHelper),
                new UIPropertyMetadata(null, SelectedItemChanged)
            );

        public static object GetSelectedItem(DependencyObject dependencyObject)
        {
            return (object)dependencyObject.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(SelectedItemProperty, value);
        }

        private static void SelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (false == (dependencyObject is TreeView) || null == e.NewValue)
            {
                return;
            }

            TreeView view = dependencyObject as TreeView;

            view.SelectedItemChanged += (sender, e2) =>
            {
                SetSelectedItem(view, e2.NewValue);
            };

            ICommand command = (ICommand)(view as DependencyObject).GetValue(SelectedItemChangedProperty);
            if (null != command)
            {
                if (true == command.CanExecute(null))
                {
                    command.Execute(new DependencyPropertyEventArgs(e));
                }
            }
        }
        #endregion

        #region Selected Item Changed
        public static readonly DependencyProperty SelectedItemChangedProperty = DependencyProperty.RegisterAttached
            (
                "SelectedItemChanged",
                typeof(ICommand),
                typeof(TreeViewHelper)
            );

        public static ICommand GetSelectedItemChanged(DependencyObject dependencyObject)
        {
            return (ICommand)dependencyObject.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItemChanged(DependencyObject dependencyObject, ICommand value)
        {
            dependencyObject.SetValue(SelectedItemProperty, value);
        }
        #endregion

        #region Event Args
        public class DependencyPropertyEventArgs : EventArgs
        {
            public DependencyPropertyChangedEventArgs DependencyPropertyChangedEventArgs { get; private set; }

            public DependencyPropertyEventArgs(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
            {
                this.DependencyPropertyChangedEventArgs = dependencyPropertyChangedEventArgs;
            }
        }
        #endregion
    }
}
