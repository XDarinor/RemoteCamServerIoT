using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Threading
{
    public static class TaskExtensions
    {
        public static void RunAndForget(this Task task)
        {
            task.ContinueWith(currentTask =>
            {
                if (Debugger.IsAttached)
                    Debug.WriteLine(currentTask.Exception.ToString());
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
