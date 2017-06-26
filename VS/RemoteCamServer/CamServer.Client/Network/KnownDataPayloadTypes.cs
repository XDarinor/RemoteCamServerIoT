using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Network
{
    public enum KnownDataPayloadTypes
       : byte
    {
        NotSet = 0x00,
        JPEGDataPayload = 0x01,
        PNGDataPayload = 0x02
    }
}
