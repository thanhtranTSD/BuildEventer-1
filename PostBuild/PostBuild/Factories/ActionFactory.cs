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

using PostBuild.Action;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace PostBuild.Factories
{
    public class ActionFactory
    {
        #region Public Static Function
        public static ActionFactory Instance()
        {
            if (null == s_DefinitionActionFactory)
            {
                s_DefinitionActionFactory = new ActionFactory();
            }

            return s_DefinitionActionFactory;
        }
        #endregion

        #region Constructors
        private ActionFactory()
        {
            m_RegisterActionFactory = new Dictionary<string, Type>();
        }
        #endregion Constructors

        #region Public Functions
        public void Initialize()
        {
            IEnumerable<Type> result = FindDerivedTypesFromAssembly(Assembly.GetEntryAssembly(), typeof(IAction), false);
            foreach (Type type in result)
            {
                m_RegisterActionFactory.Add(type.Name, type);
            }
        }

        public IAction CreateActionInstance(string actionName, XmlNode actionNode, bool overwrite)
        {
            IAction returnAction = null;

            if (true == m_RegisterActionFactory.ContainsKey(actionName))
            {
                Type actionType = m_RegisterActionFactory[actionName];
                IAction typeOfAction = (IAction)Activator.CreateInstance(actionType, actionNode, overwrite);
                returnAction = typeOfAction;
            }

            return returnAction;
        }
        #endregion Public Functions

        #region Private Functions
        private static IEnumerable<Type> FindDerivedTypesFromAssembly(Assembly assembly, Type baseType, bool classOnly)
        {
            if (null == assembly)
            {
                throw new ArgumentNullException("assembly", "Assembly must be defined");
            }
            if (null == baseType)
            {
                throw new ArgumentNullException("baseType", "Parent Type must be defined");
            }
            // Get all the types
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                // If classOnly, it must be a class
                if ((true == classOnly) && (false == type.IsClass))
                    continue;
                if (true == baseType.IsInterface)
                {
                    Type interfaceType = type.GetInterface(baseType.FullName);
                    if (null != interfaceType)
                        // Add it to result list
                        yield return type;
                }
                else if (true == type.IsSubclassOf(baseType))
                {
                    // Add it to result list
                    yield return type;
                }
            }
        }
        #endregion

        #region Members
        private Dictionary<string, Type> m_RegisterActionFactory;
        private static ActionFactory s_DefinitionActionFactory;
        #endregion Members

    }
}
