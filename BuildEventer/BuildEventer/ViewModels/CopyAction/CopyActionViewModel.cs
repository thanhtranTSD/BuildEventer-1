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

using BuildEventer.Models;

namespace BuildEventer.ViewModels
{
    public class CopyActionViewModel : SettingsViewModelBase
    {
        #region Constructors
        public CopyActionViewModel()
        {
        }

        public CopyActionViewModel(CopyActionModel copyAction)
        {
            this.Action = copyAction;
            this.CopyAction = copyAction;
            this.ActionSourcesViewModel = new ActionSourcesViewModel(copyAction.Sources);
            this.ActionDestinationsViewModel = new ActionDestinationsViewModel(copyAction.Destinations);
        }
        #endregion

        #region Properties
        public CopyActionModel CopyAction { get; private set; }

        public ActionSourcesViewModel ActionSourcesViewModel { get; private set; }

        public ActionDestinationsViewModel ActionDestinationsViewModel { get; private set; }

        public override IActionModel Action { get; protected set; }

        public override bool CanExecuteAction
        {
            get { return ((false == ActionSourcesViewModel.IsSourcesEmpty) && (false == ActionDestinationsViewModel.IsDestinationsEmpty)); }
        }

        public override bool IsChanged
        {
            get { return ((ActionSourcesViewModel.IsChanged) || (ActionDestinationsViewModel.IsChanged)); }
        }

        public bool HasBackup
        {
            get { return ((ActionSourcesViewModel.HasBackup) || (ActionDestinationsViewModel.HasBackup)); }
        }
        #endregion

        #region Override Functions
        public override void Backup()
        {
            if (false == HasBackup)
            {
                ActionSourcesViewModel.Backup();
                ActionDestinationsViewModel.Backup();
            }
        }

        public override void Restore()
        {
            if (true == HasBackup)
            {
                ActionSourcesViewModel.Restore();
                ActionDestinationsViewModel.Restore();
                ClearBackup();
            }
        }

        public override void Update()
        {
            ClearBackup();
        }
        #endregion

        #region Private Functions
        private void ClearBackup()
        {
            ActionSourcesViewModel.ClearBackup();
            ActionDestinationsViewModel.ClearBackup();
        }
        #endregion
    }
}
