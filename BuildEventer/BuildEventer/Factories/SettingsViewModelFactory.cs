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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BuildEventer.Factories
{
    public class SettingsViewModelFactory
    {
        #region Public Static Function
        public static SettingsViewModelFactory Instance()
        {
            if (null == s_DefinitionSettingsViewModelFactory)
            {
                s_DefinitionSettingsViewModelFactory = new SettingsViewModelFactory();
            }

            return s_DefinitionSettingsViewModelFactory;
        }
        #endregion

        #region Constructors
        private SettingsViewModelFactory()
        {
            m_RegisterSettingsViewModel = new Dictionary<string, Type>();
        }
        #endregion Constructors

        #region Public Functions
        public void Initialize()
        {
            foreach (Type entityTypes in Assembly.GetAssembly(typeof(SettingsViewModelBase)).GetTypes()
                                                 .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SettingsViewModelBase))))
            {
                Regex regex = new Regex(@"\b\w+(?=ViewModel\b)");
                Match name = regex.Match(entityTypes.Name);
                RegisterDefinitionTestObject(name.ToString() + "Model", entityTypes);
            }
        }

        public SettingsViewModelBase CreateSettingsViewModel(string actionType, Object obj)
        {
            SettingsViewModelBase returnObject = null;

            if (true == m_RegisterSettingsViewModel.ContainsKey(actionType))
            {
                Type entityType = m_RegisterSettingsViewModel[actionType];
                SettingsViewModelBase typeOfObject = (SettingsViewModelBase)Activator.CreateInstance(entityType, obj);

                returnObject = typeOfObject;
            }

            return returnObject;
        }
        #endregion Public Functions

        #region Private Functions
        private void RegisterDefinitionTestObject(String name, Type classType)
        {
            if (false == m_RegisterSettingsViewModel.ContainsKey(name))
            {
                m_RegisterSettingsViewModel.Add(name, classType);
            }
        }
        #endregion

        #region Members
        private Dictionary<string, Type> m_RegisterSettingsViewModel;
        private static SettingsViewModelFactory s_DefinitionSettingsViewModelFactory;
        #endregion Members
    }
}
