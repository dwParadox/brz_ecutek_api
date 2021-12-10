using System;
using System.Collections.Generic;
using System.Text;
using EcuDox.EcuTek.Logging;

namespace EcuDox.OBD.Data.ReadMethods
{
    public class CAN_RaceROM : CAN_ReadMethod
    {
        public CAN_RaceROM(OBDCmdManager manager)
            : base(manager)
        {

        }
        
        public static uint ReadMethod => 0x00004202;

        private bool[] uukrimmpkhyd;
        private int[] uukrimmpkhya;
        private int[] uukrimmpkhyb;

        public override void AddLogParam(LogParam param)
        {
            if (param == null)
                return;

            AllLogParams.Add(param);
        }

        public override void Init()
        {
            SetupCommandList();
        }

        private void SetupCommandList()
        {
            uukrimmpkhyd = new bool[AllLogParams.Count];

            for (int i = 0; i < AllLogParams.Count; i++)
                uukrimmpkhyd[i] = true;
        }

        public override OBDCommand[] GetParamCommandList()
        {
            uukrimmpkhya = new int[AllLogParams.Count];
            uukrimmpkhyb = new int[AllLogParams.Count];

            List<OBDCommand> list = new List<OBDCommand>();

            for (int num5 = 0; num5 < AllLogParams.Count; num5++) 
            { 
                if (uukrimmpkhyd[num5])
                {
                    LogParam logParam = AllLogParams[num5];

                    ushort num6 = (ushort)(logParam.ReadAddress & 65535U);
                    byte b = (byte)((logParam.ReadAddress & 16711680U) >> 16);

                    int num8;

                    //1
                    b -= 1;
                    //2
                    num8 = GetCmdsUInt(list, num6);
                    //3
                    if (num8 != -1)
                    {
                        uukrimmpkhya[num5] = num8;
                        uukrimmpkhyb[num5] = (int)b;
                        continue;
                    }

                    num8 = list.Count;

                    //0
                    //4
                    list.Add(new OBDCommand(34, new byte[]
                    {
                        (byte)(num6 >> 8),
                        (byte)num6
                    }));

                    uukrimmpkhya[num5] = num8;
                    uukrimmpkhyb[num5] = (int)b;
                }
                else
                {
                    uukrimmpkhya[num5] = -1;
                }
            }

            return list.ToArray();
        }

        private int GetCmdsUInt(List<OBDCommand> axd, ushort axe)
        {
            int num;

            for (num = 0; num < axd.Count; num++)
            {
                if (ECM.ECMHelper.DecodeUShortResult(axd[num].Data, ECM.Endian.Big, 0) == axe)
                    return num;
            }

            return -1;
        }

        public override LogParamValue[] ProcessOBDResponses(OBDResponse[] responses)
        {
            List<LogParamValue> paramValues = new List<LogParamValue>();
            int responseCount = responses.Length;

            for (int num = 0; num < responseCount; num++)
            {
                OBDResponse response = responses[num];

                for (int num7 = 0; num7 < AllLogParams.Count; num7++)
                {
                    if (uukrimmpkhya[num7] == num)
                    {
                        paramValues.Add(GetParamValue(num7, response.Data, uukrimmpkhyb[num7], ECM.Endian.Big));
                    }
                }
            }

            return paramValues.ToArray();
        }
    }
}
