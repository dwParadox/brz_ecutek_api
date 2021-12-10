using EcuDox.ECM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EcuDox.EcuTek.Logging
{
    // Token: 0x020001DC RID: 476
    public abstract class LogParam : ICloneable
    {
        public bool LogByDefault { get; set; }

        public bool DebugOnly { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ReadMethod { get; set; }

        public uint ReadAddress { get; set; }

        public DataType DataType { get; set; }

        public ParamOrigin Origin { get; set; }

        public EcuType EcuType { get; set; }

        public int SubaruInitStringByteNum { get; set; } = -1;

        public int SubaruInitStringBitIndex { get; set; } = -1;

        public int DataSize
        {
            get
            {
                return LogParam.rnjjprpujpcb[(int)this.DataType];
            }
        }

        public virtual string CsvHeaderString
        {
            get
            {
                return this.Name;
            }
        }

        public virtual string DisplayUnitString
        {
            get
            {
                return "";
            }
        }

        public virtual float ScaleFromRawValue(byte[] buffer, int offset, Endian endian)
        {
            return 0f;
        }

        public virtual string GetDisplayValue(float value)
        {
            return "";
        }

        public virtual byte[] ScaleToRawValue(float value, Endian endian)
        {
            return new byte[0];
        }

        public virtual string GetCsvString(float value)
        {
            return "-";
        }

        public virtual string GetDisplayString(float value)
        {
            return "-";
        }

        protected static string FloatToString(float value, int decimals)
        {
            return value.ToString(LogParam.rnjjprpujpcc[decimals], CultureInfo.InvariantCulture);
        }

        public abstract object Clone();

        private static int[] rnjjprpujpcb = new int[]
        {
            1,
            2,
            4,
            1,
            2,
            4,
            4,
            3,
            2
        };

        private static string[] rnjjprpujpcc = new string[]
        {
            "F0",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9"
        };
    }
}
