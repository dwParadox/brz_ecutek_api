using EcuDox.EcuTek;
using EcuDox.EcuTek.Logging;
using EcuDox.OBD;
using EcuDox.OBD.Data.ReadMethods;
using EcuDox.XmlData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace EcuDox
{
    public class AG6API
    {
        private static List<string> _usedParamNames = new List<string>
        {
            "Engine Speed",
            "Manifold Absolute Pressure",
            "Accelerator Angle",
            "Vehicle Speed",
            "AFR Actual",
            "Torque Actual",
            "Throttle Delta",
            "Engine Oil Temperature",
            "Coolant Temperature",
            "Battery Voltage",
            "Fuel Trim Long Term",
            "Fuel Trim Short Term",
            "Fuel Rail Pressure",
            "Mass Air Flow",
            "Fuel Injection PI Ratio",
            "Ignition Timing",
            "Knock Correction",
            "Brake Pressure",
            "VVT Enabled",
            "Engine Load",
            "Injection End of DI to Spark",
            "Wheel Speed Front",
            "Wheel Speed Rear",
            "FlexFuel Ethanol Content - Raw",
            "FlexFuel Ignition Advance",
            "FlexFuel Quantity Multiplier",
            "FlexFuel AFR Adjustment",
            "FlexFuel Cranking Multiplier",
            "Mapswitch Mode"
        };

        public string BootVersion { get; private set; }
        public string FirmwareVersion { get; private set; }
        public string BluetoothVersion { get; private set; }
        public string SerialVersion { get; private set; }

        private bool hasInitialized;

        private ReadMethodProcessor _readMethodProcessor;
        private SerialPortProcessor _port;
        private OBDCmdManager _obdManager;

        public RaceROM RROM { get; private set; }

        public AG6API(SerialPortProcessor portProcessor)
        {
            _port = portProcessor;
            _obdManager = new OBDCmdManager(_port);

            hasInitialized = false;
        }

        private void SetupRaceROM()
        {
            RROM = new RaceROM(_obdManager);
            RROM.Setup();
        }

        private void SetupReadMethodProcessor()
        {
            _readMethodProcessor = new ReadMethodProcessor(_obdManager);

            if (RROM.RaceROMInstalled)
            {
                foreach (var param in RROM.RaceROMParams)
                {
                    if (_usedParamNames.Contains(param.Name))
                       _readMethodProcessor.RegisterLogParam(param);
                }
            }

            _readMethodProcessor.InitParams();
        }

        public void Init()
        {
            SetupData();

            _obdManager.Init();

            SetupRaceROM();

            SetupReadMethodProcessor();

            hasInitialized = true;
        }

        private void SetupData()
        {
            // Get once to clear in buffer
            _port.StringCommand(0, 0, 1000);

            BootVersion = _port.StringCommand(0, 0, 1000);
            FirmwareVersion = _port.StringCommand(0, 1, 1000);
            BluetoothVersion = _port.StringCommand(0, 14, 1000);
            SerialVersion = _port.GetDeviceSerial();
        }

        public string QueryData()
        {
            if (!hasInitialized)
                throw new Exception("Device was not initialized");

            if (_readMethodProcessor == null)
                throw new Exception("ReadMethodProcessor was null");

            Dictionary<string, double> paramData = new Dictionary<string, double>();

            _readMethodProcessor.QueryData();
            List<ReadMethodData> data = _readMethodProcessor.GetParamValues();

            foreach (var dataObj in data)
                paramData.Add(dataObj.param.Name, dataObj.value.RawValue);

            return JsonConvert.SerializeObject(paramData, Formatting.Indented);
        }
    }
}
