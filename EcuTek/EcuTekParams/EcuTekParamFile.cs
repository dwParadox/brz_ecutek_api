using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.EcuTek
{
    public class EcuTekParamFile
    {
        public EcuTekParamFile(byte[] blobData, uint blobOffset)
        {
            this._blobData = blobData;
            this._blobOffset = blobOffset;
            this._blobSize = this._blobOffset + (uint)this._blobData.Length;
        }

        public uint juygwkjiglbw(uint aqx)
        {
            if (aqx < this._blobOffset || aqx + 4U > this._blobSize)
                throw new Exception("Data outside blob");

            uint num = aqx - this._blobOffset;
            return (uint)((int)this._blobData[(int)num++] << 24 | (int)this._blobData[(int)num++] << 16 | (int)this._blobData[(int)num++] << 8 | (int)this._blobData[(int)num++]);
        }

        public ushort juygwkjiglbx(uint aqy)
        {
            if (aqy < this._blobOffset || aqy + 2U > this._blobSize)
                throw new Exception("Data outside blob");

            uint num = aqy - this._blobOffset;
            return (ushort)((ushort)(this._blobData[(int)num++] << 8) | (ushort)this._blobData[(int)num++]);
        }

        public string juygwkjiglcb(uint arc)
        {
            uint arb = this.juygwkjiglbw(arc);
            return this.juygwkjiglca(arb);
        }

        public string juygwkjiglca(uint arb)
        {
            if (arb >= this._blobOffset)
            {
                if (arb < this._blobSize)
                {
                    uint num = arb - this._blobOffset;
                    uint num2 = num;
                    while (this._blobData[(int)num2] != 0)
                    {
                        num2 += 1U;
                    }
                    uint count = num2 - num;
                    return gjdmxybpjmsp.GetString(this._blobData, (int)num, (int)count);
                }
            }
            throw new Exception("Data outside blob");
        }

        public float juygwkjiglbz(uint ara)
        {
            if (ara < this._blobOffset || ara + 4U > this._blobSize)
            {
                throw new Exception("Data outside blob");
            }
            uint num = ara - this._blobOffset;
            return BitConverter.ToSingle(new byte[]
            {
            this._blobData[(int)(num + 3U)],
            this._blobData[(int)(num + 2U)],
            this._blobData[(int)(num + 1U)],
            this._blobData[(int)num]
            }, 0);
        }

        public byte juygwkjiglby(uint aqz)
        {
            if (aqz < this._blobOffset || aqz + 1U > this._blobSize)
            {
                throw new Exception("Data outside blob");
            }
            uint num = aqz - this._blobOffset;
            return this._blobData[(int)num];
        }

        // Token: 0x0400073D RID: 1853
        private static Encoding gjdmxybpjmsp = Encoding.GetEncoding("iso-8859-1");

        // Token: 0x0400073E RID: 1854
        private byte[] _blobData;

        // Token: 0x0400073F RID: 1855
        private uint _blobOffset;

        // Token: 0x04000740 RID: 1856
        private uint _blobSize;
    }
}
