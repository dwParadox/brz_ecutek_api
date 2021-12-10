using System;
using System.Collections.Generic;
using System.Text;
using EcuDox.EcuTek.Logging;

namespace EcuDox.OBD.Data.ReadMethods
{
    public class CAN_SubaruGeneric : CAN_ReadMethod
    {
        public CAN_SubaruGeneric(OBDCmdManager manager)
            : base(manager)
        {

        }

        public static uint ReadMethod => 0x00004201;

        private int[] CIDIndices;
        private int[] CIDSizeOffsets;
        private List<ushort> CIDs = new List<ushort>();

        public override void AddLogParam(LogParam param)
        {
            if (param == null)
                return;

            AllLogParams.Add(param);
        }

        public override void Init()
        {
            CIDIndices = new int[AllLogParams.Count];
            CIDSizeOffsets = new int[AllLogParams.Count];

            List<ushort> CIDList = new List<ushort>();

            int num5 = 0, num6 = 0;

            int currentParam = AllLogParams.Count;

            while (currentParam > 0)
            {
                int numParamsToCID = Math.Min(currentParam, 10);
                ushort availableCID = GetAvailableCID();

                if (availableCID == 0)
                    throw new Exception("Exhausted available CIDs");

                OBDCommand registerCidCmd = CmdRegisterParamCID(availableCID, 0, numParamsToCID, 0);

                num5++;
                num6 += numParamsToCID;

                CIDs.Add(availableCID);

                obdManager.Connection.ReturnResponseFromCmd(registerCidCmd);

                currentParam -= numParamsToCID;
            }
        }

        private OBDCommand CmdRegisterParamCID(ushort paramCID, int aya, int ayb, int ayc)
        {
            List<byte> list = new List<byte>(256);

            int i = aya;
            int num5 = aya + ayb - 1;
            int num6 = 2;

            list.AddRange(GetCIDBytes(paramCID));

            while (i <= num5)
            {
                LogParam logParam = AllLogParams[i];

                list.AddRange(GetParamBytes(logParam));

                CIDIndices[i] = ayc;
                CIDSizeOffsets[i] = num6;

                num6 += logParam.DataSize;
                
                i++;
            }

            return new OBDCommand(44, list.ToArray());
        }

        private byte[] GetParamBytes(LogParam logParam)
        {
            ushort num = (ushort)(logParam.ReadAddress & 65535U);

            byte b = (byte)((logParam.ReadAddress & 16711680U) >> 16);
            byte b2 = (byte)logParam.DataSize;

            b -= 2;

            return new byte[]
            {
                (byte)(num >> 8),
                (byte)num,
                b,
                b2
            };
        }

        private byte[] GetCIDBytes(ushort cid)
        {
            return new byte[]
            {
                1,
                (byte)(cid >> 8),
                (byte)cid
            };
        }

        private ushort GetAvailableCID()
        {
            ushort num = 62208;

            while (num < 62227)
            {
                if (!CIDs.Contains(num))
                    return num;

                num += 1;
            }

            return 0;
        }

        public override OBDCommand[] GetParamCommandList()
        {
            OBDCommand[] commands = new OBDCommand[this.CIDs.Count];

            for (int i = 0; i < CIDs.Count; i++)
            {
                ushort curCID = CIDs[i];
                commands[i] = new OBDCommand(34, new byte[]
                {
                    (byte)(curCID >> 8),
                    (byte)curCID
                });
            }

            return commands;
        }

        public override LogParamValue[] ProcessOBDResponses(OBDResponse[] responses)
        {
            List<LogParamValue> paramValues = new List<LogParamValue>();
            int responseCount = responses.Length;
            
            for (int i = 0; i < responseCount; i++)
            {
                OBDResponse response = responses[i];
                int dataOffset = CIDSizeOffsets[i];

                paramValues.Add(GetParamValue(i, response.Data, dataOffset));
            }

            return paramValues.ToArray();
        }
    }
}
