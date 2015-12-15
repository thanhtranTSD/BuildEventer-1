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
using PostBuild.Utilites;
using System;
using System.Collections.Generic;
using System.IO;

namespace PostBuild.Action
{
    public class CopyAction : IAction
    {
        #region Constructor
        public CopyAction(CopyActionData copyActionData, bool overwrite)
        {
            this.m_Sources = copyActionData.Sources;
            this.m_Destinations = copyActionData.Destinations;
            this.m_Overwrite = overwrite;
        }
        #endregion

        #region Public Functions
        public void Execute()
        {
            foreach (string source in m_Sources)
            {
                if (false == FileOrDirectoryExists(source))
                {
                    Console.WriteLine(String.Format("{0}  does not exist.", source));
                    continue;
                }
                foreach (string dest in m_Destinations)
                {
                    DoCopyAction(source, dest, m_Overwrite);
                }
            }
        }
        #endregion

        #region Private Function
        private void DoCopyAction(string source, string dest, bool overwrite = false)
        {
            if (true == File.Exists(source))
            {
                FolderCopy.FileCopy(source, dest, overwrite);
            }
            else if (true == Directory.Exists(source))
            {
                var s = new DirectoryInfo(source);
                string newPathDest = Path.Combine(dest, s.Name);
                FolderCopy.DirectoryCopy(source, newPathDest, true, overwrite);
            }
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
