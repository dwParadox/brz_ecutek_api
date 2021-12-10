using EcuDox.ECM;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.OBD
{
    public class OBDConnection
    {
        private bool isChannelOpen 
        { 
            get 
            {
                if (port == null)
                    return false;

                return port.IsOpen;
            } 
        }

        private SerialPortProcessor port;

        private VehicleISOData isoData;
        private bool isIsoChannelOpen = false;

        public OBDConnection(SerialPortProcessor portProcessor)
        {
            this.port = portProcessor;
            
            isoData = new VehicleISOData
            {
                aspcedfjlacl = false,
                aspcedfjlacm = false,
                aspcedfjlaco = 2016U,
                aspcedfjlacq = 2024U
            };
        }

        
        public void InitOBDConnection()
        {
            byte[] array = new byte[5];
            SerialPortMessage portMsg;

            array[0] = 0;
            ECMHelper.WriteArrayUInt(array, 1, (uint)500000);
            portMsg = new SerialPortMessage
            {
                Channel = MsgChannel.Normal,
                CmdType = 3,
                Data = array
            };

            bool result = port.SendObdSerialMessage(portMsg, 1000);
        }

        private void InitISOPort()
        {
            byte b = 0;
            b |= (byte)(isoData.aspcedfjlacm ? 1 : 0);
            b |= (byte)(isoData.aspcedfjlacn ? 2 : 0);

            byte[] array = new byte[13];

            array[0] = 2;
            array[1] = (byte)(isoData.aspcedfjlacl ? 1 : 0);
            array[2] = b;
            ECMHelper.WriteArrayUInt(array, 3, isoData.aspcedfjlaco);
            array[7] = isoData.aspcedfjlacp;
            ECMHelper.WriteArrayUInt(array, 8, isoData.aspcedfjlacq);
            array[12] = isoData.aspcedfjlacr;

            port.SendSerialCommand(new SerialPortMessage
            {
                Channel = MsgChannel.Normal,
                CmdType = 3,
                Data = array
            }, 1000);
        }

        private void ConnectISO()
        {
            if (!isIsoChannelOpen)
            {
                InitISOPort();
                isIsoChannelOpen = true;
            }
        }

        private bool ObdSendResult(byte[] data)
        {
            bool result = false;

            try
            {
                ConnectISO();
                result = port.SendObdCommand(data);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        private OBDReturnResult SerializeCmd(OBDCommand cmd)
        {
            if (!isChannelOpen)
                throw new Exception("OBDConnection is not open.");

            if (!ObdSendResult(cmd.GetBytes()))
                return OBDReturnResult.Timeout;

            return OBDReturnResult.OK;
        }

        private OBDReturnResult ObdResponse(out OBDResponse response, OBDCommand cmd)
        {
            response = null;

            if (port.LastOBDResponse == null)
                return OBDReturnResult.Timeout;

            byte[] recvData = port.LastOBDResponse.Data;

            if (recvData == null)
                return OBDReturnResult.Timeout;

            if (recvData[0] == 127)
            {
                if (SerialPortProcessor.DebugState)
                    Console.WriteLine("[ObdResponse] Received a deferred state signal");

                if (recvData.Length != 3)
                    return OBDReturnResult.ngradglmfgvt;

                OBDReturnResult result = OBDResponseType.tqqhbokahxzf(recvData[2]);

                if (result != OBDReturnResult.ResponsePending)
                    return result;

                return OBDReturnResult.Timeout;
            }

            if (cmd != null)
            {
                if (recvData[0] == cmd.Cmd + 64)
                {
                    response = new OBDResponse(recvData);
                    return OBDReturnResult.OK;
                }
            }
            else
            {
                if (recvData[0] >= 64)
                {
                    response = new OBDResponse(recvData);
                    return OBDReturnResult.OK;
                }
            }

            return OBDReturnResult.Timeout;
        }

        public OBDReturnResult SendObdCommand(OBDCommand cmd, out OBDResponse response)
        {
            port.LastOBDResponse = null;

            OBDReturnResult result = SerializeCmd(cmd);

            if (result != OBDReturnResult.OK)
            {
                response = null;
                return result;
            }
            
            return ObdResponse(out response, cmd);
        }

        public OBDReturnResult tqqhbokahxza(out OBDResponse arm)
        {
            return this.ObdResponse(out arm, null);
        }

        public OBDResponse tqqhbokahxzb()
        {
            OBDResponse result;
            OBDReturnResult obdreturnResult = ObdResponse(out result, null);
            if (obdreturnResult != OBDReturnResult.OK)
                throw new Exception(obdreturnResult.ToString());

            return result;
        }

        public void tqqhbokahxyy(OBDCommand arj)
        {
            OBDReturnResult obdreturnResult = SerializeCmd(arj);

            if (obdreturnResult != OBDReturnResult.OK)
                throw new Exception(obdreturnResult.ToString());
        }

        public OBDResponse ReturnResponseFromCmd(OBDCommand arn)
        {
            this.tqqhbokahxyy(arn);
            return this.tqqhbokahxzb();
        }
    }
}
