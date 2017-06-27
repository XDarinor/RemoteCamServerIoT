using AMDev.CamServer.Client.Network;
using Camserver.Client.TestApplication.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Camserver.Client.TestApplication.ViewModels
{
    public class MainWindowViewModel
        : ViewModelBase
    {
        #region Delegates

        private delegate void SetCurrentImageStreamDelegate(CamDataFrame frame);

        #endregion

        #region Fields

        private String host;
        private int port = 5555;
        private CamStreamingClient camClient = null;
        private BitmapImage currentVideoFrame = null;
        private MemoryStream frameMemoryStream = null;

        #endregion

        #region Properties

        private CamStreamingClient CamClient
        {
            get
            {
                return this.camClient;
            }
            set
            {
                if (this.SetProperty(ref this.camClient, value))
                    this.ConnectCommand.RaiseCanExecuteChanged();
            }
        }

        public String Host
        {
            get
            {
                return this.host;
            }
            set
            {
                if (this.SetProperty(ref this.host, value))
                {
                    this.ConnectCommand.RaiseCanExecuteChanged();
                    this.DisconnectCommand.RaiseCanExecuteChanged();
                }
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
                this.SetProperty(ref this.port, value);
            }
        }

        public BitmapImage CurrentVideoFrame
        {
            get
            {
                return this.currentVideoFrame;
            }
            private set
            {
                this.SetProperty(ref this.currentVideoFrame, value);
            }
        }

        #endregion

        #region Commands

        private DelegateCommand connectCommand = null;
        private DelegateCommand disconnectCommand = null;

        public DelegateCommand ConnectCommand
        {
            get
            {
                if (this.connectCommand == null)
                    this.connectCommand = new DelegateCommand(this.Connect, this.CanConnect);
                return this.connectCommand;
            }
        }

        public DelegateCommand DisconnectCommand
        {
            get
            {
                if (this.disconnectCommand == null)
                    this.disconnectCommand = new DelegateCommand(this.Disconnect, this.CanDisconnect);
                return this.disconnectCommand;
            }
        }

        #endregion

        #region .ctor

        public MainWindowViewModel()
        {
            this.CurrentVideoFrame = new BitmapImage();
            this.frameMemoryStream = new MemoryStream();
        }        

        #endregion

        #region Methods

        public bool CanConnect(object parameter)
        {
            if (!String.IsNullOrEmpty(Host) && this.CamClient == null)
                return true;
            return false;
        }

        public bool CanDisconnect(object parameter)
        {
            if (this.camClient != null)
                return true;
            return false;
        }

        public void Connect(object parameter)
        {
            if (!String.IsNullOrEmpty(this.Host) && this.Port > 0)
            {
                if (this.CamClient == null)
                {
                    this.CamClient = new CamStreamingClient(this.Host, this.Port);
                    this.CamClient.FrameReceived += CamClient_FrameReceived;
                    this.CamClient.Connect(StreamingProtocols.Tcp);
                }
            }
        }

        public void Disconnect(object parameter)
        {
            if (this.camClient != null)
            {
                this.CamClient.Close();
                this.CamClient.FrameReceived -= CamClient_FrameReceived;
                this.CamClient = null;
            }
        }

        private void SetCurrentImageStream(CamDataFrame frame)
        {
            BitmapImage bi = null;
            if (frame != null  && frame.Payload != null && this.CurrentVideoFrame != null)
            {
                if (this.frameMemoryStream != null)
                {   
                    this.frameMemoryStream.SetLength(0);
                    this.frameMemoryStream.Write(frame.Payload, 0, frame.Payload.Length);
                    //this.currentVideoFrame.BeginInit();
                    //this.currentVideoFrame.CacheOption = BitmapCacheOption.OnLoad;
                    //this.currentVideoFrame.StreamSource = this.frameMemoryStream;
                    //this.currentVideoFrame.EndInit();
                    //this.CurrentVideoFrame = this.currentVideoFrame;

                    try
                    {
                        bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = this.frameMemoryStream;
                        bi.EndInit();
                    }
                    catch(Exception exc)
                    {
                        if (Debugger.IsAttached)
                        {
                            // Debugger.Break();
                            Debug.WriteLine(exc.ToString());
                            try
                            {
                                // File.Create("C:\\Temp\\" + Path.GetRandomFileName()).Write(frame.Payload, 0, frame.Payload.Length);
                            }
                            catch(Exception)
                            {

                            }
                        }
                       
                    }
                }
            }
        }

        #endregion

        #region Event Handlers

        private void CamClient_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                               new SetCurrentImageStreamDelegate(this.SetCurrentImageStream),
                                               e.Frame);
        }

        #endregion
    }
}
