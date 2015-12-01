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
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BuildEventer.UI
{
    public class ExplorerWindowViewModel : ViewModelBase
    {
        #region Constructor

        public ExplorerWindowViewModel()
        { }

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

        public BindingList<TreeViewItem> WorkingDirectory
        {
            get
            {
                return m_WorkingDirectory;
            }
        }

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

        private DelegateCommand<TreeViewHelper.DependencyPropertyEventArgs> m_SelectedItemChangedCommand;

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

        #region LoadedCommand

        public ICommand LoadedCommand
        {
            get
            {
                return m_LoadedCommand ?? (m_LoadedCommand = new DelegateCommand(param => TreeViewLoaded((RoutedEventArgs)param)));
            }
        }
        #endregion LoadedCommand

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

        private void TreeViewLoaded(RoutedEventArgs e)
        {
            m_WorkingDirectory = new BindingList<TreeViewItem>();

            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                item.Expanded += new RoutedEventHandler(FolderExpanded);

                m_WorkingDirectory.Add(item);
            }
            OnPropertyChanged("WorkingDirectory");
        }

        #endregion

        #region Members

        private static string m_SelectedPath;
        private static BindingList<TreeViewItem> m_WorkingDirectory;
        private static object dummyNode;
        private object m_SelectedItem;
        private bool? m_DialogResult;

        private ICommand m_LoadedCommand;

        #endregion

        #region Constants

        private const string PATH_SEPARATOR_CHAR = "\\";

        #endregion
    }
}
