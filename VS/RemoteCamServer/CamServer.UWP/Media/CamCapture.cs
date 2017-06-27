using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using AMDev.CamServer.UWP.Threading;

namespace AMDev.CamServer.UWP.Media
{
    public class CamCapture
    {
        #region Const

        private const int CaptureWaitMillisecs = 60;

        #endregion

        #region Events

        public event EventHandler<MediaCapturedEventArgs> MediaCaptured;

        #endregion

        #region Fields

        private MediaCapture mediaCapture = null;
        private LowLagPhotoCapture lowLagPhotoCapture = null;

        #endregion

        #region Properties

        public bool Capturing
        {
            get;
            protected set;
        }

        public KnownMediaTypes OutputMediaType
        {
            get;
            set;
        }

        #endregion

        #region .ctor

        public CamCapture()
        {
            this.mediaCapture = new MediaCapture();
            this.OutputMediaType = KnownMediaTypes.JPG;
        }

        #endregion

        #region Methods

        public async Task Start()
        {
            if (!this.Capturing)
            {
                await this.mediaCapture.InitializeAsync();
                this.lowLagPhotoCapture = await this.mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));
                if (this.lowLagPhotoCapture != null)
                {
                    this.Capturing = true;
                    Task.Run(this.CaptureTask).RunAndForget();
                }
            }
        }

        public async Task Stop()
        {
            if (this.Capturing)
            {
                try
                {
                    this.Capturing = false;

                    await this.lowLagPhotoCapture.FinishAsync();
                    this.lowLagPhotoCapture = null;
                    this.mediaCapture.Dispose();
                    this.mediaCapture = null;
                }
                catch(Exception)
                {
                }
            }
        }

        private async Task CaptureTask()
        {
            while(this.Capturing)
            {
                CapturedPhoto capturedPhoto = null;
                MediaCapturedEventArgs capturedEventsArgs = null;
                KnownMediaTypes currentMediaType = default(KnownMediaTypes);

                byte[] imageData = null;

                try
                {
                    capturedPhoto = await this.lowLagPhotoCapture.CaptureAsync();
                }
                catch(Exception)
                {
                    capturedPhoto = null;
                }

                if (capturedPhoto != null && capturedPhoto.Frame.SoftwareBitmap != null)
                {
                    currentMediaType = this.OutputMediaType;
                    imageData = await this.SoftwareBitmapToImageBuffer(capturedPhoto.Frame.SoftwareBitmap, currentMediaType);   
                    if (imageData != null)
                    {
                        capturedEventsArgs = new MediaCapturedEventArgs(imageData, currentMediaType);
                        if (this.MediaCaptured != null)
                            Task.Run(() =>
                            {
                                this.MediaCaptured(this, capturedEventsArgs);
                            }).RunAndForget(); 
                    }
                }
                await Task.Delay(CaptureWaitMillisecs);
            }
        }

        private async Task<byte[]> SoftwareBitmapToImageBuffer(SoftwareBitmap bitmap, KnownMediaTypes mediaType)
        {
            byte[] jpgData = null;
            BitmapEncoder bitmapEncoder = null;
            Guid encoderID = Guid.Empty;
            MemoryStream ms = new MemoryStream();
            IRandomAccessStream randomAccessStream = null;            

            switch (mediaType)
            {
                default:
                case KnownMediaTypes.JPG:
                    encoderID = BitmapEncoder.JpegEncoderId;
                    break;

                case KnownMediaTypes.PNG:
                    encoderID = BitmapEncoder.PngEncoderId;
                    break;
            }
            randomAccessStream = ms.AsRandomAccessStream();
            bitmapEncoder = await BitmapEncoder.CreateAsync(encoderID, randomAccessStream);
            bitmapEncoder.SetSoftwareBitmap(bitmap);
            bitmapEncoder.IsThumbnailGenerated = false;

            try
            {
                await bitmapEncoder.FlushAsync();
            }
            catch(Exception )
            {

            }

            jpgData = ms.ToArray();

            try
            {
                bitmapEncoder = null;
                randomAccessStream.Dispose();
                randomAccessStream = null;
                ms.Dispose();
                ms = null;
            }
            catch (Exception)
            {
            }

            return jpgData;
        }

        #endregion      
    }
}
