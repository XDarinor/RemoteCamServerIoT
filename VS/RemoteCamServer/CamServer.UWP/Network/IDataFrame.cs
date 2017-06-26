using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Network
{
    public interface IDataFrame
    {
        #region Properties

        byte Version
        {
            get;
        }

        byte PayloadType
        {
            get;
        }

        uint SequenceCounter
        {
            get;
        }

        double Timestamp
        {
            get;
        }

        #endregion

        #region Methods

        Byte[] AsByteArray();

        #endregion
    }
}
