using AMDev.CamServer.UWP.Media;
using AMDev.CamServer.UWP.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Network
{
    public class CamStreamingServer
    {
        #region Fields
       
        private INetworkServer networkServer = null;
        private CamCapture camCapture = null;
        private uint seqNumber = 0;

        #endregion

        #region Events

        public event EventHandler<MediaCapturedEventArgs> FrameCaptured;

        #endregion

        #region Properties

        public bool Streaming
        {
            get;
            protected set;
        }

        public String Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        #endregion

        #region .ctor

        public CamStreamingServer()
        {

        }

        #endregion

        #region Methods

        public void StartStreaming(StreamingProtocols protocol = StreamingProtocols.Tcp)
        {
            if (this.networkServer == null)
            {
                switch (protocol)
                {
                    case StreamingProtocols.Tcp:
                    default:
                        this.networkServer = new TcpServer();
                        this.networkServer.Host = this.Host;
                        this.networkServer.Port = this.Port;
                        this.networkServer.Listen();
                        break;

                    case StreamingProtocols.Udp:
                        break;
                }

                if (this.camCapture == null)
                {
                    this.camCapture = new CamCapture();
                    this.camCapture.MediaCaptured += CamCapture_MediaCaptured;
                    this.camCapture.Start().RunAndForget(); 
                }
            }
        }

        public async Task StopStreaming()
        {
            if (this.networkServer != null)
            {
                if (this.camCapture != null)
                {
                    await this.camCapture.Stop();
                    this.camCapture.MediaCaptured -= CamCapture_MediaCaptured;
                    this.camCapture = null;
                }

                this.networkServer.Close();
                this.networkServer = null;
            }
        }

        #endregion

        #region Event Handlers

        private void CamCapture_MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            CamDataFrame dataFrame = null;
            KnownDataPayloadTypes payloadType = default(KnownDataPayloadTypes);
            byte[] frameBuffer = null;

            if (this.networkServer != null)
            {
                switch(e.MediaType)
                {
                    case KnownMediaTypes.JPG:
                        payloadType = KnownDataPayloadTypes.JPEGDataPayload;
                        break;
                    case KnownMediaTypes.PNG:
                        payloadType = KnownDataPayloadTypes.PNGDataPayload;
                        break;
                    default:
                        payloadType = KnownDataPayloadTypes.NotSet;
                        break;
                }
                dataFrame = new CamDataFrame(e.Data, payloadType);
                dataFrame.SequenceCounter = this.seqNumber;
                if (this.seqNumber < uint.MaxValue)
                    this.seqNumber++;
                else
                    this.seqNumber = 0;
                frameBuffer = dataFrame.AsByteArray();
                this.networkServer.Send(frameBuffer);
            }

            if (this.FrameCaptured != null)
                this.FrameCaptured(this, e);
        }

        #endregion
    }
}
