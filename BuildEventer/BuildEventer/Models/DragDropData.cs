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

namespace BuildEventer.Models
{
    public class DragDropData : IDragDropData
    {

        #region Constructors
        public DragDropData(string path)
        {
            m_Path = path;
        }

        public DragDropData(string path, bool isFolder)
            : this(path)
        {
            this.IsFolder = isFolder;
        }
        #endregion

        #region Properties
        public string Path
        {
            get { return m_Path; }
        }

        public bool IsFolder { get; private set; }
        #endregion

        #region Members
        private string m_Path;
        #endregion
    }
}
