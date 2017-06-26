using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Network
{
    public class DataReceivedEventArgs
        : EventArgs
    {
        #region Properties

        public byte[] Buffer
        {
            get;
            protected set;
        }

        #endregion

        #region .ctor

        public DataReceivedEventArgs(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        #endregion
    }
}
