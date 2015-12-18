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
using BuildEventer.Dialog;
using BuildEventer.Models;
using BuildEventer.UI.ConfirmActionName;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BuildEventer.ViewModels
{
    public class ConfigurationActionViewModel : ViewModelBase
    {
        #region Constructor

        public ConfigurationActionViewModel()
        {
            Initialize();
        }

        #endregion

        #region Commands

        #region CreateActionCommand
        public ICommand CreateActionCommand
        {
            get
            {
                return m_CreateActionCommand ?? (m_CreateActionCommand = new DelegateCommand(p => CreateCopyAction()));
            }
        }

        #endregion

        #region SaveChangedCommand
        public ICommand SaveChangedCommand
        {
            get
            {
                return m_SaveChangedCommand ?? (m_SaveChangedCommand = new DelegateCommand(p => SaveChangedMethod(), CanExecuteSaveChangedCommand));
            }
        }

        private void SaveChangedMethod()
        {
            m_SelectedActionViewModel.Update();
        }

        private bool CanExecuteSaveChangedCommand()
        {
            return ((null == m_SelectedActionViewModel) ? false : m_SelectedActionViewModel.IsChanged);
        }

        #endregion

        #region DeleteSelectedActionCommand
        public ICommand DeleteSelectedActionCommand
        {
            get
            {
                return m_DeleteSelectedActionCommand ?? (m_DeleteSelectedActionCommand = new DelegateCommand(param => DeleteActionViewModel((Object)param), CanExecuteDeleteSelectedActionCommand));
            }
        }

        private void DeleteActionViewModel(Object obj)
        {
            if (-1 != SelectedIndex)
            {
                m_SelectedActionViewModel = null;
                ActionViewModels.RemoveAt(SelectedIndex);
            }
        }

        private bool CanExecuteDeleteSelectedActionCommand()
        {
            return (0 != m_ActionViewModels.Count);
        }

        #endregion

        #endregion

        #region Properties

        public ObservableCollection<SettingsViewModelBase> ActionViewModels
        {
            get
            {
                if (null == m_ActionViewModels)
                {
                    m_ActionViewModels = new ObservableCollection<SettingsViewModelBase>();
                }

                return m_ActionViewModels;
            }
            private set
            {
                m_ActionViewModels = value;
                OnPropertyChanged("ActionViewModels");
            }
        }

        public SettingsViewModelBase SelectedActionViewModel
        {
            get
            {
                return m_SelectedActionViewModel;
            }
            set
            {
                if ((m_SelectedActionViewModel != value) || (null == m_SelectedActionViewModel))
                {
                    if (null != m_SelectedActionViewModel)
                    {
                        m_SelectedViewModelValue = value;
                        if (true == m_SelectedActionViewModel.IsChanged)
                        {
                            MessageBoxDialog messageDialog = new MessageBoxDialog();
                            messageDialog.Show(SaveChangedCallback, "Confirm", String.Format("Do you want to save changed in {0}?", m_SelectedActionViewModel.Action.Name),
                                               MessageBoxButton.YesNoCancel);
                            return;
                        }
                        if (false == m_SelectedActionViewModel.CanExecuteAction)
                        {
                            MessageBoxDialog messageDialog = new MessageBoxDialog();
                            messageDialog.Show(CanExecuteActionCallback, "Warning", String.Format("Sources or destinations of {0} cannot empty.", m_SelectedActionViewModel.Action.Name),
                                               MessageBoxButton.OK);
                            return;
                        }
                    }

                    m_SelectedActionViewModel = value;
                    m_IsBackup = false;
                    BackupSelectedViewModel();
                    OnPropertyChanged("SelectedActionViewModel");
                    OnPropertyChanged("ActionViewModels");
                }
            }
        }

        public int SelectedIndex
        {
            get
            {
                return m_SelectedIndex;
            }
            set
            {
                m_SelectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        #endregion

        #region Public Functions

        public void GenerateXmlFile()
        {
            if (null == m_SelectedActionViewModel)
            {
                return;
            }

            foreach (SettingsViewModelBase vm in m_ActionViewModels)
            {
                if (false == vm.CanExecuteAction)
                {
                    return;
                }
            }

            string path = XMLManager.GetPathToSaveFile();
            if (null != path)
            {
                XMLManager.GenerateConfigurationFile(path, m_ActionViewModels);
            }
        }

        public void LoadXmlFile()
        {
            if (null != m_SelectedActionViewModel)
            {
                m_SelectedActionViewModel = null;
            }

            string path = XMLManager.GetPathToLoadFile();

            if (null != path)
            {
                ActionViewModels.Clear();
                ActionViewModels = XMLManager.LoadConfigurationFile(path);
                SelectedViewModelsIndex(m_ActionViewModels.Count - 1);
            }
        }

        #endregion

        #region Private Functions

        #region Callback functions
        private void SaveChangedCallback(MessageBoxResultButton result)
        {
            bool isSavedProcess = true;
            switch (result)
            {
                // Yes
                case MessageBoxResultButton.Yes:
                    {
                        m_SelectedActionViewModel.Update();
                        isSavedProcess = true;
                        break;
                    }
                // No
                case MessageBoxResultButton.No:
                    {
                        isSavedProcess = true;
                        m_SelectedActionViewModel.Restore();
                        break;
                    }
                // Cancel
                case MessageBoxResultButton.Cancel:
                    {
                        isSavedProcess = false;
                        Task task = new Task(() =>
                        {
                            SelectedViewModelsIndex(m_ActionViewModels.IndexOf(m_SelectedActionViewModel));
                        });
                        task.Wait(50);
                        task.Start();
                        break;
                    }
            }
            if (true == isSavedProcess)
            {
                m_SelectedActionViewModel = m_SelectedViewModelValue;
                m_IsBackup = false;
                BackupSelectedViewModel();
                OnPropertyChanged("SelectedViewModel");
                OnPropertyChanged("ActionViewModels");
            }
        }

        private void CanExecuteActionCallback(MessageBoxResultButton result)
        {
            Task task = new Task(() =>
            {
                SelectedViewModelsIndex(m_ActionViewModels.IndexOf(m_SelectedActionViewModel));
            });
            task.Wait(50);
            task.Start();
        }
        #endregion

        private void Initialize()
        {
            Factories.SettingsViewModelFactory.Instance().Initialize();

            m_ActionViewModels = new ObservableCollection<SettingsViewModelBase>();
        }

        private bool CheckCompletedActionViewModel(SettingsViewModelBase actionViewModel)
        {
            if (null != actionViewModel)
            {
                if ((true == actionViewModel.IsChanged) || (false == actionViewModel.CanExecuteAction))
                {
                    return false;
                }
            }
            return true;
        }

        private void CreateCopyAction()
        {
            bool isComplete = CheckCompletedActionViewModel(m_SelectedActionViewModel);
            if (true == isComplete)
            {
                ConfirmActionNameViewModel confirmActionNameVM = new ConfirmActionNameViewModel(m_ActionViewModels);
                ConfirmActionNameWindow confirmActionNameWindow = new ConfirmActionNameWindow();
                confirmActionNameWindow.DataContext = confirmActionNameVM;

                if (true == confirmActionNameWindow.ShowDialog())
                {
                    string defaultActionName = confirmActionNameVM.ActionName;

                    CopyActionModel action = new CopyActionModel();
                    action.Name = defaultActionName;
                    action.Sources = new List<DragDropData>();
                    action.Destinations = new List<DragDropData>();

                    CopyActionViewModel actionVM = new CopyActionViewModel(action);
                    ActionViewModels.Add(actionVM);

                    SelectedViewModelsIndex(m_ActionViewModels.Count - 1);
                }
            }
            else
            {
                MessageBoxDialog messageDialog = new MessageBoxDialog();
                messageDialog.Show(CanExecuteActionCallback, "Information", String.Format("Cannot create new action now. The action {0} must be completed or saved first.", m_SelectedActionViewModel.Action.Name),
                                   MessageBoxButton.OK);
            }
        }

        private void SelectedViewModelsIndex(int index)
        {
            SelectedIndex = index;
        }

        private void BackupSelectedViewModel()
        {
            if (null != m_SelectedActionViewModel)
            {
                if (false == m_IsBackup)
                {
                    m_IsBackup = true;
                    m_SelectedActionViewModel.Backup();
                }
            }
        }

        #endregion

        #region Members
        private bool m_IsBackup;
        private int m_SelectedIndex;

        private ObservableCollection<SettingsViewModelBase> m_ActionViewModels;
        private SettingsViewModelBase m_SelectedActionViewModel;
        private SettingsViewModelBase m_SelectedViewModelValue;

        private ICommand m_CreateActionCommand;
        private ICommand m_SaveChangedCommand;
        private ICommand m_DeleteSelectedActionCommand;
        #endregion
    }
}
