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
using BuildEventerDAL;
using BuildEventerDAL.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace BuildEventer
{
    public static class XMLManager
    {
        #region Public Functions
        /// <summary>
        /// Get the fullpath from dialog
        /// </summary>
        public static string GetPathToLoadFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "xml files (*.xml)|*.xml";
            ofd.Title = "Load XML File";
            ofd.InitialDirectory = Directory.GetCurrentDirectory();
            if (true == ofd.ShowDialog())
            {
                return ofd.FileName;
            }
            return null;
        }

        public static string GetPathToSaveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "xml files (*.xml)|*.xml";
            sfd.FileName = "BuildEventer";
            sfd.Title = "Save XML File";
            sfd.InitialDirectory = Directory.GetCurrentDirectory();
            if (true == sfd.ShowDialog())
            {
                return sfd.FileName;
            }
            return null;
        }

        /// <summary>
        /// Load all the actions in the XML file.
        /// </summary>
        public static ObservableCollection<SettingsViewModelBase> LoadConfigurationFile(string filePath)
        {
            ObservableCollection<SettingsViewModelBase> returnList = new ObservableCollection<SettingsViewModelBase>();

            List<IActionData> actionDataList = DataAccessLayer.LoadFile(filePath);

            foreach (IActionData actionData in actionDataList)
            {
                Type actionType = Type.GetType(actionData.Type);
                Object actionInstance = Activator.CreateInstance(actionType, actionData);
                SettingsViewModelBase actionViewModel = Factories.SettingsViewModelFactory.Instance().CreateSettingsViewModel(actionType.Name, actionInstance);

                returnList.Add(actionViewModel);
            }

            return returnList;
        }

        public static void GenerateConfigurationFile(string filePath, ObservableCollection<SettingsViewModelBase> actionViewModels)
        {
            List<IActionData> actionsData = ConvertActionViewModelToActionData(actionViewModels);

            DataAccessLayer.GenerateFile(filePath, actionsData);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Try to convert string to enum.
        /// </summary>
        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private static List<IActionData> ConvertActionViewModelToActionData(ObservableCollection<SettingsViewModelBase> viewModels)
        {
            List<IActionData> returnList = new List<IActionData>();

            foreach (SettingsViewModelBase viewModel in viewModels)
            {
                string viewModelName = viewModel.GetType().Name;

                eActionViewModel enumViewModel = ParseEnum<eActionViewModel>(viewModelName);
                switch (enumViewModel)
                {
                    case eActionViewModel.CopyActionViewModel:
                        {
                            returnList.Add(ConvertCopyViewModelToCopyData((CopyActionViewModel)viewModel));
                            break;
                        }
                    default: break;
                }
            }

            return returnList;
        }

        #region Convert for CopyAction
        /// <summary>
        /// Converts CopyAction viewmodel to CopyAction data.
        /// </summary>
        private static CopyActionData ConvertCopyViewModelToCopyData(CopyActionViewModel copyActionViewModel)
        {
            string name = copyActionViewModel.CopyAction.Name;
            string type = copyActionViewModel.CopyAction.ClassType.FullName;
            List<string> sources = ConvertPathsFromCopyViewModelToCopyData(copyActionViewModel.CopyAction.Sources);
            List<string> destinations = ConvertPathsFromCopyViewModelToCopyData(copyActionViewModel.CopyAction.Destinations);
            return new CopyActionData(name, type, sources, destinations);
        }

        /// <summary>
        /// Converts paths including sources and destinations in CopyAction viewmodel to paths in CopyAction data.
        /// </summary>
        private static List<string> ConvertPathsFromCopyViewModelToCopyData(List<DragDropData> paths)
        {
            List<string> returnList = new List<string>();
            foreach (DragDropData path in paths)
            {
                returnList.Add(path.Path);
            }
            return returnList;
        }
        #endregion

        #endregion

        #region Enum
        // List all the action's viewmodels here.
        private enum eActionViewModel
        {
            CopyActionViewModel,
        }
        #endregion
    }
}
