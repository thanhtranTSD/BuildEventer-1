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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace BuildEventer.UI.ConfirmActionName
{
    public class ConfirmActionNameViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Constructors

        public ConfirmActionNameViewModel(ICollection<SettingsViewModelBase> listViewModels)
        {
            m_DefaultName = GetDefaultActionName(listViewModels);
            m_ListViewModels = new List<SettingsViewModelBase>(listViewModels);
        }

        #endregion

        #region Properties

        public bool? DialogResult
        {
            get
            {
                return m_DialogResult;
            }
            private set
            {
                m_DialogResult = value;
                OnPropertyChanged("DialogResult");
            }
        }

        public string NewName
        {
            get
            {
                return m_NewName;
            }
            private set
            {
                m_NewName = value;
                m_NewName = Regex.Replace(m_NewName, @"\s+", "");
                OnPropertyChanged("NewName");
            }
        }

        public string DefaultName
        {
            get
            {
                return m_DefaultName;
            }
        }

        public string ActionName { get; private set; }

        #endregion

        #region Commands

        #region NewNameCommand

        public ICommand NewNameCommand
        {
            get
            {
                return m_NewNameCommand ?? (m_NewNameCommand = new DelegateCommand(p => NewNameMethod(), CanExecuteNewNameCommand));
            }
        }

        private bool CanExecuteNewNameCommand()
        {
            bool result = true;
            if ((true == string.IsNullOrWhiteSpace(NewName)) || (false == m_IsValidNewName))
            {
                result = false;
            }
            return result;
        }

        private void NewNameMethod()
        {
            ActionName = m_NewName;
            DialogResult = true;
        }
        #endregion

        #region DefaultCommand

        public ICommand DefaultCommand
        {
            get
            {
                return m_DefaultCommand ?? (m_DefaultCommand = new DelegateCommand(p => DefaultMethod()));
            }
        }

        private void DefaultMethod()
        {
            ActionName = m_DefaultName;
            DialogResult = true;
        }
        #endregion

        #endregion

        #region Private Functions
        private string GetDefaultActionName(ICollection<SettingsViewModelBase> listViewModels)
        {
            bool isRepeated = false;
            int orderAction = listViewModels.Count + 1;

            do
            {
                isRepeated = false;
                foreach (SettingsViewModelBase vm in listViewModels)
                {
                    if (vm.Action.Name == (DEFAULT_ACTION_NAME_PREFIX + orderAction.ToString()))
                    {
                        isRepeated = true;
                        ++orderAction;
                        break;
                    }
                }
            } while (true == isRepeated);

            string defaultActionName = DEFAULT_ACTION_NAME_PREFIX + orderAction.ToString();

            return defaultActionName;
        }
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
                if (NEW_NAME == propertyName)
                {
                    result = ValidationPropertyName(NewName);
                    m_IsValidNewName = string.IsNullOrEmpty(result);
                }
                return result;
            }
        }

        private string ValidationPropertyName(string propertyName)
        {
            foreach (SettingsViewModelBase viewModel in m_ListViewModels)
            {
                if (viewModel.Action.Name == propertyName)
                {
                    return string.Format("Action named {0} has existed. Type a different name.", viewModel.Action.Name);
                }
            }
            return null;
        }
        #endregion

        #region Private Members
        private string m_NewName;
        private string m_DefaultName;
        private List<SettingsViewModelBase> m_ListViewModels;
        private bool? m_DialogResult;
        private bool m_IsValidNewName;

        private ICommand m_NewNameCommand;
        private ICommand m_DefaultCommand;
        #endregion

        #region Constants
        private const string NEW_NAME = "NewName";
        private const string DEFAULT_ACTION_NAME_PREFIX = "Action_";
        #endregion
    }
}
