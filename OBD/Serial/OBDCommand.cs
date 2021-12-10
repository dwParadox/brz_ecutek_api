using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.OBD
{
    public class OBDCommand
    {
        public byte Cmd { get; private set; }
        public byte[] Data { get; private set; }

        public OBDCommand(byte cmd, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.Cmd = cmd;
            this.Data = data;
        }

        public OBDCommand(byte cmd)
        {
            this.Cmd = cmd;
        }

        public byte[] GetBytes()
        {
            if (this.Data == null)
            {
                return new byte[]
                {
                    this.Cmd
                };
            }

            byte[] bytes = new byte[1 + this.Data.Length];

            bytes[0] = this.Cmd;
            Array.Copy(this.Data, 0, bytes, 1, this.Data.Length);

            return bytes;
        }
    }
}
