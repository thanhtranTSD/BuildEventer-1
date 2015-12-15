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
using BuildEventer.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace BuildEventer.UI
{
    public class StartUpDialogueViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Constructor
        public StartUpDialogueViewModel()
        {
            WorkingDirectoryPath = Directory.GetCurrentDirectory();
        }
        #endregion

        #region Properties
        public string WorkingDirectoryPath
        {
            get { return m_WorkingDirectoryPath; }
            private set
            {
                m_WorkingDirectoryPath = value;
                OnPropertyChanged("WorkingDirectoryPath");
            }
        }

        public bool IsPathValid
        {
            get { return m_IsPathValid; }
            private set
            {
                m_IsPathValid = value;
                OnPropertyChanged("IsPathValid");
            }
        }
        #endregion

        #region Commands

        #region BrowserCommand
        public ICommand BrowserCommand
        {
            get
            {
                return m_BrowserCommand ?? (m_BrowserCommand = new DelegateCommand(p => OnBrowserCommand((RoutedEventArgs)p)));
            }
        }

        private void OnBrowserCommand(RoutedEventArgs e)
        {
            ExplorerWindow explorerWindow = new ExplorerWindow();
            if (true == explorerWindow.ShowDialog())
            {
                WorkingDirectoryPath = explorerWindow.SelectedPath.Text;
            }
        }
        #endregion

        #endregion

        #region IDataErrorInfo
        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string result = string.Empty;
                if (WORKING_DIRECTORY_PATH == propertyName)
                {
                    if (false == Directory.Exists(WorkingDirectoryPath))
                    {
                        IsPathValid = false;
                        return string.Format("Path does not exist. Please input a valid path.");
                    }
                    IsPathValid = true;
                }
                return result;
            }
        }
        #endregion

        #region Members
        private bool m_IsPathValid;
        private string m_WorkingDirectoryPath;

        private ICommand m_BrowserCommand;
        #endregion

        #region Constants
        private const string WORKING_DIRECTORY_PATH = "WorkingDirectoryPath";
        #endregion
    }
}
