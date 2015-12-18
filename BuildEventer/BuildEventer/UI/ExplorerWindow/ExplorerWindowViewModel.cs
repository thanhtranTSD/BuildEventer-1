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
using BuildEventer.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BuildEventer.UI
{
    public class ExplorerWindowViewModel : ViewModelBase
    {
        #region Constructor

        public ExplorerWindowViewModel(string path)
        {
            TreeLoaded();
            ExpandToFolder(WorkingDirectory, path);
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
                m_SelectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        }

        public BindingList<TreeViewItem> WorkingDirectory { get; private set; }

        public bool? DialogResult
        {
            get
            {
                return m_DialogResult;
            }
            private set
            {
                m_DialogResult = value;
                OnPropertyChanged("DialogResult");
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
                SelectedPath = treeViewItem.Tag.ToString();
            }
        }
        #endregion

        #endregion

        #region Private Functions

        private static void FolderExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if ((1 == item.Items.Count) && (item.Items[0] == dummyNode))
            {
                item.Items.Clear();
                foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                {
                    TreeViewItem subitem = new TreeViewItem();
                    subitem.Header = s.Substring(s.LastIndexOf(PATH_SEPARATOR_CHAR) + 1);
                    subitem.Tag = s;
                    subitem.FontWeight = FontWeights.Normal;
                    subitem.Items.Add(dummyNode);
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

        private void TreeLoaded()
        {
            WorkingDirectory = new BindingList<TreeViewItem>();

            foreach (string driveName in Directory.GetLogicalDrives())
            {
                DriveInfo driveInfo = new DriveInfo(driveName);
                if (true == driveInfo.IsReady)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = driveName;
                    item.Tag = driveName;
                    item.FontWeight = FontWeights.Normal;
                    item.Items.Add(dummyNode);
                    item.Expanded += new RoutedEventHandler(FolderExpanded);
                    WorkingDirectory.Add(item);
                }
            }
        }

        private void ExpandItem(ItemCollection items, string node)
        {
            foreach (TreeViewItem item in items)
            {
                if (true == node.StartsWith(item.Tag.ToString()))
                {
                    item.IsExpanded = true;
                    item.IsSelected = true;
                    item.Focus();

                    if (node == item.Tag.ToString())
                    {
                        return;
                    }
                    else
                    {
                        ExpandItem(item.Items, node);
                    }
                }
            }
        }

        /// <summary>
        /// Expand the tree to the input node.
        /// </summary>
        private void ExpandToFolder(BindingList<TreeViewItem> treeViewItems, string node)
        {
            if (false == Directory.Exists(node))
            {
                return;
            }

            foreach (TreeViewItem item in treeViewItems)
            {
                if (true == node.StartsWith(item.Tag.ToString()))
                {
                    item.IsExpanded = true;
                    item.IsSelected = true;
                    item.Focus();

                    if (node == item.Tag.ToString())
                    {
                        return;
                    }
                    else
                    {
                        ExpandItem(item.Items, node);
                    }
                }
            }
        }
        #endregion

        #region Members
        private static string m_SelectedPath;
        private static object dummyNode;
        private object m_SelectedItem;
        private bool? m_DialogResult;

        private DelegateCommand<TreeViewHelper.DependencyPropertyEventArgs> m_SelectedItemChangedCommand;
        #endregion

        #region Constants

        private const string PATH_SEPARATOR_CHAR = "\\";

        #endregion
    }
}
