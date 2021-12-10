using EcuDox.ECM;
using EcuDox.EcuTek.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.OBD.Data.ReadMethods
{
    public abstract class CAN_ReadMethod
    {
        protected OBDCmdManager obdManager;

        public List<LogParam> AllLogParams { get; private set; } = new List<LogParam>();

        public LogParamValue[] LogParamValues { get; protected set; } = null;

        public abstract void AddLogParam(LogParam param);
        public abstract void Init();

        public abstract OBDCommand[] GetParamCommandList();
        public abstract LogParamValue[] ProcessOBDResponses(OBDResponse[] responses);

        public void PullData()
        {
            OBDCommand[] cmdsToSend = GetParamCommandList();
            OBDResponse[] responses = new OBDResponse[cmdsToSend.Length];

            for (int i = 0; i < cmdsToSend.Length; i++)
            {
                if (cmdsToSend[i] == null)
                    throw new InvalidOperationException("An OBD command was null.");

                responses[i] = obdManager.Connection.ReturnResponseFromCmd(cmdsToSend[i]);
            }

            LogParamValues = ProcessOBDResponses(responses);

            //for (int i = 0; i < logParamValues.Length; i++)
            //{
            //    LogParam logParam = AllLogParams[i];
            //    AnalogLogParam analogParam = logParam as AnalogLogParam;
            //    if (analogParam != null)
            //        Console.WriteLine($"{analogParam.Name}: {logParamValues[i].RawValue.ToString() + analogParam.Scaling.UnitName}");
            //}
        }

        protected LogParamValue GetParamValue(int paramIndex, byte[] data, int offset, Endian endian = Endian.Big)
        {
            LogParamValue paramValue = new LogParamValue();

            LogParam logParam = AllLogParams[paramIndex];
            AnalogLogParam analogParam = logParam as AnalogLogParam;

            if (analogParam == null)
            {
                if (SerialPortProcessor.DebugState)
                    Console.WriteLine($"logParam \"{logParam.Name}\" was digital");

                paramValue.RawValue = 0;

                return paramValue;
            }

            if (offset < data.Length)
                paramValue.RawValue = analogParam.ScaleFromRawValue(data, offset, endian);


            return paramValue;
        }

        public CAN_ReadMethod(OBDCmdManager manager)
        {
            obdManager = manager;
        }
    }
}
