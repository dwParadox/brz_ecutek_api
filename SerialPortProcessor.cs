using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace EcuDox
{

    public class SerialPortProcessor
    {
        public static bool DebugState = false;

        private MsgChannel msgChannel;
        private string comName;
        private SerialPort inPort;

        public SerialPortMessage LastOBDResponse;
        public SerialPortMessage SerialMessageIn;
        public SerialPortMessage SerialMessageOut;

        public bool IsOpen
        {
            get
            {
                if (inPort == null)
                    return false;

                return inPort.IsOpen;
            }
        }

        public SerialPortProcessor(string comPort)
        {
            msgChannel = MsgChannel.Normal;
            comName = comPort;
        }

        private void SetOBDResponse(SerialPortMessage serialMessage, byte[] rawData)
        {
            LastOBDResponse = new SerialPortMessage();

            LastOBDResponse.Channel = serialMessage.Channel;
            LastOBDResponse.CmdType = serialMessage.CmdType;
            LastOBDResponse.Data = new byte[serialMessage.Data.Length - 1];

            Array.Copy(serialMessage.Data, 1, LastOBDResponse.Data, 0, LastOBDResponse.Data.Length);
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] receivedBytes = new byte[inPort.BytesToRead];
            inPort.BaseStream.Read(receivedBytes, 0, inPort.BytesToRead);

            if (DebugState)
                Console.WriteLine("<- " + $"[{BitConverter.ToString(receivedBytes)}]");

            SerialPortMessage serialMessage = DeserializePortMessage(receivedBytes);

            if (serialMessage.Data[0] != 16)
            {
                SerialMessageIn = serialMessage;

                int serialMessageSize = SerialMessageIn.Data.Length + 10;

                // EXPERIMENTAL
                if (receivedBytes.Length - serialMessageSize > 8) 
                { 
                    // If the byte after end of initial command is the start of a new command
                    if (receivedBytes[serialMessageSize] == 0x7E)
                    {
                        if (DebugState)
                            Console.WriteLine("[OnDataReceived] Detected overflow, scraping OBD response.");

                        byte[] obdResponseData = new byte[receivedBytes.Length - serialMessageSize];
                        Array.Copy(receivedBytes, serialMessageSize, obdResponseData, 0, obdResponseData.Length);

                        SerialPortMessage splitMessage = DeserializePortMessage(obdResponseData);

                        if (DebugState)
                            Console.WriteLine("\t<- " + $"[{BitConverter.ToString(splitMessage.Data)}]");

                        SetOBDResponse(splitMessage, null);
                    }
                }
            }
            else
                SetOBDResponse(serialMessage, null);
        }

        private SerialPortMessage SetupSerialMessage(byte cmdType, byte cmd, MsgChannel channel)
        {
            SerialPortMessage result = new SerialPortMessage();

            result.CmdType = cmdType;
            result.Channel = channel;
            result.Data = new byte[] { cmd };

            return result;
        }

        private ushort GetMsgDataSize(SerialPortMessage msg)
        {
            MsgChannel channel = msg.Channel;

            int dataSize = 0;
            if (channel == MsgChannel.Normal)
            {
                dataSize = msg.Data.Length;
            }
            else if (channel == MsgChannel.Secure)
            {
                int num2 = msg.Data.Length % 16;
                if (num2 != 0)
                    dataSize = msg.Data.Length + (16 - num2);
                else
                    dataSize = msg.Data.Length;
            }

            return (ushort)(1 + (3 + dataSize));
        }
        private void InitializeCommand(byte[] array, ref int dataIndex)
        {
            array[dataIndex] = 126;
            dataIndex++;
        }
        private void SetByteInPacket(byte[] array, ref int dataIndex, byte val)
        {
            if (val == 126)
            {
                array[dataIndex] = 125;

                // 0
                array[dataIndex + 1] = 94;

                // 1
                dataIndex += 2;

                // 2
                return;
            }

            if (val == 125)
            {
                array[dataIndex] = 125;

                // 0
                array[dataIndex + 1] = 93;

                // 1
                dataIndex += 2;

                // 2
                return;
            }

            array[dataIndex] = val;
            dataIndex++;
        }

        private int SetMsgChannel(byte[] array, ref int dataIndex, SerialPortMessage msg, ref byte someByte)
        {
            byte msgChannelByte = (byte)msg.Channel;
            MsgChannel msgChannel;

            // 169
            byte b2 = (byte)(msg.Data.Length & 255);

            // 171
            byte b3 = (byte)(msg.Data.Length >> 8);

            // 177
            SetByteInPacket(array, ref dataIndex, msgChannelByte);                  // i = 6, data = 0x7E 0x00 0x05 0x00 0x00 0x00

            // 176
            someByte += msgChannelByte;                                             // b = 0x05

            // 172
            SetByteInPacket(array, ref dataIndex, b2);                              // i = 7, data = 0x7E 0x00 0x05 0x00 0x00 0x00 0x01

            // 173
            someByte += b2;                                                         // b = 0x06

            // 174
            SetByteInPacket(array, ref dataIndex, b3);                              // i = 8, data = 0x7E 0x00 0x05 0x00 0x00 0x00 0x01 0x00

            // 175
            someByte += b3;                                                         // b = 0x06

            // 170
            msgChannel = msg.Channel;

            // 168          IF (CHANNEL == SECURE) -> FLAG = FALSE : TRUE
            bool isSecured = msgChannel != MsgChannel.Normal;

            int num8 = 0;
            if (isSecured)
            {
                num8 = 0;
            }
            else
            {
                byte[] array3 = msg.Data;

                for (int i = 0; i < array3.Length; i++)
                {
                    byte curByte = array3[i];
                    //      172
                    SetByteInPacket(array, ref dataIndex, curByte);                 // i = 9, data = 0x7E 0x00 0x05 0x00 0x00 0x00 0x01 0x00 0x01

                    //      171
                    someByte += curByte;                                            // b = 0x07
                }

                num8 = msg.Data.Length;
            }

            return 3 + num8;
        }

        private int SetupCmdType(byte[] array, ref int dataIndex, SerialPortMessage msg, ref byte someByte)
        {
            byte msgType = msg.CmdType;
            int msgChannelReturn;

            // 1
            SetByteInPacket(array, ref dataIndex, msgType);                                 // i = 5, data = 0x7E 0x00 0x05 0x00 0x00

            // 2
            someByte += msgType;                                                            // b = 0x05

            // 3
            msgChannelReturn = SetMsgChannel(array, ref dataIndex, msg, ref someByte);      // 

            // 0
            if (msgChannelReturn != 0)
                return 1 + msgChannelReturn;

            return 0;
        }

        // Comment flow assuming:
        //              CmdType = 0
        //              Data = { 0x01 }
        //              Channel = 0
        public SerialPortMessage SendSerialCommand(SerialPortMessage msg, int timeOut)
        {
            SerialMessageIn = null;
            SerialMessageOut = msg;

            byte b = 0;

            // 2
            byte b2 = 0;

            // 6
            ushort msgDataLen = GetMsgDataSize(msg);        // 5
            byte b4 = (byte)(msgDataLen & 255);             // 0x05
            byte b3 = (byte)(msgDataLen >> 8);              // 0x00

            // 11
            byte[] array = new byte[32782];

            // 15
            int dataIndex = 0;

            // 5
            InitializeCommand(array, ref dataIndex);        // i = 1, data = 0x7E

            // 12
            SetByteInPacket(array, ref dataIndex, b2);      // i = 2, data = 0x7E 0x00

            // 14
            b += b2;                                        // b = 0x00

            // 16
            SetByteInPacket(array, ref dataIndex, b4);      // i = 3, data = 0x7E 0x00 0x05

            // 13
            b += b4;                                        // b = 0x05

            // 7
            SetByteInPacket(array, ref dataIndex, b3);      // i = 4, data = 0x7E 0x00 0x05 0x00

            // 4
            b += b3;                                        // b = 0x05

            // 8
            SetupCmdType(array, ref dataIndex, msg, ref b); // i = 9, data = 0x7E 0x00 0x05 0x00 0x00 0x00 0x01 0x00 0x01
                                                            // b = 0x07
                                                            // 10
            SetByteInPacket(array, ref dataIndex, b);       // i = 10, data = 0x7E 0x00 0x05 0x00 0x00 0x00 0x01 0x00 0x01 0x07

            // 3                                                              
            SetByteInPacket(array, ref dataIndex, 0);       // i = 11, data = 0x7E 0x00 0x05 0x00 0x00 0x00 0x01 0x00 0x01 0x07 0x00

            // 0

            // 9
            if (DebugState)
                Console.WriteLine("-> [" + BitConverter.ToString(array.Take(dataIndex).ToArray()) + "]");

            inPort.BaseStream.Write(array, 0, dataIndex);

            DateTime startTime = DateTime.Now;
            while (SerialMessageIn == null)
            {
                if ((DateTime.Now - startTime).TotalMilliseconds > timeOut)
                    return null;

                Thread.Sleep(100);
            }

            return SerialMessageIn;
        }

        public SerialPortMessage SendCommand(byte cmdType, byte cmdData, int timeOut)
        {
            if (DebugState)
                Console.WriteLine($"cmdData ({cmdData}):");

            SerialPortMessage msgToSend = SetupSerialMessage(cmdType, cmdData, msgChannel);
            SerialPortMessage msgResponse = SendSerialCommand(msgToSend, timeOut);

            if (DebugState)
            {
                if (msgResponse == null)
                    Console.WriteLine("\t[ NO RESPONSE ]");

                Console.WriteLine();
            }

            return msgResponse;
        }

        public void OpenComPort()
        {
            inPort = new SerialPort(comName, 9600, Parity.None, 8, StopBits.One);

            inPort.DataReceived += OnDataReceived;

            inPort.ReadBufferSize = 1024;
            inPort.WriteBufferSize = 1024;

            inPort.ReadTimeout = -1;
            inPort.WriteTimeout = 1000;

            inPort.Open();

            Thread.Sleep(1000);
        }

        public void CloseComPort()
        {
            inPort.Close();
            inPort = null;
        }

        private SerialPortMessage DeserializePortMessage(byte[] data)
        {
            if (data.Length < 1)
                return null;

            if (data[0] != 126)
                return null;

            SerialPortMessage serialPortMessage = new SerialPortMessage();

            ushort dataSize = (ushort)(data[6] | (data[7] << 8));

            byte cmdType = data[4];
            byte cmdChannel = data[5];

            byte[] cmdData = new byte[dataSize];

            int dataOffset = 8;

            for (int i = 0; i < dataSize; i++)
            {
                byte b = data[dataOffset];

                if (b == 125)
                {
                    if (data[dataOffset + 1] == 94)
                    {
                        cmdData[i] = 126;
                        dataOffset += 2;
                        continue;
                    }
                    else if (data[dataOffset + 1] == 93)
                    {
                        cmdData[i] = 125;
                        dataOffset += 2;
                        continue;
                    }
                }

                cmdData[i] = b;

                dataOffset++;
            }

            serialPortMessage.CmdType = cmdType;
            serialPortMessage.Channel = (MsgChannel)cmdChannel;
            serialPortMessage.Data = cmdData;

            return serialPortMessage;
        }

        private byte[] GetCmdResponse(byte cmdType, byte cmdData, int timeOut)
        {
            SerialPortMessage serialMessage = SendCommand(cmdType, cmdData, timeOut);

            if (serialMessage == null)
                return null;

            if (serialMessage.Data == null)
                return null;

            if (serialMessage.Data.Length == 0)
                return null;

            byte[] msgData = new byte[serialMessage.Data.Length - 1];
            Array.Copy(serialMessage.Data, 1, msgData, 0, msgData.Length);

            return msgData;
        }

        public string StringCommand(byte cmdType, byte cmdData, int timeOut)
        {
            byte[] msgData = GetCmdResponse(cmdType, cmdData, timeOut);

            if (msgData == null)
                return "";

            return Encoding.UTF8.GetString(msgData);
        }

        public bool BoolCommand(byte cmdType, byte cmdData, int timeOut)
        {
            byte[] msgData = GetCmdResponse(cmdType, cmdData, timeOut);

            if (msgData == null)
                return false;

            return msgData[0] != 0x00;
        }

        public sbyte SByteCommand(byte cmdType, byte cmdData, int timeOut)
        {
            byte[] msgData = GetCmdResponse(cmdType, cmdData, timeOut);

            if (msgData == null)
                return 0;

            return (sbyte)msgData[0];
        }

        public uint UIntCommand(byte cmdType, byte cmdData, int timeOut)
        {
            byte[] msgData = GetCmdResponse(cmdType, cmdData, timeOut);

            if (msgData == null)
                return 0;

            if (msgData.Length < 4)
                return 0;

            return (uint)((int)msgData[0] | (int)msgData[1] << 8 | (int)msgData[2] << 16 | (int)msgData[3] << 24);
        }

        public string GetDeviceSerial()
        {
            byte[] msgData = GetCmdResponse(0, 6, 1000);

            if (msgData == null)
                return "ERROR";

            if (msgData.Length != 16)
                return "ERROR";

            return BitConverter.ToString(msgData).Replace("-", "");
        }

        private bool GetObdBool(SerialPortMessage msg, int timeOut)
        {
            SerialPortMessage msgResponse = SendSerialCommand(msg, timeOut);

            if (msgResponse == null || msgResponse.Data == null)
                throw new Exception("No response");

            if (msgResponse.Data.Length != 2)
                throw new Exception("Unexpected response length");

            return msgResponse.Data[1] > 0;
        }

        public bool SendObdSerialMessage(SerialPortMessage msg, int timeOut)
        {
            return GetObdBool(msg, 2500);
        }

        public bool SendObdCommand(byte[] data)
        {
            byte[] cmd = new byte[1 + data.Length];

            cmd[0] = 32;
            Array.Copy(data, 0, cmd, 1, data.Length);

            SerialPortMessage portMsg = new SerialPortMessage
            {
                Channel = MsgChannel.Normal,
                CmdType = 3,
                Data = cmd
            };

            return GetObdBool(portMsg, 2500);
        }
    }
}
