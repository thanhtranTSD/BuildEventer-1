﻿/*
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
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace BuildEventer.UI
{
    /// <summary>
    /// Interaction logic for StartUpDialogue.xaml
    /// </summary>
    public partial class StartUpWindow
    {
        #region Constructor
        public StartUpWindow()
        {
            InitializeComponent();
            this.DataContext = new StartUpDialogueViewModel();
        }
        #endregion

        #region Events
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.System) && (e.SystemKey == Key.F4))
            {
                e.Handled = true;
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        #endregion
    }
}
