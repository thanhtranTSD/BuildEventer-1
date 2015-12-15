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

using System;
using System.Collections.Generic;

namespace BuildEventer.Models
{
    public class CopyActionModel : IActionModel
    {

        #region Constructors
        public CopyActionModel()
        {
            m_Name = DEFAULT_NAME;
            m_Type = TYPE;
        }

        public CopyActionModel(string name)
        {
            m_Name = name;
            m_Type = TYPE;
        }

        public CopyActionModel(string name, List<DragDropData> sources, List<DragDropData> destinations)
            : this(name)
        {
            m_Sources = sources;
            m_Destinations = destinations;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (m_Name != value)
                {
                    m_Name = value;
                }
            }
        }

        public string Type
        {
            get { return m_Type; }
        }

        public Type ClassType
        {
            get { return this.GetType(); }
        }

        public List<DragDropData> Sources
        {
            get { return m_Sources; }
            set
            {
                if (m_Sources != value)
                {
                    m_Sources = value;
                }
            }
        }

        public List<DragDropData> Destinations
        {
            get { return m_Destinations; }
            set
            {
                if (m_Destinations != value)
                {
                    m_Destinations = value;
                }
            }
        }
        #endregion

        #region Members
        private string m_Name;
        private string m_Type;
        private List<DragDropData> m_Sources;
        private List<DragDropData> m_Destinations;
        #endregion

        #region Constants
        private const string DEFAULT_NAME = "Copy Action";
        private const string TYPE = "Copy";
        #endregion
    }
}
