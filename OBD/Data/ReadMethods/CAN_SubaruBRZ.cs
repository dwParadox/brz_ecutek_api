using System;
using System.Collections.Generic;
using System.Text;
using EcuDox.ECM;
using EcuDox.EcuTek.Logging;
using EcuDox.Extensions;

namespace EcuDox.OBD.Data.ReadMethods
{
    public class CAN_SubaruBRZ : CAN_ReadMethod
    {
        private Dictionary<byte, List<LogParam>> _paramGroups = new Dictionary<byte, List<LogParam>>();
        private List<byte> _groupIndexer = new List<byte>();

        private EcuHelperData[] helperData = null;
        private bool[] obdCmdForParam;
        private int[] receivedParamOffsets;
        private int[] paramOffsets;
        private int[] paramGroupIds;

        public CAN_SubaruBRZ(OBDCmdManager manager) 
            : base (manager)
        {

        }

        public static uint ReadMethod => 0x00004211;

        public override void AddLogParam(LogParam param)
        {
            byte readIndex = (byte)(param.ReadAddress & 255U);

            if (!_paramGroups.ContainsKey(readIndex))
            {
                _paramGroups.Add(readIndex, new List<LogParam>());
                _groupIndexer.Add(readIndex);
            }

            _paramGroups[readIndex].Add(param);

            AllLogParams.Add(param);
        }

        public override void Init()
        {
            SomeInit1();
            SomeInit2();
        }

        public override OBDCommand[] GetParamCommandList()
        {
            List<OBDCommand> commands = new List<OBDCommand>();

            paramOffsets = new int[AllLogParams.Count];
            paramGroupIds = new int[AllLogParams.Count];
            
            for (int num5 = 0; num5 < AllLogParams.Count; num5++)
            {
                if (obdCmdForParam[num5])
                {
                    int num7;
                    int num8;

                    //170
                    LogParam logParam = AllLogParams[num5];
                    //171
                    byte paramId = (byte)(logParam.ReadAddress & 255U);
                    //175
                    num8 = receivedParamOffsets[num5];
                    //172
                    num7 = commands.FindIndex(i => i.Data[0] == paramId);
                    //173
                    if (num7 != -1)
                    {
                        paramGroupIds[num5] = num7;
                        paramOffsets[num5] = num8;
                        continue;
                    }
                    //174
                    paramGroupIds[num5] = commands.Count;
                    //176
                    commands.Add(new OBDCommand(33, new byte[] { paramId }));
                    //177
                    paramOffsets[num5] = num8;
                }
                else
                    paramGroupIds[num5] = -1;
            }

            /*
            foreach (var cmdGroup in _paramGroups)
                commands.Add(new OBDCommand(33, new byte[] { cmdGroup.Key }));
            */
            
            return commands.ToArray();
        }

        public override LogParamValue[] ProcessOBDResponses(OBDResponse[] responses)
        {
            List<LogParamValue> paramValues = new List<LogParamValue>();
            int dqx = responses.Length;

            for (int num = 0; num < dqx; num++)
            {
                OBDResponse response = responses[num];

                for (int num7 = 0; num7 < AllLogParams.Count; num7++)
                {
                    if (paramGroupIds[num7] == num)
                    {
                        if (SerialPortProcessor.DebugState)
                            Console.WriteLine($"ParamValue({num7}) Group: {paramGroupIds[num7]} DataLen: {response.Data.Length}, Offset: {paramOffsets[num7]}");

                        paramValues.Add(GetParamValue(num7, response.Data, paramOffsets[num7], Endian.Big));
                    }
                }
            }


            /*
            int responseCount = responses.Length;

            int i = 0;
            for (int paramGroup = 0; paramGroup < responseCount; paramGroup++)
            {
                var curGroup = _paramGroups[_groupIndexer[paramGroup]];
                OBDResponse response = responses[paramGroup];

                for (int param = 0; param < curGroup.Count; param++)
                {
                    // COULD FUCK
                    int dataOffset = receivedParamOffsets[i]; //(byte)((AllLogParams[i].ReadAddress & 0xFF0000) >> 16);

                    if (SerialPortProcessor.DebugState)
                        Console.WriteLine($"ParamValue({i}) Group: 0x{((byte)paramGroup).ToString("X")} DataLen: {response.Data.Length}, Offset: {dataOffset}");

                    paramValues.Add(GetParamValue(i, response.Data, dataOffset, Endian.Big));

                    i++;
                }
            }
            */

            return paramValues.ToArray();
        }

        private void PopulateHelperData(OBDResponse ayf, List<EcuHelperData> ayg)
        {
            byte[] someData = ayf.Data;

            int num = 1;

            while (num < someData.Length)
            {
                byte ayh = someData[num];

                //7
                num++;

                //3
                byte b = someData[num];

                //6
                num++;

                //4
                byte[] ayi = someData.tfmuterkgctf(num, (int)b);

                //5
                num += (int)b;

                //8
                ayg.Add(new EcuHelperData(ayh, ayi));
            }        
        }

        private void SomeInit1()
        {
            List<EcuHelperData> list = new List<EcuHelperData>();

            OBDCommand arj = new OBDCommand(168, new byte[] { 1 });
            obdManager.Connection.tqqhbokahxyy(arj);

            OBDResponse ayf;
            while (obdManager.Connection.tqqhbokahxza(out ayf) == OBDReturnResult.OK)
                PopulateHelperData(ayf, list);

            helperData = list.ToArray();
        }

        private void SomeInit2()
        {
            obdCmdForParam = new bool[AllLogParams.Count];
            receivedParamOffsets = new int[AllLogParams.Count];

            EcuHelperData[] array = helperData;
            EcuHelperData curData;

            for (int num6 = 0; num6 < array.Length; num6++)
            {
                curData = array[num6];
                byte b = 1;
                
                for (int num5 = 0; num5 < curData.data.Length; num5++)
                {
                    if (curData.data[num5] != 0)
                    {
                        int num7 = 2 + num5;
                        
                        for (int num8 = 0; num8 < AllLogParams.Count; num8++)
                        {
                            LogParam logParam = AllLogParams[num8];
                            byte b2 = (byte)(logParam.ReadAddress & 255U);
                            byte b3 = (byte)((logParam.ReadAddress & 16711680U) >> 16);

                            if (curData.cmd == b2)
                            {
                                if (num7 == (int)b3)
                                {
                                    obdCmdForParam[num8] = true;
                                    receivedParamOffsets[num8] = (int)b;

                                    if (SerialPortProcessor.DebugState)
                                        Console.WriteLine($"\tCommand For [{AllLogParams[num8].Name}]: true, Offset: {b}");



                                }
                            }
                        }

                        b += 1;
                    }
                }
            }
        }

        private struct EcuHelperData
        {
            public EcuHelperData(byte ayh, byte[] ayi)
            {
                this.cmd = ayh;
                this.data = ayi;
            }

            public byte cmd;
            public byte[] data;
        }
    }
}
