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

using BuildEventerDAL.XML;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BuildEventerDAL.Data
{
    public class CopyActionData : IActionData
    {
        #region Constructors
        public CopyActionData(string name, string type, List<string> sources, List<string> destinations)
        {
            this.Name = name;
            this.Type = type;
            this.Sources = sources;
            this.Destinations = destinations;
        }

        public CopyActionData(XmlNode copyActionNode, bool removeRelativePathSingal)
        {
            this.Name = XmlParser.GetActionName(copyActionNode);
            this.Type = XmlParser.GetActionType(copyActionNode);
            this.Sources = GetSources(copyActionNode, removeRelativePathSingal);
            this.Destinations = GetDestinations(copyActionNode, removeRelativePathSingal);
        }
        #endregion

        #region Properties
        public string Name { get; protected set; }

        public string Type { get; protected set; }

        public List<string> Sources { get; private set; }

        public List<string> Destinations { get; private set; }
        #endregion

        #region Private Functions
        /// <summary>
        /// Get the Sources node of a copy action node.
        /// </summary>
        private List<string> GetSources(XmlNode actionNode, bool removeRelativePathSingal)
        {
            List<string> sources = new List<string>();

            string name = actionNode.Attributes.GetNamedItem(XmlConstants.XML_NAME_ATTRIBUTE).Value;
            string type = actionNode.Attributes.GetNamedItem(XmlConstants.XML_TYPE_ATTRIBUTE).Value;

            // Select the Sources node and get all the paths
            XmlNode sourceList = actionNode.SelectSingleNode(XmlConstants.XML_SOURCES_NODE);
            foreach (XmlNode node in sourceList)
            {
                string relativePath = (node.InnerText);
                if (true == removeRelativePathSingal)
                {
                    relativePath = relativePath.TrimStart(Path.DirectorySeparatorChar);
                }
                sources.Add(relativePath);
            }

            return sources;
        }

        /// <summary>
        /// Get the Destinations node of a copy action node.
        /// </summary>
        private List<string> GetDestinations(XmlNode actionNode, bool removeRelativePathSingal)
        {
            List<string> destinations = new List<string>();

            // Get the name and type attribute
            string name = actionNode.Attributes.GetNamedItem(XmlConstants.XML_NAME_ATTRIBUTE).Value;
            string type = actionNode.Attributes.GetNamedItem(XmlConstants.XML_TYPE_ATTRIBUTE).Value;

            // Select the Destinations node and get all the paths
            XmlNode destinationList = actionNode.SelectSingleNode(XmlConstants.XML_DESTINATIONS_NODE);
            foreach (XmlNode node in destinationList)
            {
                string relativePath = (node.InnerText);
                if (true == removeRelativePathSingal)
                {
                    relativePath = relativePath.TrimStart(Path.DirectorySeparatorChar);
                }
                destinations.Add(relativePath);
            }

            return destinations;
        }
        #endregion
    }
}
