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

using BuildEventerDAL.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace BuildEventerDAL.Factories
{
    public class ActionDataFactory
    {
        #region Public Static Function
        public static ActionDataFactory Instance()
        {
            if (null == s_DefinitionActionDataFactory)
            {
                s_DefinitionActionDataFactory = new ActionDataFactory();
            }

            return s_DefinitionActionDataFactory;
        }
        #endregion

        #region Constructors
        private ActionDataFactory()
        {
            m_RegisterActionDataFactory = new Dictionary<string, Type>();
        }
        #endregion Constructors

        #region Public Functions
        public void Initialize()
        {
            IEnumerable<Type> result = FindDerivedTypesFromAssembly(Assembly.GetExecutingAssembly(), typeof(IActionData), false);
            foreach (Type type in result)
            {
                m_RegisterActionDataFactory.Add(type.Name, type);
            }
        }

        public IActionData CreateActionInstance(string actionName, XmlNode actionNode, bool removeRelativePathSingal)
        {
            IActionData returnAction = null;

            if (true == m_RegisterActionDataFactory.ContainsKey(actionName))
            {
                Type actionType = m_RegisterActionDataFactory[actionName];
                IActionData typeOfAction = (IActionData)Activator.CreateInstance(actionType, actionNode, removeRelativePathSingal);
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
                {
                    continue;
                }
                if (true == baseType.IsInterface)
                {
                    Type interfaceType = type.GetInterface(baseType.FullName);
                    if (null != interfaceType)
                    {
                        // Add it to result list
                        yield return type;
                    }
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
        private Dictionary<string, Type> m_RegisterActionDataFactory;
        private static ActionDataFactory s_DefinitionActionDataFactory;
        #endregion Members

    }
}
