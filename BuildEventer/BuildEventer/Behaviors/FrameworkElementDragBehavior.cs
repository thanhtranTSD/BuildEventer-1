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
using System.Windows.Interactivity;

namespace BuildEventer.Behaviors
{
    public class FrameworkElementDragBehavior : Behavior<FrameworkElement>
    {
        #region Override functions
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
            this.AssociatedObject.MouseLeftButtonUp += new MouseButtonEventHandler(AssociatedOBject_MouseLeftButtonUp);
            this.AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
        }
        #endregion

        #region Private functions
        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_IsMouseClicked = true;
        }

        private void AssociatedOBject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_IsMouseClicked = false;
        }

        void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (true == m_IsMouseClicked)
            {
                // Set the item's DataContext as the data to be transferred
                IDragable dragObject = this.AssociatedObject.DataContext as IDragable;
                if (null != dragObject)
                {
                    if (null != dragObject.GetType)
                    {
                        DataObject data = new DataObject();
                        data.SetData(dragObject.GetType, this.AssociatedObject.DataContext);
                        DragDrop.DoDragDrop(this.AssociatedObject, data, DragDropEffects.Copy);
                    }
                }
            }
            m_IsMouseClicked = false;
        }
        #endregion

        #region Members
        private bool m_IsMouseClicked;
        #endregion
    }
}
