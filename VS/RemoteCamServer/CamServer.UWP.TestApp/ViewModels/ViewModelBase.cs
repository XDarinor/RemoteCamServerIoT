using AMDev.CamServer.UWP.Threading;
using CamServer.UWP.TestApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace CamServer.UWP.TestApp.ViewModels
{
    public abstract class ViewModelBase
        : BindableBase
    {
        public void Dispatch(Action action)
        {
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                action.Invoke();
            }).AsTask().RunAndForget();
        }
    }
}
