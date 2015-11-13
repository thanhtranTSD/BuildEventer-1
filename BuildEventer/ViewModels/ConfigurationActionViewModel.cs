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
using BuildEventer.UI.ConfirmActionName;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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

        private ICommand m_CreateActionCommand;

        public ICommand CreateActionCommand
        {
            get
            {
                return m_CreateActionCommand ?? (m_CreateActionCommand = new DelegateCommand(param => CreateCopyAction()));
            }
        }

        #endregion

        #region SaveChangedCommand

        private ICommand m_SaveChangedCommand;

        public ICommand SaveChangedCommand
        {
            get
            {
                return m_SaveChangedCommand ?? (m_SaveChangedCommand = new DelegateCommand(p => SaveChangedMethod(), CanExecuteSaveChangedCommand));
            }
        }

        private void SaveChangedMethod()
        {
            m_SelectedViewModel.Update();
        }

        private bool CanExecuteSaveChangedCommand()
        {
            return ((null == m_SelectedViewModel) ? false : m_SelectedViewModel.IsChanged);
        }

        #endregion

        #region DeleteSelectedActionCommand

        private ICommand m_DeleteSelectedActionCommand;

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
                m_SelectedViewModel = null;
                ViewModels.RemoveAt(SelectedIndex);

                if (0 == m_ViewModels.Count)
                {
                    m_ActionUI = null;
                    OnPropertyChanged("ActionUI");
                }
            }
        }

        private bool CanExecuteDeleteSelectedActionCommand()
        {
            return (0 != m_ViewModels.Count);
        }

        #endregion

        #endregion

        #region Properties

        public ObservableCollection<SettingsViewModelBase> ViewModels
        {
            get
            {
                if (null == m_ViewModels)
                {
                    m_ViewModels = new ObservableCollection<SettingsViewModelBase>();
                }

                return m_ViewModels;
            }
            set
            {
                m_ViewModels = value;
                OnPropertyChanged("ViewModels");
            }
        }

        public SettingsViewModelBase SelectedViewModel
        {
            get
            {
                return m_SelectedViewModel;
            }
            set
            {
                if ((m_SelectedViewModel != value) && (null != value))
                {
                    bool isSavedProcess = true;
                    if (null != m_SelectedViewModel)
                    {
                        if (true == m_SelectedViewModel.IsChanged)
                        {
                            MessageBoxResult result = MessageBox.Show(String.Format("Do you want to save changed in {0}", m_SelectedViewModel.Action.Name), "Confirm",
                                                                      MessageBoxButton.YesNoCancel,
                                                                      MessageBoxImage.Question,
                                                                      MessageBoxResult.Yes);
                            switch (result)
                            {
                                case MessageBoxResult.Yes:
                                    {
                                        m_SelectedViewModel.Update();
                                        isSavedProcess = true;
                                        break;
                                    }
                                case MessageBoxResult.No:
                                    {
                                        m_SelectedViewModel.IsChanged = false;
                                        isSavedProcess = true;
                                        m_SelectedViewModel.Restore();
                                        break;
                                    }
                                case MessageBoxResult.Cancel:
                                    {
                                        isSavedProcess = false;
                                        m_DispatcherTimer.Start();
                                        break;
                                    }
                            }
                        }
                        if (false == m_SelectedViewModel.CanExecuteAction)
                        {
                            MessageBoxResult result = MessageBox.Show(String.Format("Sources or destinations of {0} cannot empty.", m_SelectedViewModel.Action.Name), "Warning",
                                                                          MessageBoxButton.OK,
                                                                          MessageBoxImage.Warning,
                                                                          MessageBoxResult.OK);
                            isSavedProcess = false;
                            m_DispatcherTimer.Start();
                        }
                    }

                    if (true == isSavedProcess)
                    {
                        m_SelectedViewModel = value;
                        m_IsBackup = true;
                        SettingView();
                        OnPropertyChanged("ActionUI");
                        OnPropertyChanged("SelectedViewModel");
                        OnPropertyChanged("ViewModels");
                    }
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

        public UserControl ActionUI
        {
            get
            {
                return m_ActionUI;
            }
        }

        #endregion

        #region Public Functions

        public void GenerateXmlFile()
        {
            if (null == m_SelectedViewModel)
            {
                return;
            }

            foreach (SettingsViewModelBase vm in m_ViewModels)
            {
                if (false == vm.CanExecuteAction)
                {
                    return;
                }
            }

            string path = XMLManager.GetPathToSaveFile();
            if (null != path)
            {
                XMLManager.GenerateXmlFile(path, m_ViewModels);
            }
        }

        public void LoadXmlFile()
        {
            if (null != m_SelectedViewModel)
            {
                m_SelectedViewModel = null;
            }

            string path = XMLManager.GetPathToLoadFile();

            if (null != path)
            {
                ViewModels.Clear();
                ViewModels = XMLManager.LoadXmlFile(path);
                SelectedViewModelsIndex(m_ViewModels.Count - 1);
            }
        }

        #endregion

        #region Private Functions

        private void Initialize()
        {
            Factories.SettingsViewModelFactory.Instance().Initialize();

            m_ViewModels = new ObservableCollection<SettingsViewModelBase>();
            List<Type> m_ActionTypes = Utilities.ActionUtility.GetAllActionTypes();

            m_AllViewModels = Utilities.ActionUtility.GetAllViewModels();

            m_DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            m_DispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            m_DispatcherTimer.Interval = TimeSpan.FromMilliseconds(10);
        }

        private bool CheckCompletedActionViewModel(SettingsViewModelBase actionViewModel)
        {
            if (null != actionViewModel)
            {
                if ((true == actionViewModel.IsChanged) || (false == actionViewModel.CanExecuteAction))
                {
                    MessageBoxResult result = MessageBox.Show(String.Format("Cannot create new action now. The action {0} must be completed first.", actionViewModel.Action.Name), "Information",
                                                                          MessageBoxButton.OK,
                                                                          MessageBoxImage.Information,
                                                                          MessageBoxResult.OK);
                    return false;
                }
            }
            return true;
        }

        private void CreateCopyAction()
        {
            bool isComplete = CheckCompletedActionViewModel(m_SelectedViewModel);
            if (true == isComplete)
            {
                ConfirmActionNameViewModel confirmActionNameVM = new ConfirmActionNameViewModel(m_ViewModels);
                ConfirmActionNameWindow confirmActionNameWindow = new ConfirmActionNameWindow();
                confirmActionNameWindow.DataContext = confirmActionNameVM;

                if (true == confirmActionNameWindow.ShowDialog())
                {
                    string defaultActionName = confirmActionNameVM.ActionName;

                    CopyActionModel action = new CopyActionModel();
                    action.Name = defaultActionName;
                    action.Sources = new ObservableCollection<DragDropData>();
                    action.Destinations = new ObservableCollection<DragDropData>();

                    CopyActionViewModel actionVM = new CopyActionViewModel(action);
                    ViewModels.Add(actionVM);

                    SelectedViewModelsIndex(m_ViewModels.Count - 1);
                }
            }
        }

        private void SelectedViewModelsIndex(int index)
        {
            SelectedIndex = index;
        }

        private void SettingView()
        {
            if (null == m_SelectedViewModel)
            {
                m_ActionUI = null;
            }
            else
            {
                if (true == m_IsBackup)
                {
                    m_IsBackup = false;
                    m_SelectedViewModel.Backup();
                }
                m_ActionUI = m_AllViewModels[m_SelectedViewModel.GetType()];

                m_ActionUI.DataContext = m_SelectedViewModel;
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            SelectedIndex = m_ViewModels.IndexOf(m_SelectedViewModel);
            m_DispatcherTimer.Stop();
        }

        #endregion

        #region Members

        private bool m_IsBackup;
        private int m_SelectedIndex;

        private UserControl m_ActionUI;

        private ObservableCollection<SettingsViewModelBase> m_ViewModels;
        private SettingsViewModelBase m_SelectedViewModel;
        private Dictionary<Type, UserControl> m_AllViewModels = new Dictionary<Type, UserControl>();

        private DispatcherTimer m_DispatcherTimer;

        #endregion
    }
}
