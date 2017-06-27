using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Network
{
    public class CamStreamingClient
    {
        #region Events

        public event EventHandler<FrameReceivedEventArgs> FrameReceived;

        #endregion

        #region Fields

        private INetworkClient networkClient = null;

        #endregion

        #region Properties

        public String Host
        {
            get;
            protected set;
        }

        public int Port
        {
            get;
            protected set;
        }

        public bool Connected
        {
            get
            {
                if (this.networkClient != null)
                    return this.networkClient.Connected;
                else
                    return false;                
            }
        }

        #endregion

        #region .ctor

        public CamStreamingClient(String host, int port)
        {
            if (String.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));

            this.Host = host;
            this.Port = port;
        }

        #endregion

        #region Methods

        public void Connect(StreamingProtocols protocol)
        {
            if (this.networkClient == null)
            {
                switch(protocol)
                {
                    case StreamingProtocols.Tcp:
                    default:
                        this.networkClient = new TcpStreamClient();                        
                        break;

                    case StreamingProtocols.Udp:
                        throw new NotSupportedException("UDP");
                }

                this.networkClient.Host = this.Host;
                this.networkClient.Port = this.Port;
                this.networkClient.DataReceived += NetworkClient_DataReceived;
                this.networkClient.Connect();
            }
        }        

        public void Close()
        {

        }

        #endregion

        #region Event Handlers

        private void NetworkClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            CamDataFrame camDataFrame = null;
            FrameReceivedEventArgs frameReceivedEventArgs = null;

            if (e.Buffer != null)
            {
                try
                {
                    camDataFrame = CamDataFrame.FromByteArray(e.Buffer);
                    if (camDataFrame != null && this.FrameReceived != null)
                    {
                        frameReceivedEventArgs = new FrameReceivedEventArgs(camDataFrame);
                        this.FrameReceived(this, frameReceivedEventArgs);
                    }
                }
                catch(Exception exc)
                {
                    if (Debugger.IsAttached)
                        Debug.WriteLine(exc.ToString());
                }               
            }
        }

        #endregion
    }
}
