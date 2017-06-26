using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CamServer.UWP.TestApp.Common
{
    public class DelegateCommand
       : ICommand
    {
        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Fields

        private Func<object, bool> canExecuteFunction;
        private Action<object> executeAction;
        private bool canExecutePreviousValue;

        #endregion

        #region .ctor

        public DelegateCommand(Action<object> executeAction)
            : this(executeAction, null)
        {
        }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecuteFunction = canExecute;
        }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            try
            {
                bool functionResult = canExecuteFunction(parameter);

                if (this.canExecutePreviousValue != functionResult)
                {
                    this.canExecutePreviousValue = functionResult;

                    if (this.CanExecuteChanged != null)
                        this.CanExecuteChanged(this, new EventArgs());
                }

                return this.canExecutePreviousValue;
            }
            catch (NullReferenceException)
            {
                return true;
            }
            catch (Exception exc)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine("[" + nameof(this.CanExecute) + "] Error occured: " +
                                                       Environment.NewLine + exc.ToString());
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (this.executeAction != null)
                this.executeAction.Invoke(parameter);
            else
            {
                if (Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine("[" + nameof(this.Execute) + "] ExecuteAction is NULL");
            }
        }


        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged(EventArgs.Empty);
        }

        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            try
            {
                if (this.CanExecuteChanged != null)
                    this.CanExecuteChanged(this, e);
            }
            catch (Exception exc)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine("[" + nameof(this.OnCanExecuteChanged) + "] Error occured: " +
                                                       Environment.NewLine + exc.ToString());
            }
        }

        #endregion
    }
}
