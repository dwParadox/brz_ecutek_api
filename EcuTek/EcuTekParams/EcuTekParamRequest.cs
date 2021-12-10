using EcuDox.ECM;
using EcuDox.OBD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace EcuDox.EcuTek
{
    public class EcuTekParamRequest
    {
        private OBDConnection obdConnection;
        private OBDCmdManager obdManager;

        public EcuTekParamRequest(OBDCmdManager manager)
        {
            this.obdManager = manager;
            this.obdConnection = manager.Connection;
        }

        public EcuTekParamFile SendParamFileRequest()
        {
            string cacheFileName = "";

            ushort num = 0;
            uint are = 0U;

            int num7 = 2;
            int num9 = num7 + 4;

            MemoryStream memoryStream = new MemoryStream();

            while (true)
            {
                OBDCommand obdCmd = imfslhnukxzs(hqgofhsgkpbz.cirwnepnhxay, qivenswymwob(num, Endian.Big));
                OBDResponse obdResp;
                OBDReturnResult returnResult = obdConnection.SendObdCommand(obdCmd, out obdResp);

                if (returnResult == OBDReturnResult.OK)
                {
                    int count = obdResp.Data.Length - num9;

                    if (num == 0)
                    {
                        are = ECMHelper.DecodeUIntResult(obdResp.Data, Endian.Big, num7);

                        if (Directory.Exists("./AG6_DATA/RaceROM/Cache"))
                        {
                            uint fileId = ECMHelper.DecodeUIntResult(obdResp.Data, Endian.Big, num9);
                            cacheFileName = "./AG6_DATA/RaceROM/Cache/RRData-" + fileId + ".bin";

                            if (File.Exists(cacheFileName))
                                return DecodeReceivedFile(File.ReadAllBytes(cacheFileName), are);
                        }
                        else
                            Directory.CreateDirectory("./AG6_DATA/RaceROM/Cache");
                    }

                    memoryStream.Write(obdResp.Data, num9, count);

                    num++;

                    continue;
                }

                else if (returnResult == OBDReturnResult.RequestOutOfRange)
                    break;

                else throw new Exception($"({returnResult.ToString()}) while reading metadata");
            }

            byte[] raceRomMetadata = memoryStream.ToArray();

            if (!string.IsNullOrWhiteSpace(cacheFileName))
                File.WriteAllBytes(cacheFileName, raceRomMetadata);

            return DecodeReceivedFile(raceRomMetadata, are);
        }

        private EcuTekParamFile DecodeReceivedFile(byte[] ard, uint are)
        {
            MemoryStream memoryStream = new MemoryStream(ard);

            MemoryStream memoryStream2;
            memoryStream2 = new MemoryStream();

            memoryStream.Seek(4, SeekOrigin.Current);

            are += 4;

            byte[] array = new byte[4];
            memoryStream.Read(array, 0, array.Length);

            uint num6 = (uint)((uint)array[0] << 24);
            num6 |= (uint)((uint)array[1] << 16);
            num6 |= (uint)((uint)array[2] << 8);
            num6 |= (uint)array[3];

            uint num7 = (uint)(ard.Length - array.Length);

            byte[] decoderProperties = new byte[5];
            decoderProperties[0] = 93;
            decoderProperties[3] = 1;

            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(decoderProperties);

            decoder.Code(memoryStream, memoryStream2, num7, num6, null);

            return new EcuTekParamFile(memoryStream2.ToArray(), are);
        }

        public static byte[] qivenswymwob(ushort ab, Endian ac)
        {
            byte[] array = new byte[2];
            if (ac != Endian.Little)
            {
                if (ac == Endian.Big)
                {
                    array[0] = (byte)(ab >> 8);
                    array[1] = (byte)ab;
                }
            }
            else
            {
                array[0] = (byte)ab;
                array[1] = (byte)(ab >> 8);
            }
            return array;
        }

        public static OBDCommand imfslhnukxzs(hqgofhsgkpbz apt, byte[] apu)
        {
            byte[] array = new byte[1 + apu.Length];

            array[0] = (byte)apt;
            Array.Copy(apu, 0, array, 1, apu.Length);

            return RaceROM.imfslhnukxyc(SubaruBRZ.CustomLogParamRequest, array);
        }
    }

    public enum hqgofhsgkpbz : byte
    {
        // Token: 0x040006B1 RID: 1713
        cirwnepnhxaw,
        // Token: 0x040006B2 RID: 1714
        cirwnepnhxax,
        // Token: 0x040006B3 RID: 1715
        cirwnepnhxay,
        // Token: 0x040006B4 RID: 1716
        cirwnepnhxaz,
        // Token: 0x040006B5 RID: 1717
        cirwnepnhxba,
        // Token: 0x040006B6 RID: 1718
        cirwnepnhxbb = 16,
        // Token: 0x040006B7 RID: 1719
        cirwnepnhxbc,
        // Token: 0x040006B8 RID: 1720
        cirwnepnhxbd,
        // Token: 0x040006B9 RID: 1721
        cirwnepnhxbe,
        // Token: 0x040006BA RID: 1722
        cirwnepnhxbf,
        // Token: 0x040006BB RID: 1723
        cirwnepnhxbg,
        // Token: 0x040006BC RID: 1724
        cirwnepnhxbh,
        // Token: 0x040006BD RID: 1725
        cirwnepnhxbi,
        // Token: 0x040006BE RID: 1726
        cirwnepnhxbj,
        // Token: 0x040006BF RID: 1727
        cirwnepnhxbk,
        // Token: 0x040006C0 RID: 1728
        cirwnepnhxbl,
        // Token: 0x040006C1 RID: 1729
        cirwnepnhxbm,
        // Token: 0x040006C2 RID: 1730
        cirwnepnhxbn,
        // Token: 0x040006C3 RID: 1731
        cirwnepnhxbo,
        // Token: 0x040006C4 RID: 1732
        cirwnepnhxbp,
        // Token: 0x040006C5 RID: 1733
        cirwnepnhxbq,
        // Token: 0x040006C6 RID: 1734
        cirwnepnhxbr,
        // Token: 0x040006C7 RID: 1735
        cirwnepnhxbs
    }
}
