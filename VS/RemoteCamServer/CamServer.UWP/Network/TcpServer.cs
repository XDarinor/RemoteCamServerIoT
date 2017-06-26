using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Network
{
    public class TcpServer
        : INetworkServer
    {
        #region Consts

        public const int DefaultListenPort = 5555;

        #endregion

        #region Fields

        private TcpListener tcpListener = null;
        private string host = null;
        private int port = 0;
        private readonly Dictionary<String, TcpClient> clientDictionary = new Dictionary<String, TcpClient>();
        
        #endregion

        #region Properties

        public bool Listening
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
                if (this.Listening)
                    throw new InvalidOperationException("The server is listening on current host");               
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
                if (this.Listening)
                    throw new InvalidOperationException("The server is listening on current port");
                if (value == 0)
                    throw new ArgumentNullException(nameof(Port));
                this.port = value;
            }
        }

        #endregion

        #region .ctor

        public TcpServer()
            : this(String.Empty, DefaultListenPort)
        {

        }

        public TcpServer(string host, int port)
        {
            this.Host = host;
            this.Port = port;
        }

        #endregion

        public async Task Listen()
        {
            IPEndPoint currentEndPoint = null;
            IPAddress[] resolvedAddresses = null;
            IPAddress currentAddress = null;

            if (!this.Listening)
            {
                if (String.IsNullOrEmpty(this.Host))
                    currentAddress = IPAddress.Any;
                else
                {
                    resolvedAddresses = await Dns.GetHostAddressesAsync(this.Host);
                    if (resolvedAddresses != null)
                    {
                        currentAddress = resolvedAddresses.FirstOrDefault();
                        if (currentAddress == null)
                            throw new Exception("Error resolving address");                     
                    }
                    else
                        throw new Exception("Cannot resolve address");
                }

                currentEndPoint = new IPEndPoint(currentAddress, this.Port);
                this.tcpListener = new TcpListener(currentEndPoint);
                this.tcpListener.Start();                
                this.Listening = true;
                Task.Run(this.AcceptConnectionLooptask);
            }
        }        

        public void Close()
        {
            TcpClient[] clients = null;
            if (this.Listening)
            {
                if (this.tcpListener != null)
                {                    
                    this.tcpListener.Stop();
                    this.tcpListener = null;
                    this.Listening = false;

                    try
                    {
                        if (this.clientDictionary.Count > 0)
                        {
                            clients = this.clientDictionary.Values.ToArray();
                            for (int i = 0; i < clients.Length; i++)                            
                                clients[i].Dispose();
                            clients = null;
                            this.clientDictionary.Clear();
                        }
                    }
                    catch(Exception)
                    {

                    }
                }
            }
        }    

        public int Send(byte[] buffer)
        {
            TcpClient[] clients = null;
            int result = 0;

            if (this.Listening)
            {
                if (this.clientDictionary.Count > 0)
                {
                    clients = this.clientDictionary.Values.ToArray();
                    for (int i = 0; i < clients.Length; i++)
                    {
                        NetworkStream ns = null;
                        try
                        {
                            ns = clients[i].GetStream();
                            ns.Write(buffer, 0, buffer.Length);
                            result += buffer.Length;
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            return result;
        }

        public int Send(String client, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        private async Task AcceptConnectionLooptask()
        {
            while(this.Listening)
            {
                TcpClient currentClient = null;
                IPEndPoint clientEndpoint = null;
                String clientEndpointString = null;

                currentClient = await this.tcpListener.AcceptTcpClientAsync();
                if (currentClient != null)
                {
                    if (currentClient.Client.RemoteEndPoint != null)
                        clientEndpoint = currentClient.Client.RemoteEndPoint as IPEndPoint;
                    else
                        clientEndpoint = currentClient.Client.LocalEndPoint as IPEndPoint;

                    if (clientEndpoint != null)
                    {
                        clientEndpointString = String.Format("{0}@{1}", clientEndpoint.Address.ToString(), 
                                                                        clientEndpoint.Port.ToString());
                        if (this.clientDictionary.ContainsKey(clientEndpointString))
                        {
                            this.clientDictionary[clientEndpointString].Dispose();
                            this.clientDictionary[clientEndpointString] = currentClient;
                        }
                        else                            
                            this.clientDictionary.Add(clientEndpointString, currentClient);
                    }
                }
            }
        }
    }
}
