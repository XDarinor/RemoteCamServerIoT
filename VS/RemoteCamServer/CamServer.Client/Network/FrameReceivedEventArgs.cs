using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Network
{
    public class FrameReceivedEventArgs
        : EventArgs
    {
        #region Properties

        public CamDataFrame Frame
        {
            get;
            protected set;
        }

        #endregion

        #region .ctor

        public FrameReceivedEventArgs(CamDataFrame frame)
        {
            this.Frame = frame;
        }

        #endregion
    }
}
