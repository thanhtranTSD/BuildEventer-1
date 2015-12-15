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
using BuildEventerDAL.XML;
using PostBuild.Factories;
using System;
using System.Collections.Generic;

namespace PostBuild.Action
{
    public static class ActionManager
    {
        #region Public Function
        /// <summary>
        /// Executes all the actions with their data.
        /// </summary>
        public static void ExecuteAllActions(List<IActionData> actionDataList, bool overwrite)
        {
            foreach (IActionData actionData in actionDataList)
            {
                string actionName = GetActionNameFromType(actionData.Type);
                IAction action = ActionFactory.Instance().CreateActionInstance(actionName, actionData, overwrite);

                PrintActionStart(actionData.Name);
                action.Execute();
                PrintActionSuccess(actionData.Name);
            }
        }
        #endregion

        #region Private Function
        private static void PrintActionStart(string actionName)
        {
            Console.WriteLine(String.Format("Execute {0}", actionName));
        }

        private static void PrintActionSuccess(string actionName)
        {
            Console.WriteLine(String.Format("Action {0} success.", actionName));
            Console.WriteLine();
        }

        /// <summary>
        /// Get the action name from the action type.
        /// </summary>
        internal static string GetActionNameFromType(string actionType)
        {
            string returnName = string.Empty;
            if (null != actionType)
            {
                actionType = actionType.Substring(actionType.LastIndexOf(XmlConstants.PATH_SEPARATOR_CHAR) + 1);
                returnName = actionType.Replace(MODEL_STRING, String.Empty);
            }

            return returnName;
        }
        #endregion

        #region Constants
        private const string MODEL_STRING = "Model";
        #endregion
    }
}
