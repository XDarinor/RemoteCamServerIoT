using AMDev.CamServer.Client.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private String host;
        private int port;

        #endregion

        #region Properties

        public bool Connected
        {
            get;
            protected set;
        }

        public string Host
        {
            get
            {                
                return this.host;
            }
            set
            {
                if (this.Connected)
                    throw new InvalidOperationException("The client is connected");
                this.host = value;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                if (this.Connected)
                    throw new InvalidOperationException("The client is connected");
                this.port = value;
            }
        }

        #endregion

        #region .ctor

        public TcpStreamClient()
        {
        }

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
                    Task.Run(new Action(this.ReadLoopTask)).RunAndForget(); 
                }
                catch(SocketException socktExc)
                {
                    if (Debugger.IsAttached)
                        Debug.WriteLine(socktExc.ToString());
                    this.Connected = false;
                }
                catch(Exception exc)
                {
                    if (Debugger.IsAttached)
                        Debug.WriteLine(exc.ToString());
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
                int readResult = 0;

                if (this.tcpClient != null)
                {
                    try
                    {
                        if (this.tcpClient.Available > 0)
                        {
                            buffer = new byte[this.tcpClient.Available];
                            ns = this.tcpClient.GetStream();
                            readResult = ns.Read(buffer, 0, buffer.Length);
                            if (this.DataReceived != null)
                            {
                                eventArgs = new DataReceivedEventArgs(buffer);
                                this.DataReceived.Invoke(this, eventArgs);
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
    }
}
