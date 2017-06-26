using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Media
{
    public class MediaCapturedEventArgs
        : EventArgs
    {
        #region Properties

        public KnownMediaTypes MediaType
        {
            get;
            protected set;
        }

        public byte[] Data
        {
            get;
            protected set;
        }

        #endregion

        #region .ctor

        public MediaCapturedEventArgs(byte[] data)
            : this(data, KnownMediaTypes.JPG)
        {

        }

        public MediaCapturedEventArgs(byte[] data, KnownMediaTypes mediaType)
        {
            this.MediaType = mediaType;
            this.Data = data;
        }

        #endregion
    }
}
