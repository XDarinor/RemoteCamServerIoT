using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private MemoryStream bufferStream = null;

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

            this.bufferStream = new MemoryStream();
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
            if (this.Connected && this.networkClient != null)
            {
                this.networkClient.Close();
                this.networkClient = null;
            }
        }

        private CamDataFrame GetFrame()
        {
            int tagLen = CamDataFrame.FrameTegLength;
            int frameLen = 0;
            long currentPosition = 0;
            long cutPosition = 0;
            byte[] tagBuffer = null;
            byte[] frameBuffer = null;
            byte[] buffer = null;
            String tag = null;
            BinaryReader br = null;
            CamDataFrame dataFrame = null;

            if (this.bufferStream.Length > 0)
            {
                this.bufferStream.Seek(0, SeekOrigin.Begin);
                currentPosition = this.bufferStream.Position;
                br = new BinaryReader(this.bufferStream);
                while (currentPosition < this.bufferStream.Length)
                {
                    tagBuffer = new byte[tagLen];
                    this.bufferStream.Read(tagBuffer, 0, tagLen);
                    tag = Encoding.ASCII.GetString(tagBuffer);
                    if (tag == CamDataFrame.FrameTag)
                    {
                        frameLen = br.ReadInt32();
                        this.bufferStream.Seek(currentPosition, SeekOrigin.Begin);
                        frameBuffer =  br.ReadBytes(frameLen);
                        cutPosition = this.bufferStream.Position;
                        dataFrame = CamDataFrame.FromByteArray(frameBuffer);
                        buffer = this.bufferStream.GetBuffer();
                        this.bufferStream.Seek(0, SeekOrigin.Begin);
                        this.bufferStream.Write(buffer, (int) cutPosition, (buffer.Length - (int)cutPosition));
                        break;
                    }
                    currentPosition = this.bufferStream.Position;
                }                
            }

            return dataFrame;
        }

        #endregion

        #region Event Handlers

        private void NetworkClient_DataReceived(object sender, DataReceivedEventArgs e)
        {
            CamDataFrame camDataFrame = null;
            FrameReceivedEventArgs frameReceivedEventArgs = null;

            if (e.Buffer != null)
            {
                this.bufferStream.Seek(0, SeekOrigin.End);
                this.bufferStream.Write(e.Buffer, 0, e.Buffer.Length);

                try
                {
                    camDataFrame = this.GetFrame();
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
