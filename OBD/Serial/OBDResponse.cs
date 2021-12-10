using System;

namespace EcuDox.OBD
{

    public class OBDResponse
    {
        public byte Cmd { get; private set; }
        public byte[] Data { get; private set; }

        public OBDResponse(byte[] rawData)
        {
            if (rawData == null || rawData.Length == 0)
                throw new ArgumentNullException("rawData");

            byte cmd = rawData[0];

            if (cmd != 127 && cmd < 64)
                throw new ArgumentException("Invalid Service byte in raw data", "rawData");

            this.Cmd = (byte)(cmd - 64);
            this.Data = new byte[rawData.Length - 1];
            Array.Copy(rawData, 1, this.Data, 0, this.Data.Length);
        }
    }
}
