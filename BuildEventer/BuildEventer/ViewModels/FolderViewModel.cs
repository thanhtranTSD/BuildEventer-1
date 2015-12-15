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

using BuildEventer.Behaviors;
using System;
using System.Collections.ObjectModel;

namespace BuildEventer.ViewModels
{
    public class FolderViewModel : DragDropViewModel, IDragable
    {
        public delegate void ExpandedEventHandler(object sender);
        public event ExpandedEventHandler Expanded;

        #region Constructor
        public FolderViewModel(string path, string fullPath, string relativePath)
            : base(path, fullPath, relativePath)
        {
            Items = new ObservableCollection<DragDropViewModel>();
        }

        public FolderViewModel(string path, string fullPath, string relativePath, FolderViewModel parent = null)
            : this(path, fullPath, relativePath)
        {
            Parent = parent;
        }
        #endregion

        #region Properties
        public bool IsExpanded
        {
            get { return m_IsExpanded; }
            set
            {
                if (m_IsExpanded != value)
                {
                    m_IsExpanded = value;
                    if (null != Expanded)
                    {
                        Expanded(this);
                    }
                }
            }
        }

        public FolderViewModel Parent { get; private set; }

        public ObservableCollection<DragDropViewModel> Items { get; private set; }
        #endregion

        #region Public Functions
        public void Add(DragDropViewModel item)
        {
            Items.Add(item);
        }

        public void Remove(DragDropViewModel item)
        {
            Items.Remove(item);
        }
        #endregion

        #region IDragable Members
        Type IDragable.GetType
        {
            get
            {
                // Only allow drag if not root
                if (null != Parent)
                {
                    return typeof(DragDropViewModel);
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region Member
        private bool m_IsExpanded;
        #endregion
    }
}
