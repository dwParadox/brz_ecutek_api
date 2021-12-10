using EcuDox.ECM;
using EcuDox.EcuTek.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.OBD.Data.ReadMethods
{
    public struct ReadMethodData
    {
        public LogParam param;
        public LogParamValue value;
    }

    public class ReadMethodProcessor
    {
        private OBDCmdManager obdManager;

        private Dictionary<uint, CAN_ReadMethod> _readMethods = new Dictionary<uint, CAN_ReadMethod>();

        public ReadMethodProcessor(OBDCmdManager manager)
        {
            this.obdManager = manager;

            CAN_SubaruBRZ can_SubaruBRZ = new CAN_SubaruBRZ(manager);
            CAN_SubaruGeneric can_SubaruGeneric = new CAN_SubaruGeneric(manager);
            CAN_RaceROM can_SubaruRaceROM = new CAN_RaceROM(manager);

            //_readMethods.Add(CAN_SubaruBRZ.ReadMethod, can_SubaruBRZ);
            //_readMethods.Add(CAN_SubaruGeneric.ReadMethod, can_SubaruGeneric);
            _readMethods.Add(CAN_RaceROM.ReadMethod, can_SubaruRaceROM);
        }

        public void QueryData()
        {
            foreach (var readMethod in _readMethods)
                readMethod.Value.PullData();
        }

        public bool ValidReadMethod(uint readMethod) =>
            _readMethods.ContainsKey(readMethod);

        public void RegisterLogParam(LogParam logParam)
        {
            uint readMethod = 0x4202;//Convert.ToUInt32(logParam.ReadMethod, 16);

            if (readMethod != 0)
                _readMethods[readMethod].AddLogParam(logParam);
        }

        public void InitParams()
        {
            foreach (var readMethod in _readMethods)
                readMethod.Value.Init();
        }

        public List<ReadMethodData> GetParamValues()
        {
            List<ReadMethodData> methodData = new List<ReadMethodData>();
            
            foreach (var method in _readMethods.Values)
            {
                var values = method.LogParamValues;
                if (values == null)
                    continue;

                //if (values.Length != method.AllLogParams.Count)
                //    continue;

                for (int i = 0; i < values.Length; i++)
                {
                    var param = method.AllLogParams[i];

                    var analog = param as AnalogLogParam;
                    if (analog == null)
                        continue;

                    var value = values[i];

                    double convertedValue = value.RawValue;
                    string unitType = analog.Scaling.UnitName;

                    if (ECMHelper.ConvertUnit(ref unitType))
                        convertedValue = ECMHelper.ConvertUnitMeasurement(convertedValue, unitType);

                    LogParamValue newValue = new LogParamValue();
                    newValue.RawValue = Math.Round(convertedValue, analog.Scaling.Decimals);

                    methodData.Add(new ReadMethodData { param = analog, value = newValue });
                }
            }

            return methodData;
        }
    }
}
