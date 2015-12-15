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
using BuildEventer.Command;
using BuildEventer.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace BuildEventer.ViewModels
{
    public class ActionSourcesViewModel : ViewModelBase, IDropable
    {
        #region Constructor
        public ActionSourcesViewModel(List<DragDropData> sources)
        {
            this.Sources = new BindingList<DragDropData>(sources);
        }
        #endregion

        #region Properties
        public BindingList<DragDropData> Sources { get; private set; }

        public int SelectedIndex
        {
            get { return m_SelectedIndex; }
            set
            {
                m_SelectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public DragDropData SelectedItem { get; set; }

        public bool IsFocused
        {
            get { return m_IsFocused; }
            private set
            {
                m_IsFocused = value;
                OnPropertyChanged("IsFocused");
            }
        }

        public bool IsChanged { get; private set; }

        public bool IsSourcesEmpty
        {
            get
            {
                return (0 == this.Sources.Count);
            }
        }

        public bool HasBackup { get; private set; }
        #endregion

        #region Commands

        #region DeleteSelectedItem
        public ICommand DeleteSelectedItemCommand
        {
            get
            {
                return m_DeleteSelectedItemCommand ?? (m_DeleteSelectedItemCommand = new DelegateCommand(p => DeleteSelectedItem(), CanExecuteDeleteSelectedItemCommand));
            }
        }

        private void DeleteSelectedItem()
        {
            Backup();
            this.Sources.Remove(SelectedItem);
            IsChanged = true;
        }

        private bool CanExecuteDeleteSelectedItemCommand()
        {
            return ((0 != this.Sources.Count) && (null != SelectedItem));
        }
        #endregion

        #endregion

        #region Public Functions
        public void Backup()
        {
            if (false == HasBackup)
            {
                m_SourcesBackup = new ObservableCollection<DragDropData>(this.Sources);
                HasBackup = true;
            }
        }

        public void Restore()
        {
            if (true == HasBackup)
            {
                this.Sources = new BindingList<DragDropData>(m_SourcesBackup);
                ClearBackup();
            }
        }

        public void ClearBackup()
        {
            IsChanged = false;
            HasBackup = false;
        }
        #endregion

        #region Override IDropable
        void IDropable.Drop(object data, object sender)
        {
            DropToSources((DragDropViewModel)data);
        }

        bool IDropable.IsValidDragData(object data, object sender)
        {
            DragDropViewModel dropData = data as DragDropViewModel;
            if (null != dropData)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Private Function
        private DragDropData CheckFolderOrFile(DragDropViewModel viewModel)
        {
            DragDropData returnData = null;
            if (true == (viewModel is FileViewModel))
            {
                returnData = new DragDropData(viewModel.Data.RelativePath, false);
            }
            else if (true == (viewModel is FolderViewModel))
            {
                returnData = new DragDropData(viewModel.Data.RelativePath, true);
            }

            return returnData;
        }

        private void DropToSources(DragDropViewModel data)
        {
            DragDropData dragDropData = CheckFolderOrFile(data);
            if (null == dragDropData)
            {
                return;
            }

            foreach (DragDropData source in this.Sources)
            {
                if (source.Path == dragDropData.Path)
                {
                    // Path is already in Sources of this action
                    // Highlight the duplicated path
                    SelectedIndex = this.Sources.IndexOf(source);
                    return;
                }
            }
            // Backup the Sources before receive new data
            Backup();
            Update(dragDropData);
        }

        private void Update(DragDropData newData)
        {
            this.Sources.Add(newData);
            IsChanged = true;
            OnFocused();
        }

        private void OnFocused()
        {
            IsFocused = true;
        }
        #endregion

        #region Members
        private int m_SelectedIndex;
        private bool m_IsFocused;
        private ObservableCollection<DragDropData> m_SourcesBackup;

        private ICommand m_DeleteSelectedItemCommand;
        #endregion

    }
}
