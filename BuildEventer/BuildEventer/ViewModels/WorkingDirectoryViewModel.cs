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

using BuildEventer.Behaviors;
using BuildEventer.Command;
using BuildEventer.Models;
using BuildEventer.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BuildEventer.ViewModels
{
    public class WorkingDirectoryViewModel : ViewModelBase
    {
        #region Constructor

        public WorkingDirectoryViewModel(string workingDirectory)
        {
            m_WorkingDirectory = workingDirectory;
        }

        #endregion

        #region Properties

        public object SelectedItem
        {
            get
            {
                if (null == m_SelectedItem)
                {
                    m_SelectedItem = new object();
                }
                return m_SelectedItem;
            }
            set
            {
                if (m_SelectedItem != value)
                {
                    m_SelectedItem = value;
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        public string SelectedPath
        {
            get
            {
                return m_SelectedPath;
            }
            set
            {
                if (m_SelectedPath != value)
                {
                    m_SelectedPath = value;
                    OnPropertyChanged("SelectedPath");
                }
            }
        }

        public BindingList<TreeViewItem> WorkingDirectoryTreeViewItems
        {
            get
            {
                return m_WorkingDirectoryTreeViewItems;
            }
        }

        #endregion

        #region Commands

        #region SelectedItemChangedCommand

        public DelegateCommand<TreeViewHelper.DependencyPropertyEventArgs> SelectedItemChangedCommand
        {
            get
            {
                return m_SelectedItemChangedCommand ?? (m_SelectedItemChangedCommand = new DelegateCommand<TreeViewHelper.DependencyPropertyEventArgs>(TreeViewItemSelectedChangedCallBack));
            }
        }

        private void TreeViewItemSelectedChangedCallBack(TreeViewHelper.DependencyPropertyEventArgs e)
        {
            if ((null != e) && (null != e.DependencyPropertyChangedEventArgs.NewValue))
            {
                TreeViewItem treeViewItem = e.DependencyPropertyChangedEventArgs.NewValue as TreeViewItem;
                string header = treeViewItem.Header.ToString();
                string tag = treeViewItem.Tag.ToString();
            }
        }

        #endregion

        #region MouseMoveCommand
        private ICommand m_MouseMoveCommand;

        public ICommand MouseMoveCommand
        {
            get
            {
                return m_MouseMoveCommand ?? (m_MouseMoveCommand = new DelegateCommand<MouseEventArgs>(param => ExecuteMouseMove((MouseEventArgs)param)));
            }
        }

        private void ExecuteMouseMove(MouseEventArgs e)
        {
            WorkingDirectoryView explorerView = e.Source as WorkingDirectoryView;
            if ((null != explorerView) && (e.LeftButton == MouseButtonState.Pressed))
            {
                TreeViewItem treeviewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
                if (null != treeviewItem)
                {
                    string fullPath = (string)treeviewItem.Tag;

                    string relativePath = GetRelativePath(fullPath);

                    if (string.Empty != relativePath)
                    {
                        DragDropData data = new DragDropData(relativePath, Directory.Exists(fullPath));
                        DragDrop.DoDragDrop(treeviewItem, data, DragDropEffects.Copy);
                    }
                }
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
        #endregion MouseMoveCommand

        #region LoadedCommand
        private ICommand m_TreeViewLoadedCommand;

        public ICommand TreeViewLoadedCommand
        {
            get
            {
                return m_TreeViewLoadedCommand ?? (m_TreeViewLoadedCommand = new DelegateCommand(param => TreeViewLoaded((RoutedEventArgs)param)));
            }
        }
        #endregion LoadedCommand

        #endregion

        #region Public Functions

        public void SetWorkingDirectory(string workingDirectory)
        {
            m_WorkingDirectory = workingDirectory;
        }

        #endregion

        #region Private Functions

        // Helper to search up the VisualTree
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            } while (null != current);

            return null;
        }

        private void FindDropTarget(TreeView treeView, out TreeViewItem itemNode, DragEventArgs dragEventArgs)
        {
            itemNode = null;

            DependencyObject k = VisualTreeHelper.HitTest(treeView, dragEventArgs.GetPosition(treeView)).VisualHit;

            while (null != k)
            {
                if (k is TreeViewItem)
                {
                    TreeViewItem treeNode = k as TreeViewItem;
                    itemNode = treeNode;
                }
                else if (k == treeView)
                {
                    return;
                }

                k = VisualTreeHelper.GetParent(k);
            }
        }

        private void FolderExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if ((1 == item.Items.Count) && (item.Items[0] == m_DummyNode))
            {
                item.Items.Clear();
                foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                {
                    TreeViewItem subitem = new TreeViewItem();
                    subitem.Header = s.Substring(s.LastIndexOf(PATH_SEPARATOR_CHAR) + 1);
                    subitem.Tag = s;
                    subitem.FontWeight = FontWeights.Normal;
                    subitem.Items.Add(m_DummyNode);
                    subitem.Expanded += new RoutedEventHandler(FolderExpanded);
                    item.Items.Add(subitem);

                }

                foreach (string file in Directory.GetFiles(item.Tag.ToString()))
                {
                    TreeViewItem fileItem = new TreeViewItem();
                    fileItem.Header = file.Substring(file.LastIndexOf(PATH_SEPARATOR_CHAR) + 1);
                    fileItem.Tag = file;
                    fileItem.FontWeight = FontWeights.Normal;
                    item.Items.Add(fileItem);
                }
            }
        }

        private void TreeViewLoaded(RoutedEventArgs e)
        {
            m_WorkingDirectoryTreeViewItems = new BindingList<TreeViewItem>();

            if (null != m_WorkingDirectory)
            {
                bool hasDir = Directory.Exists(m_WorkingDirectory);

                string path = Path.GetFullPath(m_WorkingDirectory);
                string shortPath = ShortPath(path, WORKING_DIRECTORY_LENGHT);

                TreeViewItem item = new TreeViewItem();
                item.Header = shortPath;
                item.Tag = path;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(m_DummyNode);
                item.Expanded += new RoutedEventHandler(FolderExpanded);

                m_WorkingDirectoryTreeViewItems.Add(item);
            }
            OnPropertyChanged("WorkingDirectoryTreeViewItems");
        }

        /// <summary>
        /// Help to reduce the lenght of path.
        /// </summary>
        private string ShortPath(string fullPath, int lenght)
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

                return ShortPath(newFullPath, lenght);
            }
        }

        #endregion

        #region Members
        private string m_SelectedPath;
        private BindingList<TreeViewItem> m_WorkingDirectoryTreeViewItems;

        private object m_DummyNode;

        public string m_WorkingDirectory;
        private object m_SelectedItem;
        private DelegateCommand<TreeViewHelper.DependencyPropertyEventArgs> m_SelectedItemChangedCommand;
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
