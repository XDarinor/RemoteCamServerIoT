using AMDev.CamServer.UWP.Media;
using AMDev.CamServer.UWP.Network;
using CamServer.UWP.TestApp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace CamServer.UWP.TestApp.ViewModels
{
    public sealed class MainPageViewModel
        : ViewModelBase
    {
        #region Fields

        private CamStreamingServer camServer = null;
        private BitmapSource previewSource = null;

        #endregion

        #region Properties

        public BitmapSource PreviewSource
        {
            get
            {
                return this.previewSource;
            }
            private set
            {
                this.SetProperty(ref this.previewSource, value);
            }
        }

        #endregion

        #region Commands

        private DelegateCommand startCaptureCommand = null;
        

        public DelegateCommand StartCaptureCommand
        {
            get
            {
                if (this.startCaptureCommand == null)
                    this.startCaptureCommand = new DelegateCommand(this.StartCapture);
                return this.startCaptureCommand;
            }
        }

        #endregion

        #region .ctor

        public MainPageViewModel()
        {
            this.camServer = new CamStreamingServer();
            this.camServer.FrameCaptured += CamServer_FrameCaptured;
        }

        #endregion

        #region Methods

        private async Task UpdatePreview(byte[] data)
        {
            BitmapImage bitmapImage = null;
            BitmapSource currentSource = null;
            MemoryStream ms = null;
            IRandomAccessStream randomAccessStream = null;

            ms = new MemoryStream(data);
            bitmapImage = new BitmapImage();
            randomAccessStream = ms.AsRandomAccessStream();
            await bitmapImage.SetSourceAsync(randomAccessStream);
            currentSource = this.PreviewSource;
            this.PreviewSource = bitmapImage;
            try
            {
                randomAccessStream.Dispose();
                ms.Dispose();                
            }
            catch(Exception)
            {

            }
        }

        private void StartCapture(object parameter)
        {
            if (this.camServer != null)
                this.camServer.StartStreaming(StreamingProtocols.Tcp);
        }

        private void StopCapture(object parameter)
        {

        }

        #endregion

        #region Event Handlers

        private void CamServer_FrameCaptured(object sender, MediaCapturedEventArgs e)
        {
            this.Dispatch(() =>
            {
                this.UpdatePreview(e.Data);
            });
        }        

        #endregion
    }
}
