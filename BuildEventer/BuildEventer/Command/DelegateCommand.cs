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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace BuildEventer.Command
{
    /// <summary>
    /// This class allows delegating the commanding logic to methods passed as parameters,
    /// and enables a View to bind commands to objects that are not part of the element tree.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        #region Constructors
        public DelegateCommand(Action<object> executeMethod)
            : this(executeMethod, null, false)
        {
        }

        public DelegateCommand(Action<object> executeMethod, Func<bool> canExecuteMethod)
            : this(executeMethod, canExecuteMethod, false)
        {
        }

        public DelegateCommand(Action<object> executeMethod, Func<bool> canExecuteMethod, bool isAutomaticRequeryDisabled)
        {
            if (null != executeMethod)
            {
                m_ExecuteMethod = executeMethod;
                m_CanExecuteMethod = canExecuteMethod;
                m_IsAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        ///     Method to determine if the command can be executed
        /// </summary>
        public bool CanExecute()
        {
            if (null != m_CanExecuteMethod)
            {
                return m_CanExecuteMethod();
            }
            return true;
        }

        /// <summary>
        ///     Execution of the command
        /// </summary>
        public void Execute(object parameter)
        {
            if (null != m_ExecuteMethod)
            {
                m_ExecuteMethod(parameter);
            }
        }

        /// <summary>
        ///     Property to enable or disable CommandManager's automatic requery on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get { return m_IsAutomaticRequeryDisabled; }
            set
            {
                if (value != m_IsAutomaticRequeryDisabled)
                {
                    if (true == value)
                    {
                        CommandManagerHelper.RemoveHandlersFromRequerySuggested(m_CanExecuteChangedHandlers);
                    }
                    else
                    {
                        CommandManagerHelper.AddHandlersToRequerySuggested(m_CanExecuteChangedHandlers);
                    }
                    m_IsAutomaticRequeryDisabled = value;
                }
            }
        }

        /// <summary>
        ///     Raises the CanExecuteChaged event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        ///     Protected virtual method to raise CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            CommandManagerHelper.CallWeakReferenceHandlers(m_CanExecuteChangedHandlers);
        }
        #endregion

        #region ICommand Members
        /// <summary>
        ///     ICommand.CanExecuteChanged implementation
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (false == m_IsAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }
                CommandManagerHelper.AddWeakReferenceHandler(ref m_CanExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (false == m_IsAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }
                CommandManagerHelper.RemoveWeakReferenceHandler(m_CanExecuteChangedHandlers, value);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }
        #endregion

        #region Members
        private readonly Action<object> m_ExecuteMethod = null;
        private readonly Func<bool> m_CanExecuteMethod = null;
        private bool m_IsAutomaticRequeryDisabled = false;
        private List<WeakReference> m_CanExecuteChangedHandlers;
        #endregion
    }

    public class DelegateCommand<T> : ICommand where T : EventArgs
    {
        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute, Func<bool> canExecute)
        {
            if (null != execute)
            {
                this.m_Execute = execute;
                this.m_CanExecute = canExecute;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return (null == m_CanExecute) ? true : m_CanExecute();
        }

        public void Execute(object parameter)
        {
            m_Execute(parameter as T);
        }

        private readonly Action<T> m_Execute;
        private readonly Func<bool> m_CanExecute;
    }

    static class CommandManagerHelper
    {
        public static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (null != handlers)
            {
                // Take a snapshot of the handlers before we call out to them since the handlers
                // could cause the array to me modified while we are reading it.

                EventHandler[] callees = new EventHandler[handlers.Count];
                int count = 0;

                for (int i = handlers.Count - 1; i >= 0; --i)
                {
                    WeakReference reference = handlers[i];
                    EventHandler handler = reference.Target as EventHandler;
                    if (null == handler)
                    {
                        // Clean up old handlers that have been collected
                        handlers.RemoveAt(i);
                    }
                    else
                    {
                        callees[count] = handler;
                        ++count;
                    }
                }

                // Call the handlers that we snapshotted
                for (int i = 0; i < count; ++i)
                {
                    EventHandler handler = callees[i];
                    handler(null, EventArgs.Empty);
                }
            }
        }

        public static void AddHandlersToRequerySuggested(List<WeakReference> handlers)
        {
            if (null != handlers)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;
                    if (null != handler)
                    {
                        CommandManager.RequerySuggested += handler;
                    }
                }
            }
        }

        public static void RemoveHandlersFromRequerySuggested(List<WeakReference> handlers)
        {
            if (null != handlers)
            {
                foreach (WeakReference handlerRef in handlers)
                {
                    EventHandler handler = handlerRef.Target as EventHandler;
                    if (null != handler)
                    {
                        CommandManager.RequerySuggested -= handler;
                    }
                }
            }
        }

        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
        {
            AddWeakReferenceHandler(ref handlers, handler, -1);
        }

        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler, int defaultListSize)
        {
            if (null == handlers)
            {
                handlers = (0 < defaultListSize ? new List<WeakReference>(defaultListSize) : new List<WeakReference>());
            }

            handlers.Add(new WeakReference(handler));
        }

        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (null != handlers)
            {
                for (int i = handlers.Count - 1; i >= 0; --i)
                {
                    WeakReference reference = handlers[i];
                    EventHandler existingHandler = reference.Target as EventHandler;
                    if ((null == existingHandler) || (existingHandler == handler))
                    {
                        // Clean up old handlers that have been collected in addition to the handler that is to be removed.
                        handlers.RemoveAt(i);
                    }
                }
            }
        }
    }
}