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

using PostBuild.Utilites;
using PostBuild.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PostBuild.Action
{
    class CopyAction : IAction
    {
        #region Constructor
        public CopyAction(XmlNode copyActionNode, bool overwrite)
        {
            this.m_Sources = GetSources(copyActionNode);
            this.m_Destinations = GetDestinations(copyActionNode);
            this.m_Overwrite = overwrite;
        }
        #endregion

        #region Public Functions
        public void Execute()
        {
            foreach (string pathSource in m_Sources)
            {
                if (false == FileOrDirectoryExists(pathSource))
                {
                    Console.WriteLine(String.Format("{0}  does not exist.", pathSource));
                    continue;
                }
                foreach (string pathDest in m_Destinations)
                {
                    DoCopyAction(pathSource, pathDest, m_Overwrite);
                }
            }
        }
        #endregion

        #region Private Function
        private void DoCopyAction(string pathSource, string pathDest, bool overwrite = false)
        {
            if (true == File.Exists(pathSource))
            {
                Copy.FileCopy(pathSource, pathDest, overwrite);
            }
            else if (true == Directory.Exists(pathSource))
            {
                var s = new DirectoryInfo(pathSource);
                string newPathDest = Path.Combine(pathDest, s.Name);
                Copy.DirectoryCopy(pathSource, newPathDest, true, overwrite);
            }
        }

        private static List<string> GetSources(XmlNode actionNode)
        {
            List<string> sources = new List<string>();

            string name = actionNode.Attributes.GetNamedItem(XmlConstants.XML_NAME_ATTRIBUTE).Value;
            string type = actionNode.Attributes.GetNamedItem(XmlConstants.XML_TYPE_ATTRIBUTE).Value;

            // Select the Sources node and get all the paths
            XmlNode sourceList = actionNode.SelectSingleNode(XmlConstants.XML_SOURCES_NODE);
            foreach (XmlNode node in sourceList)
            {
                string relativePath = (node.InnerText).TrimStart(Path.DirectorySeparatorChar);
                sources.Add(relativePath);
            }

            return sources;
        }

        private static List<string> GetDestinations(XmlNode actionNode)
        {
            List<string> destinations = new List<string>();

            // Get the name and type attribute
            string name = actionNode.Attributes.GetNamedItem(XmlConstants.XML_NAME_ATTRIBUTE).Value;
            string type = actionNode.Attributes.GetNamedItem(XmlConstants.XML_TYPE_ATTRIBUTE).Value;

            // Select the Destinations node and get all the paths
            XmlNode destinationList = actionNode.SelectSingleNode(XmlConstants.XML_DESTINATIONS_NODE);
            foreach (XmlNode node in destinationList)
            {
                string relativePath = (node.InnerText).TrimStart(Path.DirectorySeparatorChar);
                destinations.Add(relativePath);
            }

            return destinations;
        }

        private static bool FileOrDirectoryExists(string path)
        {
            return (Directory.Exists(path) || File.Exists(path));
        }
        #endregion

        #region Members
        private List<string> m_Sources;
        private List<string> m_Destinations;
        private bool m_Overwrite;
        #endregion
    }
}
