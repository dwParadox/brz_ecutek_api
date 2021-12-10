using EcuDox.ECM;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.OBD
{
    public class VehicleISOData
    {
        public bool aspcedfjlacl { get; set; }
        public bool aspcedfjlacm { get; set; }
        public bool aspcedfjlacn { get; set; }
        public uint aspcedfjlaco { get; set; }
        public byte aspcedfjlacp { get; set; }
        public uint aspcedfjlacq { get; set; }
        public byte aspcedfjlacr { get; set; }
    }

    public class OBDCmdManager
    {
        public OBDConnection Connection { get; }
        private SerialPortProcessor port;

        public OBDCmdManager(SerialPortProcessor portProcessor)
        {
            if (portProcessor == null)
                throw new ArgumentNullException("portProcessor");


            this.port = portProcessor;
            this.Connection = new OBDConnection(port);
        }

        public OBDCommand SetupOBDCommand(byte[] data)
        {
            return SetupOBDCommand(data, null);
        }
        
        private OBDCommand SetupOBDCommand(byte[] data, byte[] secondary)
        {
            byte cmd = data[0];
            int dataSize = data.Length - 1;
            int secondarySize = (secondary != null) ? secondary.Length : 0;

            byte[] array = new byte[dataSize + secondarySize];

            Array.Copy(data, 1, array, 0, dataSize);

            if (secondarySize != 0)
                Array.Copy(secondary, 0, array, dataSize, secondarySize);

            return new OBDCommand(cmd, array);
        }

        public void Init() =>
            Connection.InitOBDConnection();
    }
}
