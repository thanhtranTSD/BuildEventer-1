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

using BuildEventer.Command;
using BuildEventer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace BuildEventer.ViewModels
{
    public class CopyActionViewModel : SettingsViewModelBase
    {
        #region Constructors
        public CopyActionViewModel()
        {
        }

        public CopyActionViewModel(CopyActionModel copyAction)
        {
            Action = copyAction;
            this.CopyAction = copyAction;
        }
        #endregion

        #region Properties
        public CopyActionModel CopyAction { get; set; }

        public int SelectedIndexSources { get; set; }

        public int SelectedIndexDestinations { get; set; }

        public DragDropData SelectedItemSources { get; set; }

        public DragDropData SelectedItemDestinations { get; set; }

        public override IActionModel Action { get; set; }

        public override bool CanExecuteAction
        {
            get
            {
                return ((false == IsSourcesEmpty) && (false == IsDestonationsEmpty));
            }
        }

        public override bool IsChanged { get; set; }

        public bool IsSourcesEmpty
        {
            get
            {
                return (0 == this.CopyAction.Sources.Count);
            }
        }

        public bool IsDestonationsEmpty
        {
            get
            {
                return (0 == this.CopyAction.Destinations.Count);
            }
        }

        public bool IsFocused { get; private set; }

        public bool HasBackup
        {
            get { return m_HasBackup; }
            private set
            {
                m_HasBackup = value;
                OnPropertyChanged("HasBackup");
            }
        }
        #endregion

        #region Commands

        #region DeleteSelectedItem
        private ICommand m_DeleteSelectedItemSourcesCommand;

        public ICommand DeleteSelectedItemSourcesCommand
        {
            get
            {
                return m_DeleteSelectedItemSourcesCommand ?? (m_DeleteSelectedItemSourcesCommand = new DelegateCommand(p => DeleteSelectedItem((ListView)p), CanExecuteDeleteSelectedItemSourcesCommand));
            }
        }

        private ICommand m_DeleteSelectedItemDestinationsCommand;

        public ICommand DeleteSelectedItemDestinationsCommand
        {
            get
            {
                return m_DeleteSelectedItemDestinationsCommand ?? (m_DeleteSelectedItemDestinationsCommand = new DelegateCommand(p => DeleteSelectedItem((ListView)p), CanExecuteDeleteSelectedItemDestinationsCommand));
            }
        }

        private void DeleteSelectedItem(ListView listView)
        {
            if (null != listView)
            {
                if (null != listView.SelectedItem)
                {
                    Backup();
                    if (LISTVIEW_SOURCES_NAME == listView.Name)
                    {
                        this.CopyAction.Sources.Remove((DragDropData)listView.SelectedItem);
                    }
                    else if (LISTVIEW_DESTINATIONS_NAME == listView.Name)
                    {
                        this.CopyAction.Destinations.Remove((DragDropData)listView.SelectedItem);
                    }
                    IsChanged = true;
                }
            }
        }

        private bool CanExecuteDeleteSelectedItemSourcesCommand()
        {
            return ((0 != this.CopyAction.Sources.Count) && (null != SelectedItemSources));
        }

        private bool CanExecuteDeleteSelectedItemDestinationsCommand()
        {
            return ((0 != this.CopyAction.Destinations.Count) && (null != SelectedItemDestinations));
        }
        #endregion m_DeleteSelectedItemDestinationsCommand

        #region PreviewDropToSourceCommand
        private ICommand m_PreviewDropToSourceCommand;

        public ICommand PreviewDropToSourceCommand
        {
            get
            {
                return m_PreviewDropToSourceCommand ?? (m_PreviewDropToSourceCommand = new DelegateCommand(p => HandlePreviewDropToSource(p)));
            }
        }
        #endregion

        #region PreviewDropToDestinationCommand

        private ICommand m_PreviewDropToDestinationCommand;

        public ICommand PreviewDropToDestinationCommand
        {
            get
            {
                return m_PreviewDropToDestinationCommand ?? (m_PreviewDropToDestinationCommand = new DelegateCommand(p => HandlePreviewDropToDestination(p)));
            }
        }
        #endregion

        #endregion

        #region Public Functions
        public override void Backup()
        {
            if (false == HasBackup)
            {
                m_SourcesBackup = new ObservableCollection<DragDropData>(this.CopyAction.Sources);
                m_DestinationsBackup = new ObservableCollection<DragDropData>(this.CopyAction.Destinations);
                HasBackup = true;
            }
        }

        public override void Restore()
        {
            if (true == HasBackup)
            {
                this.CopyAction.Sources = new ObservableCollection<DragDropData>(m_SourcesBackup);
                this.CopyAction.Destinations = new ObservableCollection<DragDropData>(m_DestinationsBackup);
                ClearBackup();
            }
        }

        public override void Update()
        {
            ClearBackup();
            IsChanged = false;
        }
        #endregion

        #region Private Functions
        private void ClearBackup()
        {
            m_SourcesBackup.Clear();
            m_DestinationsBackup.Clear();
            HasBackup = false;
        }

        /// <summary>
        /// Receive drop data into sources region and add to Sources
        /// </summary>
        private void HandlePreviewDropToSource(Object obj)
        {
            IDataObject ido = obj as IDataObject;

            if (null == ido)
            {
                return;
            }

            DragDropData dropData = (DragDropData)ido.GetData(typeof(DragDropData));

            foreach (DragDropData source in this.CopyAction.Sources)
            {
                if (source.Path == dropData.Path)
                {
                    MessageBox.Show(dropData.Path.ToString() + " has already in Sources of this action.");
                    return;
                }
            }
            // Backup the Sources before receive new data
            Backup();
            UpdateSource(dropData);
        }

        /// <summary>
        /// Receive drop data into destination region and add to Destinations
        /// </summary>
        private void HandlePreviewDropToDestination(Object obj)
        {
            IDataObject ido = obj as IDataObject;

            if (null == ido)
            {
                return;
            }

            DragDropData dropData = (DragDropData)ido.GetData(typeof(DragDropData));

            if (false == dropData.IsFolder)
            {
                MessageBox.Show("Path must be a folder.");
                return;
            }

            foreach (DragDropData dest in this.CopyAction.Destinations)
            {
                if (dest.Path == dropData.Path)
                {
                    MessageBox.Show(dropData.Path.ToString() + " has already in Destinations of this action.");
                    return;
                }
            }
            // Backup the Destinations before receive new data
            Backup();
            UpdateDestination(dropData);
        }

        private void OnFocused()
        {
            IsFocused = true;
            OnPropertyChanged("IsFocused");
        }

        private void UpdateSource(DragDropData newData)
        {
            this.CopyAction.Sources.Add(newData);
            IsChanged = true;
            OnFocused();
        }

        private void UpdateDestination(DragDropData newData)
        {
            this.CopyAction.Destinations.Add(newData);
            IsChanged = true;
            OnFocused();
        }
        #endregion

        #region Members
        private bool m_HasBackup;

        private ObservableCollection<DragDropData> m_SourcesBackup;
        private ObservableCollection<DragDropData> m_DestinationsBackup;
        #endregion

        #region Constants
        private const string LISTVIEW_SOURCES_NAME = "lvSources";
        private const string LISTVIEW_DESTINATIONS_NAME = "lvDestinations";
        #endregion
    }
}
