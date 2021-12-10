namespace EcuDox
{
    public enum MsgChannel
    {
        Normal,
        Secure
    }

    public class SerialPortMessage
    {
        public byte CmdType { get; set; }
        public byte[] Data { get; set; }
        public MsgChannel Channel { get; set; }
    }
}
