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

using BuildEventer.Models;
using BuildEventer.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;

namespace BuildEventer.Utilities
{
    static class ActionUtility
    {
        public static List<Type> GetAllActionTypes()
        {
            List<Type> actionTypes = new List<Type>();

            IEnumerable<Type> result = FindDerivedTypesFromAssembly(Assembly.GetEntryAssembly(), typeof(IActionModel), false);
            foreach (Type type in result)
            {
                actionTypes.Add(type);
            }

            return actionTypes;
        }

        public static Dictionary<Type, UserControl> GetAllViewModels()
        {
            Dictionary<Type, UserControl> dict = new Dictionary<Type, UserControl>();

            IEnumerable<Type> result = FindDerivedTypesFromAssembly(Assembly.GetEntryAssembly(), typeof(SettingsViewModelBase), false);
            foreach (Type type in result)
            {
                Type typeofView = GetViewTypeFromActionType(type);
                UserControl view = (UserControl)Activator.CreateInstance(typeofView);
                dict.Add(type, view);
            }

            return dict;
        }

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

        private static Type GetViewTypeFromActionType(Type typeName)
        {
            string fullName = typeName.FullName.Replace("ViewModels", "Views");
            string newFullName = fullName.Remove(fullName.Length - 5);
            return Type.GetType(newFullName);
        }
    }
}
