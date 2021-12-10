using EcuDox.ECM;
using EcuDox.EcuTek.Logging;
using EcuDox.OBD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;

namespace EcuDox.EcuTek
{
    public class RaceROM
    {
        private OBDConnection obdConnection;
        private OBDCmdManager obdManager;

        private EcuTekParamRequest paramRequest;
        public LogParam[] RaceROMParams;

        public RaceROM(OBDCmdManager manager)
        {
            this.obdManager = manager;
            this.obdConnection = manager.Connection;

            this.paramRequest = new EcuTekParamRequest(obdManager);
        }

        public bool RaceROMInstalled { get; private set; }
        public uint RaceROMVersion { get; private set; }

        public IReadOnlyDictionary<RaceROMEnum, uint> RaceROMDictionary { get; private set; }

        public byte[] U_RaceROM_ByteArray { get; private set; }

        public void Setup()
        {
            RaceROMInstalled = GetRaceROMInstalled();

            if (!RaceROMInstalled)
                return;

            RaceROMVersion = GetRaceROMVersion();

            RaceROMDictionary = GetRaceROMDictionary();

            if (RaceROMDictionary == null)
                throw new Exception("ECU timed out sending RaceROMDictionary");

            if (SerialPortProcessor.DebugState)
            {
                Console.WriteLine("RaceROM Dictionary:");

                foreach (var kvp in RaceROMDictionary)
                    Console.WriteLine($"{kvp.Key.ToString()}: {kvp.Value}");
            }

            uint ad;
            if (RaceROMDictionary.TryGetValue(RaceROMEnum.swxjatuqutib, out ad))
                U_RaceROM_ByteArray = ECMHelper.GetUIntBytes(ad, Endian.Big);

            if (SerialPortProcessor.DebugState)
                Console.WriteLine("-- CUSTOM PARAMS INIT ----------------------");

            CustomParamInit();

            if (SerialPortProcessor.DebugState)
                Console.WriteLine("--------------------------------------------");

            bool NoCustomParams = true;
            uint aqj;
            if (RaceROMDictionary.TryGetValue(RaceROMEnum.swxjatuquthf, out aqj))
                NoCustomParams = false;

            if (!NoCustomParams)
            {
                EcuTekParamBuffer buffer = new EcuTekParamBuffer(paramRequest.SendParamFileRequest(), aqj);
                RaceROMParams = buffer.ydveguwfxjqi;
            }
        }

        private IReadOnlyDictionary<RaceROMEnum, uint> GetRaceROMDictionary()
        {
            OBDCommand obdCmd = obdManager.SetupOBDCommand(SubaruBRZ.SomeDictionary);
            OBDResponse obdResponse;

            if (obdConnection.SendObdCommand(obdCmd, out obdResponse) != OBDReturnResult.OK) {
                return null;
            }

            return ProcessRaceROMDictionary(ECMHelper.TrimResult(obdResponse.Data, 1));
        }

        private bool GetRaceROMInstalled()
        {
            OBDCommand obdCmd = obdManager.SetupOBDCommand(SubaruBRZ.EcuFlashType);
            OBDResponse obdResponse;

            if (obdConnection.SendObdCommand(obdCmd, out obdResponse) != OBDReturnResult.OK)
                return false;

            byte[] obdReturnData = ECMHelper.TrimResult(obdResponse.Data, 1);
            string obdReturnString = Encoding.ASCII.GetString(obdReturnData);
            
            return obdReturnString == "EcuTek ROM Patch";
        }

        private uint GetRaceROMVersion()
        {
            OBDCommand obdCmd = obdManager.SetupOBDCommand(SubaruBRZ.EcuFlashVersion);
            OBDResponse obdResponse;

            if (obdConnection.SendObdCommand(obdCmd, out obdResponse) != OBDReturnResult.OK)
                return 0U;

            byte[] array = ECMHelper.TrimResult(obdResponse.Data, 1);
            if (array.Length != 4)
                return 0U;

            return ECMHelper.DecodeUIntResult(array, Endian.Big, 0);
        }

        private IReadOnlyDictionary<RaceROMEnum, uint> ProcessRaceROMDictionary(byte[] data)
        {
            if (SerialPortProcessor.DebugState)
                Console.WriteLine("Processing RaceROM dictionary");

            int num = 8;
            if (data.Length % num != 0)
                throw new Exception("Invalid data size");

            int dataSize = data.Length / num;

            Dictionary<RaceROMEnum, uint> dictionary = new Dictionary<RaceROMEnum, uint>();

            for (int i = 0; i < dataSize; i++)
            {
                uint key = ECMHelper.DecodeUIntResult(data, Endian.Big, num * i);
                
                if (key != 0U) 
                {
                    uint value = ECMHelper.DecodeUIntResult(data, Endian.Big, num * i + 4);

                    dictionary.Add((RaceROMEnum)key, value);
                }
            }

            return dictionary;
        }

        public static OBDCommand imfslhnukxyc(byte[] aof, byte[] aog)
        {
            byte aui = aof[0];
            int num = aof.Length - 1;
            int num2 = (aog != null) ? aog.Length : 0;
            byte[] array = new byte[num + num2];
            Array.Copy(aof, 1, array, 0, num);
            if (num2 != 0)
            {
                Array.Copy(aog, 0, array, num, num2);
            }
            return new OBDCommand(aui, array);
        }

        private OBDCommand BigInitOBDCmd(hqgofhsgkpbz ape)
        {
            byte[] aog = new byte[]
            {
                (byte)ape
            };
            return imfslhnukxyc(SubaruBRZ.CustomLogParamRequest, aog);
        }

        private void SendBigInit(byte[] apc)
        {
            OBDCommand arn = EcuTekParamRequest.imfslhnukxzs(hqgofhsgkpbz.cirwnepnhxax, apc);
            obdConnection.ReturnResponseFromCmd(arn);
        }

        private byte[] GetBigInitBytes()
        {
            OBDCommand arn = BigInitOBDCmd(hqgofhsgkpbz.cirwnepnhxaw);
            byte[] array = ECMHelper.TrimResult(obdConnection.ReturnResponseFromCmd(arn).Data, 0);

            if (array.Length != 4)
                throw new InvalidDataException("Invalid seed");

            return array;
        }

        private uint GetBigInitNumber(uint aqe)
        {
            // [FAULT POINT]
            return (uint)BigInteger.ModPow(aqe, 105958177U, 240070843U);
        }

        private void CustomParamInit()
        {
            byte[] apc = ECMHelper.GetUIntBytes(GetBigInitNumber(ECMHelper.DecodeUIntResult(GetBigInitBytes(), Endian.Big, 0)), Endian.Big);
            SendBigInit(apc);
        }
    }
}
