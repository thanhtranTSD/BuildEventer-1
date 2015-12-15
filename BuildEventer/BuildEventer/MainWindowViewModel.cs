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
using BuildEventer.UI;
using BuildEventer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace BuildEventer
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructor
        public MainWindowViewModel()
        {
            string workingDirectoryPath = GetWorkingingDirectoryPath();
            WorkingDirectoryViewDataContext = new WorkingDirectoryViewModel(workingDirectoryPath);
            m_ConfigurationActionViewDataContext = new ConfigurationActionViewModel();
        }
        #endregion

        #region Properties
        public WorkingDirectoryViewModel WorkingDirectoryViewDataContext { get; private set; }

        public ConfigurationActionViewModel ConfigurationActionViewDataContext
        {
            get { return m_ConfigurationActionViewDataContext; }
            private set
            {
                if (m_ConfigurationActionViewDataContext != value)
                {
                    m_ConfigurationActionViewDataContext = value;
                    OnPropertyChanged("ConfigurationActionViewDataContext");
                }
            }
        }
        #endregion

        #region Commands

        #region GenerateXML Command

        public ICommand GenerateXMLCommand
        {
            get
            {
                return m_GenerateXMLCommand ?? (m_GenerateXMLCommand = new DelegateCommand(p => GenerateXMLMethod(), CanExecuteGenerateXMLCommand));
            }
        }

        private void GenerateXMLMethod()
        {
            m_ConfigurationActionViewDataContext.GenerateXmlFile();
        }

        /// <summary>
        /// To ensure there is the action has source and path.
        /// </summary>
        private bool CanExecuteGenerateXMLCommand()
        {
            bool isActionCanExecute = (null == m_ConfigurationActionViewDataContext.SelectedActionViewModel) ? false : m_ConfigurationActionViewDataContext.SelectedActionViewModel.CanExecuteAction;
            return ((0 != m_ConfigurationActionViewDataContext.ActionViewModels.Count) && (true == isActionCanExecute));
        }

        #endregion

        #region LoadXML Command

        public ICommand LoadXMLCommand
        {
            get
            {
                return m_LoadXMLCommand ?? (m_LoadXMLCommand = new DelegateCommand(p => LoadXMLMethod()));
            }
        }

        private void LoadXMLMethod()
        {
            m_ConfigurationActionViewDataContext.LoadXmlFile();
        }
        #endregion

        #region Exit Command

        public ICommand ExitCommand
        {
            get
            {
                return m_ExitCommand ?? (m_ExitCommand = new DelegateCommand(p => ExitMethod()));
            }
        }

        private void ExitMethod()
        {
            Application.Current.Shutdown();
        }

        #endregion

        #endregion Commands

        #region Private Members
        private string GetWorkingingDirectoryPath()
        {
            string returnPath = string.Empty;

            StartUpDialogueViewModel startUpDialogueViewModel = new StartUpDialogueViewModel();
            StartUpWindow startUpWindow = new StartUpWindow();
            startUpWindow.DataContext = startUpDialogueViewModel;
            if (true == startUpWindow.ShowDialog())
            {
                returnPath = startUpDialogueViewModel.WorkingDirectoryPath;
            }

            return returnPath;
        }
        #endregion

        #region Members
        private ConfigurationActionViewModel m_ConfigurationActionViewDataContext;

        private ICommand m_GenerateXMLCommand;
        private ICommand m_LoadXMLCommand;
        private ICommand m_ExitCommand;
        #endregion
    }
}
