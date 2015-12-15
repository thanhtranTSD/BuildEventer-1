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

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;

namespace BuildEventer.Dialog
{
    public class MessageBoxDialog
    {
        #region Constructor
        public MessageBoxDialog()
        {
            m_Settings = new MetroDialogSettings();
            m_Style = MessageDialogStyle.Affirmative;
        }
        #endregion

        #region Public Function
        public void Show(Action<MessageBoxResultButton> callback, string title, string message, MessageBoxButton button)
        {
            DialogSettings(button);
            ShowMetroDialog(callback, title, message, m_Style, m_Settings);
        }
        #endregion

        #region Private Function
        private async void ShowMetroDialog(Action<MessageBoxResultButton> callback, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative,
                                           MetroDialogSettings settings = null)
        {
            MetroWindow metroWindow = (MetroWindow)Application.Current.MainWindow;
            MessageDialogResult dialogResult = await metroWindow.ShowMessageAsync(title, message, style, settings);
            if (null != callback)
            {
                MessageBoxResultButton resultButton = MessageBoxResultButton.OK;
                switch (dialogResult)
                {
                    case MessageDialogResult.Affirmative:
                        {
                            if (m_Style == MessageDialogStyle.Affirmative)
                            {
                                resultButton = MessageBoxResultButton.OK;
                            }
                            else
                            {
                                resultButton = MessageBoxResultButton.Yes;
                            }
                            break;
                        }
                    case MessageDialogResult.Negative:
                        {
                            resultButton = MessageBoxResultButton.No;
                            break;
                        }
                    case MessageDialogResult.FirstAuxiliary:
                        {
                            resultButton = MessageBoxResultButton.Cancel;
                            break;
                        }
                }
                callback(resultButton);
            }
        }

        private void DialogSettings(MessageBoxButton messageBoxButton)
        {
            switch (messageBoxButton)
            {
                case MessageBoxButton.OK:
                    {
                        m_Settings = new MetroDialogSettings
                        {
                            AffirmativeButtonText = "OK",
                        };
                        m_Style = MessageDialogStyle.Affirmative;
                        break;
                    }
                case MessageBoxButton.YesNo:
                    {
                        m_Settings = new MetroDialogSettings
                        {
                            AffirmativeButtonText = "Yes",
                            NegativeButtonText = "No",
                        };
                        m_Style = MessageDialogStyle.AffirmativeAndNegative;
                        break;
                    }
                case MessageBoxButton.YesNoCancel:
                    {
                        m_Settings = new MetroDialogSettings
                        {
                            AffirmativeButtonText = "Yes",
                            NegativeButtonText = "No",
                            FirstAuxiliaryButtonText = "Cancel",
                        };
                        m_Style = MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary;
                        break;
                    }
            }
        }
        #endregion

        #region Members
        private MetroDialogSettings m_Settings;
        private MessageDialogStyle m_Style;
        #endregion
    }

    public enum MessageBoxResultButton
    {
        OK = 0,
        Yes = 1,
        No = 2,
        Cancel = 3,
    }
}
