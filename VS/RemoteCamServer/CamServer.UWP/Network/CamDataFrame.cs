using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.UWP.Network
{
    public class CamDataFrame
        : IDataFrame
    {
        #region Consts      

        protected const byte VersionBytes = 0x01;
        protected const int CamFrameSize = 22;

        #endregion

        #region Fields

        private byte version = VersionBytes;
        private byte payloadType = (byte)KnownDataPayloadTypes.NotSet;
        private uint sequenceCounter = 0;
        private double timestamp = 0;
        private byte[] payload = null;

        #endregion

        #region Properties

        public byte Version
        {
            get
            {
                return version;
            }
            protected set
            {
                this.version = value;
            }
        }

        public byte PayloadType
        {
            get
            {
                return payloadType;
            }
            set
            {
                this.payloadType = value;
            }
        }

        public uint SequenceCounter
        {
            get
            {
                return this.sequenceCounter;
            }
            protected set
            {
                this.sequenceCounter = value;
            }
        }

        public double Timestamp
        {
            get
            {
                return this.timestamp;
            }
            protected set
            {
                this.timestamp = value;
            }
        }

        public int PayloadLength
        {
            get
            {
                if (this.Payload != null)
                    return this.Payload.Length;
                else
                    return 0;
            }            
        }

        public byte[] Payload
        {
            get
            {
                return this.payload;
            }
            set
            {
                this.payload = value;
            }
        }

        #endregion

        #region .ctor

        public CamDataFrame()
            : this(null, KnownDataPayloadTypes.NotSet)
        {
        }

        public CamDataFrame(byte[] payload, KnownDataPayloadTypes payloadType)
        {
            this.Payload = payload;
            this.PayloadType = PayloadType;
        }

        #endregion

        #region Methods       
        public static CamDataFrame FromByteArray(byte[] buffer)
        {
            MemoryStream ms = null;
            BinaryReader br = null;
            CamDataFrame currentDataFrame = null;
            int payloadLen = 0;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (buffer.Length < CamFrameSize)
                throw new FormatException("Frame size too small");

            ms = new MemoryStream(buffer);
            br = new BinaryReader(ms);

            currentDataFrame = new CamDataFrame();
            currentDataFrame.Version = br.ReadByte();
            currentDataFrame.PayloadType = br.ReadByte();
            currentDataFrame.SequenceCounter = br.ReadUInt32();
            currentDataFrame.Timestamp = br.ReadDouble();
            payloadLen = br.ReadInt32();
            if (payloadLen > 0)
                currentDataFrame.Payload = br.ReadBytes(payloadLen);
            return currentDataFrame;
        }

        public byte[] AsByteArray()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(ms);
            byte[] result = null;

            binaryWriter.Write(this.Version);
            binaryWriter.Write(this.PayloadType);
            binaryWriter.Write(this.SequenceCounter);
            binaryWriter.Write(this.Timestamp);
            binaryWriter.Write(this.PayloadLength);
            binaryWriter.Write(this.Payload);
            binaryWriter.Flush();

            result = ms.ToArray();
            binaryWriter.Dispose();
            ms.Dispose();
            binaryWriter = null;
            ms = null;
            return result;            
        }

        #endregion
    }
}
