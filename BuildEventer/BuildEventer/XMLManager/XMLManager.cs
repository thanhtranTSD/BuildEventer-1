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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;

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
        public static ObservableCollection<SettingsViewModelBase> LoadXmlFile(string filePath)
        {
            ObservableCollection<SettingsViewModelBase> returnList = new ObservableCollection<SettingsViewModelBase>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            // Select the Action node in the XML file
            XmlNodeList actionList = xmlDocument.SelectNodes(XML_ACTION_PATH);
            foreach (XmlNode action in actionList)
            {
                // Get the name and type attribute
                string name = action.Attributes.GetNamedItem(XML_NAME_ATTRIBUTE).Value;
                string type = action.Attributes.GetNamedItem(XML_TYPE_ATTRIBUTE).Value;

                // Select the Sources node and get all the paths
                XmlNode sourceList = action.SelectSingleNode(XML_SOURCES_NODE);
                List<DragDropData> sources = new List<DragDropData>();
                foreach (XmlNode node in sourceList)
                {
                    DragDropData data = new DragDropData(node.InnerText);
                    sources.Add(data);
                }

                // Select the Destinations node and get all the paths
                XmlNode destinationList = action.SelectSingleNode(XML_DESTINATIONS_NODE);
                List<DragDropData> destinations = new List<DragDropData>();
                foreach (XmlNode node in destinationList)
                {
                    DragDropData data = new DragDropData(node.InnerText);
                    destinations.Add(data);
                }

                // Get the type an action
                Type actionType = Type.GetType(type);

                // Create instance with input arguments
                object[] arguments = new object[3] { name, sources, destinations };
                Object actionInstance = Activator.CreateInstance(actionType, arguments);

                SettingsViewModelBase copyActionVM = Factories.SettingsViewModelFactory.Instance().CreateSettingsViewModel(actionType.Name, actionInstance);

                returnList.Add(copyActionVM);
            }

            return returnList;
        }

        /// <summary>
        /// Save all the actions into XML file
        /// </summary>
        public static void GenerateXmlFile(string filePath, ObservableCollection<SettingsViewModelBase> actionViewModels)
        {
            XmlDocument doc = new XmlDocument();
            // Declare the format for XML file
            XmlDeclaration declaration = doc.CreateXmlDeclaration(XML_VERSION, XML_ENCODING, null);
            doc.AppendChild(declaration);

            // Create the Object node in XML file
            XmlElement objectNode = doc.CreateElement(XML_OBJECT_NODE);

            // Create the Actions node in XML file
            XmlElement actionsNode = doc.CreateElement(XML_ACTIONS_NODE);

            // Create an Action node
            foreach (SettingsViewModelBase actionVM in actionViewModels)
            {
                ActionViewModelToXML(actionVM, ref actionsNode, ref doc);
            }
            objectNode.AppendChild(actionsNode);
            doc.AppendChild(objectNode);
            doc.Save(filePath);
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

        /// <summary>
        /// Generate the XML action node from the input action's viewmodel.
        /// </summary>
        private static void ActionViewModelToXML(SettingsViewModelBase actionViewModel, ref XmlElement actionsNode, ref XmlDocument document)
        {
            string viewModelName = actionViewModel.GetType().Name;

            eActionViewModel enumViewModel = ParseEnum<eActionViewModel>(viewModelName);
            if (null != enumViewModel)
            {
                switch (enumViewModel)
                {
                    case eActionViewModel.CopyActionViewModel:
                        {
                            actionsNode.AppendChild(CopyActionViewModelToXMLNode((CopyActionViewModel)actionViewModel, ref document));
                            break;
                        }
                    default: break;
                }
            }
        }

        #region For CopyActionViewModel
        /// <summary>
        /// Create a node from a list of paths
        /// </summary>
        private static XmlNode PathsToXMLNode(ObservableCollection<DragDropData> listPath, string nodeName, ref XmlDocument document)
        {
            XmlElement xmlNode = document.CreateElement(nodeName);

            foreach (DragDropData path in listPath)
            {
                XmlElement pathNode = document.CreateElement(XML_PATH_NODE);
                pathNode.InnerText = path.Path;
                xmlNode.AppendChild(pathNode);
            }

            return xmlNode;
        }

        /// <summary>
        /// Generate the copyaction viewmodel to XML node.
        /// </summary>
        private static XmlNode CopyActionViewModelToXMLNode(CopyActionViewModel copyActionVM, ref XmlDocument document)
        {
            XmlElement actionNode = document.CreateElement(XML_ACTION_NODE);
            actionNode.SetAttribute(XML_NAME_ATTRIBUTE, copyActionVM.CopyAction.Name);
            actionNode.SetAttribute(XML_TYPE_ATTRIBUTE, copyActionVM.CopyAction.ClassType.FullName);
            actionNode.AppendChild(PathsToXMLNode(copyActionVM.CopyAction.Sources, XML_SOURCES_NODE, ref document));
            actionNode.AppendChild(PathsToXMLNode(copyActionVM.CopyAction.Destinations, XML_DESTINATIONS_NODE, ref document));

            return actionNode;
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

        #region Constants
        // For XML file
        private const string XML_VERSION = "1.0";
        private const string XML_ENCODING = "UTF-8";
        private const string RELATIVE_PATH_SIGNATURE = ".";
        private const string XML_OBJECT_NODE = "Object";
        private const string XML_ACTIONS_NODE = "Actions";
        private const string XML_ACTION_NODE = "Action";
        private const string XML_ACTION_PATH = "/Object/Actions/Action";
        private const string XML_SOURCES_NODE = "Sources";
        private const string XML_DESTINATIONS_NODE = "Destinations";
        private const string XML_PATH_NODE = "Path";
        private const string XML_NAME_ATTRIBUTE = "Name";
        private const string XML_TYPE_ATTRIBUTE = "Type";
        private const string XML_IS_FOLDER_ATTRIBUTE = "IsFolder";
        private const string XML_EXTENSION_ATTRIBUTE = "extension";
        #endregion
    }
}
