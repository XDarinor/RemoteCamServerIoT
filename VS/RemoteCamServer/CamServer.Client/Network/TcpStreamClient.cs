using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Network
{
    public class TcpStreamClient
        : INetworkClient
    {
        #region Events

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        #endregion

        #region Fields

        private TcpClient tcpClient = null;

        #endregion

        #region Properties

        public bool Connected
        {
            get;
            protected set;
        }

        public string Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Port { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region Methods

        public void Close()
        {
            if (this.tcpClient != null)
            {
                this.Connected = false;
                this.tcpClient.Close();
                this.tcpClient = null;
            }
        }

        public async Task Connect()
        {            
            if (this.tcpClient == null)
            {
                this.tcpClient = new TcpClient();
                try
                {
                    await this.tcpClient.ConnectAsync(this.Host, this.Port);
                    this.Connected = true;
                    Task.Run(new Action(this.ReadLoopTask));
                }
                catch(SocketException)
                {
                    this.Connected = false;
                }
                catch(Exception)
                {
                    this.Connected = false;
                }
            }
        }
       
        public int Send(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        private void ReadLoopTask()
        {
            while(this.Connected)
            {
                NetworkStream ns = null;
                DataReceivedEventArgs eventArgs = null;
                byte[] buffer = null;

                if (this.tcpClient != null)
                {
                    try
                    {
                        if (this.tcpClient.Available > 0)
                        {
                            buffer = new byte[this.tcpClient.Available];
                            ns = this.tcpClient.GetStream();
                            ns.Read(buffer, 0, buffer.Length);
                            if (this.DataReceived != null)
                            {
                                eventArgs = new DataReceivedEventArgs(buffer);
                                this.DataReceived.BeginInvoke(this, eventArgs, this.DataReceivedAsyncCallback, null);
                            }
                        }
                    }
                    catch(Exception)
                    {

                    }
                }
            }
        }

        #endregion

        #region Events async callbacks

        private void DataReceivedAsyncCallback(IAsyncResult ar)
        {
            if (this.DataReceived != null)
                this.DataReceived.EndInvoke(ar);
        }

        #endregion
    }
}
