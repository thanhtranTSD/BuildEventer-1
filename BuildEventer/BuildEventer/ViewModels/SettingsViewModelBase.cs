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

using BuildEventer.Models;

namespace BuildEventer.ViewModels
{
    public abstract class SettingsViewModelBase : ViewModelBase
    {
        public abstract IActionModel Action { get; protected set; }

        public abstract bool IsChanged { get; }
        // Get the information that an action has enough parameters for executing or not.
        public abstract bool CanExecuteAction { get; }

        public abstract void Backup();
        public abstract void Restore();
        public abstract void Update();
    }
}
