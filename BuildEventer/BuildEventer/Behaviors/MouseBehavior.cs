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
    public static class MouseBehavior
    {
        #region PreviewDropToSourceCommand
        public static readonly DependencyProperty PreviewDropToSourceCommandProperty = DependencyProperty.RegisterAttached
            (
                "PreviewDropToSourceCommand",
                typeof(ICommand),
                typeof(MouseBehavior),
                new PropertyMetadata(PreviewDropToSourceCommandPropertyChangedCallBack)
            );

        public static void SetPreviewDropToSourceCommand(this UIElement element, ICommand value)
        {
            element.SetValue(PreviewDropToSourceCommandProperty, value);
        }

        public static ICommand GetPreviewDropToSourceCommand(UIElement element)
        {
            return (ICommand)element.GetValue(PreviewDropToSourceCommandProperty);
        }

        private static void PreviewDropToSourceCommandPropertyChangedCallBack(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (null == uiElement)
            {
                return;
            }
            uiElement.Drop += (sender, args) =>
            {
                GetPreviewDropToSourceCommand(uiElement).Execute(args.Data);
                args.Handled = true;
            };

            uiElement.PreviewDragOver += (sender, args) =>
            {
                args.Handled = true;
            };

            uiElement.PreviewDragEnter += (sender, args) =>
            {
                DataObject dataObject = args.Data as DataObject;

                // Check for file list
                if (true == dataObject.ContainsFileDropList())
                {
                    args.Effects = DragDropEffects.Copy;
                }
                else
                {
                    args.Effects = DragDropEffects.None;
                }
                args.Handled = true;
            };
        }
        #endregion

        #region PreviewDropToDestinationCommand
        public static readonly DependencyProperty PreviewDropToDestinationCommandProperty = DependencyProperty.RegisterAttached
            (
                "PreviewDropToDestinationCommand",
                typeof(ICommand),
                typeof(MouseBehavior),
                new PropertyMetadata(PreviewDropToDestinationCommandPropertyChangedCallBack)
            );

        public static void SetPreviewDropToDestinationCommand(this UIElement element, ICommand value)
        {
            element.SetValue(PreviewDropToDestinationCommandProperty, value);
        }

        public static ICommand GetPreviewDropToDestinationCommand(UIElement element)
        {
            return (ICommand)element.GetValue(PreviewDropToDestinationCommandProperty);
        }

        private static void PreviewDropToDestinationCommandPropertyChangedCallBack(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (null == uiElement)
            {
                return;
            }

            uiElement.Drop += (sender, args) =>
            {
                GetPreviewDropToDestinationCommand(uiElement).Execute(args.Data);
                args.Handled = true;
            };

            uiElement.PreviewDragOver += (sender, args) =>
            {
                args.Handled = true;
            };

            uiElement.PreviewDragEnter += (sender, args) =>
            {
                DataObject dataObject = args.Data as DataObject;

                // Check for file list
                if (true == dataObject.ContainsFileDropList())
                {
                    args.Effects = DragDropEffects.Copy;
                }
                else
                {
                    args.Effects = DragDropEffects.None;
                }
                args.Handled = true;
            };
        }
        #endregion

        #region MouseMove
        public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.RegisterAttached
            (
                "MouseMoveCommand",
                typeof(ICommand),
                typeof(MouseBehavior),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseMoveCommandChanged))
            );

        public static void SetMouseMoveCommand(UIElement element, ICommand value)
        {
            element.SetValue(MouseMoveCommandProperty, value);
        }

        public static ICommand GetMouseMoveCommand(UIElement element)
        {
            return (ICommand)element.GetValue(MouseMoveCommandProperty);
        }

        private static void MouseMoveCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            element.MouseMove += new MouseEventHandler(element_MouseMove);
        }

        private static void element_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;

            ICommand command = GetMouseMoveCommand(element);

            command.Execute(e);
        }
        #endregion
    }
}
