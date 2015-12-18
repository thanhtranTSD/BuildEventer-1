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
using BuildEventerDAL.Factories;
using System;
using System.Collections.Generic;
using System.Xml;

namespace BuildEventerDAL.XML
{
    public class XmlParser
    {
        #region Public Static Function
        public static XmlParser Instance()
        {
            if (null == s_DefinitionXmlParsery)
            {
                s_DefinitionXmlParsery = new XmlParser();
            }

            return s_DefinitionXmlParsery;
        }
        #endregion

        #region Constructor
        private XmlParser()
        {
            ActionDataFactory.Instance().Initialize();
        }
        #endregion

        #region internal Functions
        /// <summary>
        /// Load the xml configuration file.
        /// </summary>
        internal List<IActionData> LoadXmlFile(string filePath, bool removeRelativePathSingal = false)
        {
            List<IActionData> returnList = new List<IActionData>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            // Select the Action node in the XML file
            XmlNodeList actionList = GetAllActions(xmlDocument);
            foreach (XmlNode action in actionList)
            {
                // Get the type attribute
                string type = GetActionType(action);
                type = GetActionDataNameFromType(type);

                IActionData copyActionData = ActionDataFactory.Instance().CreateActionInstance(type, action, removeRelativePathSingal);

                returnList.Add(copyActionData);
            }

            return returnList;
        }

        internal void GenerateXmlFile(string filePath, List<IActionData> actionsData)
        {
            XmlDocument doc = new XmlDocument();
            // Declare the format for XML file
            XmlDeclaration declaration = doc.CreateXmlDeclaration(XmlConstants.XML_VERSION, XmlConstants.XML_ENCODING, null);
            doc.AppendChild(declaration);

            // Create the Object node in XML file
            XmlElement objectNode = doc.CreateElement(XmlConstants.XML_OBJECT_NODE);

            // Create the Actions node in XML file
            XmlElement actionsNode = doc.CreateElement(XmlConstants.XML_ACTIONS_NODE);

            // Create an Action node
            foreach (IActionData actionData in actionsData)
            {
                //Type type = Type.GetType(actionData.);

                ActionDataToXML(actionData, ref actionsNode, ref doc);
            }
            objectNode.AppendChild(actionsNode);
            doc.AppendChild(objectNode);
            doc.Save(filePath);
        }

        /// <summary>
        /// Get the name of Action node.
        /// </summary>
        internal static string GetActionName(XmlNode actionNode)
        {
            return actionNode.Attributes.GetNamedItem(XmlConstants.XML_NAME_ATTRIBUTE).Value;
        }

        /// <summary>
        /// Get the full type name of Action node.
        /// </summary>
        internal static string GetActionType(XmlNode actionNode)
        {
            return actionNode.Attributes.GetNamedItem(XmlConstants.XML_TYPE_ATTRIBUTE).Value;
        }
        #endregion

        #region Private Functions

        /// <summary>
        /// Get all the Action node in xml file.
        /// </summary>
        private static XmlNodeList GetAllActions(XmlDocument xmlDocument)
        {
            return xmlDocument.SelectNodes(XmlConstants.XML_ACTION_PATH);
        }

        private static string GetActionDataNameFromType(string type)
        {
            string returnType = string.Empty;
            if (null != type)
            {
                type = type.Substring(type.LastIndexOf(XmlConstants.PATH_SEPARATOR_CHAR) + 1);
                returnType = type.Replace(MODEL_STRING, DATA_STRING);
            }
            return returnType;
        }

        /// <summary>
        /// Try to convert string to enum.
        /// </summary>
        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private static void ActionDataToXML(IActionData actionData, ref XmlElement actionsNode, ref XmlDocument document)
        {
            string actionDataName = actionData.GetType().Name;

            eActionData enumViewModel = ParseEnum<eActionData>(actionDataName);
            switch (enumViewModel)
            {
                case eActionData.CopyActionData:
                    {
                        actionsNode.AppendChild(CopyActionDataToXMLNode((CopyActionData)actionData, ref document));
                        break;
                    }
                default: break;
            }
        }

        #region For CopyActionData
        /// <summary>
        /// Create a node from a list of paths
        /// </summary>
        private static XmlNode PathsToXMLNode(List<string> listPath, string nodeName, ref XmlDocument document)
        {
            XmlElement xmlNode = document.CreateElement(nodeName);

            foreach (string path in listPath)
            {
                XmlElement pathNode = document.CreateElement(XmlConstants.XML_PATH_NODE);
                pathNode.InnerText = path;
                xmlNode.AppendChild(pathNode);
            }

            return xmlNode;
        }

        /// <summary>
        /// Generate the copyaction viewmodel to XML node.
        /// </summary>
        private static XmlNode CopyActionDataToXMLNode(CopyActionData copyActionData, ref XmlDocument document)
        {
            XmlElement actionNode = document.CreateElement(XmlConstants.XML_ACTION_NODE);
            actionNode.SetAttribute(XmlConstants.XML_NAME_ATTRIBUTE, copyActionData.Name);
            actionNode.SetAttribute(XmlConstants.XML_TYPE_ATTRIBUTE, copyActionData.Type);
            actionNode.AppendChild(PathsToXMLNode(copyActionData.Sources, XmlConstants.XML_SOURCES_NODE, ref document));
            actionNode.AppendChild(PathsToXMLNode(copyActionData.Destinations, XmlConstants.XML_DESTINATIONS_NODE, ref document));

            return actionNode;
        }
        #endregion

        #endregion

        #region Enum
        // List all the action's viewmodels here.
        private enum eActionData
        {
            CopyActionData,
        }
        #endregion

        #region Member
        private static XmlParser s_DefinitionXmlParsery;
        #endregion

        #region Constants
        private const string MODEL_STRING = "Model";
        private const string DATA_STRING = "Data";
        #endregion
    }
}
