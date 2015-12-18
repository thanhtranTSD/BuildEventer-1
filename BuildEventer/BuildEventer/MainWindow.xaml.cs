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

using BuildEventer.Dialog;
using BuildEventer.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Windows;

namespace BuildEventer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
        #endregion

        #region Events
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            MessageBoxDialog messageDialog = new MessageBoxDialog();
            ConfigurationActionViewModel configurationActionViewModel = ((MainWindowViewModel)this.DataContext).ConfigurationActionViewDataContext;
            if (null != configurationActionViewModel.SelectedActionViewModel)
            {
                SettingsViewModelBase selectedActionViewModel = configurationActionViewModel.SelectedActionViewModel;
                if (true == selectedActionViewModel.IsChanged)
                {
                    messageDialog.Show(QuitApplication, "Warning", String.Format("The action {0} are not saved. Do you want to quit application?", selectedActionViewModel.Action.Name), MessageBoxButton.YesNo);
                    return;
                }
            }

            messageDialog.Show(QuitApplication, "Quit application?", "Do you want to quit application?", MessageBoxButton.YesNo);
        }

        private void QuitApplication(MessageBoxResultButton result)
        {
            if (MessageBoxResultButton.Yes == result)
            {
                Application.Current.Shutdown();
            }
        }
        #endregion
    }
}
