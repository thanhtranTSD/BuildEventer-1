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

using BuildEventer.ViewModels;
using System.Windows;
using System.Windows.Interactivity;

namespace BuildEventer.Behaviors
{
    public class FrameworkElementDropBehavior : Behavior<FrameworkElement>
    {
        #region Override functions
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.DragEnter += new DragEventHandler(AssociatedObject_DragEnter);
            this.AssociatedObject.DragOver += new DragEventHandler(AssociatedObject_DragOver);
            this.AssociatedObject.DragLeave += new DragEventHandler(AssociatedObject_DragLeave);
            this.AssociatedObject.Drop += new DragEventHandler(AssociatedObject_Drop);
        }
        #endregion

        #region Properties
        private DragDropViewModel DataType
        {
            get;
            set;
        }
        #endregion

        #region Private functions
        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (null != DataType)
            {
                // Drop the data
                IDropable target = this.AssociatedObject.DataContext as IDropable;
                target.Drop(DataType, sender);
            }
            DataType = null;
            e.Handled = true;
        }

        private void AssociatedObject_DragLeave(object sender, DragEventArgs e)
        {
            DataType = null;
            e.Handled = true;
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            if (null != DataType)
            {
                e.Effects = DragDropEffects.None;
                if (true == ((IDropable)this.AssociatedObject.DataContext).IsValidDragData(DataType, sender))
                {
                    e.Effects = DragDropEffects.Copy;
                }
            }
            e.Handled = true;
        }

        private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {
            DragDropViewModel drapDataType = (DragDropViewModel)e.Data.GetData(typeof(DragDropViewModel));
            if (null == DataType)
            {
                DataType = drapDataType;
            }
            e.Handled = true;
        }
        #endregion
    }
}
