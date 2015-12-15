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

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace BuildEventer.ViewModels
{
    public class WorkingDirectoryViewModel : ViewModelBase
    {
        #region Constructor
        public WorkingDirectoryViewModel(string workingDirectory)
        {
            m_WorkingDirectory = workingDirectory;
            WorkingDirectoryInitialize();
        }
        #endregion

        #region Properties
        public ObservableCollection<DragDropViewModel> WorkingDirectoryItems { get; private set; }
        #endregion

        #region Private Functions
        private void WorkingDirectoryInitialize()
        {
            WorkingDirectoryItems = new ObservableCollection<DragDropViewModel>();

            if (null != m_WorkingDirectory)
            {
                if (false == Directory.Exists(m_WorkingDirectory))
                {
                    return;
                }

                string fullPath = Path.GetFullPath(m_WorkingDirectory);
                string shortPath = GetShortPath(fullPath, WORKING_DIRECTORY_LENGHT);

                WorkingDirectoryItems.Add(GetFolderFromPath(shortPath, fullPath, null));
            }
        }

        private void FolderExpanded(object sender)
        {
            FolderViewModel folder = (FolderViewModel)sender;
            string folderPath = folder.Data.FullPath.ToString();

            if (false == Directory.Exists(folderPath))
            {
                return;
            }

            if (((1 == folder.Items.Count) && (folder.Items[0] == m_EmptyFolder)))
            {
                folder.Items.Clear();
                foreach (string dirPath in Directory.GetDirectories(folder.Data.FullPath.ToString()))
                {
                    string path = dirPath.Substring(dirPath.LastIndexOf(PATH_SEPARATOR_CHAR) + 1);
                    string relativePath = GetRelativePath(dirPath);
                    folder.Add(GetFolderFromPath(path, dirPath, relativePath, folder));
                }

                foreach (string filePath in Directory.GetFiles(folder.Data.FullPath.ToString()))
                {
                    string pathFile = filePath.Substring(filePath.LastIndexOf(PATH_SEPARATOR_CHAR) + 1);
                    string relativePath = GetRelativePath(filePath);
                    FileViewModel file = new FileViewModel(pathFile, filePath, relativePath);
                    folder.Add(file);
                }
            }
        }

        private bool IsEmptyFolder(string folderPath)
        {
            return (false == Directory.EnumerateFileSystemEntries(folderPath, "*", SearchOption.TopDirectoryOnly).Any());
        }

        private FolderViewModel GetFolderFromPath(string path, string fullPath, string relativePath, FolderViewModel parent = null)
        {
            FolderViewModel folder = new FolderViewModel(path, fullPath, relativePath, parent);
            if (false == IsEmptyFolder(fullPath))
            {
                folder.Add(m_EmptyFolder);
                folder.Expanded += FolderExpanded;
            }
            return folder;
        }

        /// <summary>
        /// Help to reduce the lenght of path.
        /// </summary>
        private string GetShortPath(string fullPath, int lenght)
        {
            string[] pathItems = fullPath.Split(Path.DirectorySeparatorChar);
            if ((lenght >= fullPath.Length) || (PATH_ITEM_LEVEL >= pathItems.Length))
            {
                return fullPath;
            }
            else
            {
                pathItems[ROOT_LEVEL] = Path.GetPathRoot(fullPath);
                pathItems = pathItems.Where(p => p != pathItems[REPLACE_POSITION]).ToArray();
                pathItems[REPLACE_POSITION] = SHORT_PATH_SYMBOL;
                string newFullPath = Path.Combine(pathItems);

                return GetShortPath(newFullPath, lenght);
            }
        }

        private string GetRelativePath(string fullPath)
        {
            if (null == fullPath)
            {
                return null;
            }
            return fullPath.Substring(m_WorkingDirectory.Length, fullPath.Length - m_WorkingDirectory.Length);
        }
        #endregion

        #region Members
        private FolderViewModel m_EmptyFolder;

        public string m_WorkingDirectory;
        #endregion

        #region Constants
        private const int WORKING_DIRECTORY_LENGHT = 80;
        private const int PATH_ITEM_LEVEL = 4;
        private const int ROOT_LEVEL = 0;
        private const int REPLACE_POSITION = 2;

        private const string SHORT_PATH_SYMBOL = "..";
        private const string PATH_SEPARATOR_CHAR = "\\";
        #endregion
    }
}
