using EcuDox.ECM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EcuDox.EcuTek.Logging
{
    // Token: 0x020001DE RID: 478
    public class DigitalLogParam : LogParam
    {
        // Token: 0x17000310 RID: 784
        // (get) Token: 0x06000C2F RID: 3119 RVA: 0x00070232 File Offset: 0x0006E432
        // (set) Token: 0x06000C30 RID: 3120 RVA: 0x0007023A File Offset: 0x0006E43A
        public DigitalState[] States { get; set; }

        // Token: 0x17000311 RID: 785
        // (get) Token: 0x06000C31 RID: 3121 RVA: 0x00070243 File Offset: 0x0006E443
        // (set) Token: 0x06000C32 RID: 3122 RVA: 0x0007024B File Offset: 0x0006E44B
        [XmlIgnore]
        public uint Mask { get; set; }

        // Token: 0x17000312 RID: 786
        // (get) Token: 0x06000C33 RID: 3123 RVA: 0x00070254 File Offset: 0x0006E454
        // (set) Token: 0x06000C34 RID: 3124 RVA: 0x00070261 File Offset: 0x0006E461
        [XmlElement("Mask")]
        public string XmlMask
        {
            get
            {
                return XmlUtils.ToHexString(this.Mask);
            }
            set
            {
                this.Mask = XmlUtils.ParseUInt32(value);
            }
        }

        // Token: 0x17000313 RID: 787
        // (get) Token: 0x06000C35 RID: 3125 RVA: 0x0007026F File Offset: 0x0006E46F
        public override string CsvHeaderString
        {
            get
            {
                return base.Name + " (DigitalParameter)";
            }
        }

        // Token: 0x06000C36 RID: 3126 RVA: 0x00070288 File Offset: 0x0006E488
        public override float ScaleFromRawValue(byte[] buffer, int offset, Endian endian)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            uint auy;
            switch (base.DataType)
            {
                case DataType.UnsignedByte:
                    auy = (uint)buffer[offset];
                    goto IL_90;
                case DataType.UnsignedWord:
                    auy = (uint)ECMHelper.DecodeUShortResult(buffer, endian, offset);
                    goto IL_90;
                case DataType.Float:
                case DataType.SignedByte:
                case DataType.SignedWord:
                case DataType.SignedLong:
                case DataType.ArmFloat16:
                    throw new Exception("28820");
                case DataType.UnsignedLong:
                    auy = ECMHelper.DecodeUShortResult(buffer, endian, offset);
                    goto IL_90;
            }
            throw new Exception("Unsupported DataType");
        IL_90:
            return (float)this.aefwvwwuwmej(auy);
        }

        // Token: 0x06000C37 RID: 3127 RVA: 0x0007032D File Offset: 0x0006E52D
        public override string GetCsvString(float value)
        {
            return this.States[(int)value].Name;
        }

        // Token: 0x06000C38 RID: 3128 RVA: 0x0007032D File Offset: 0x0006E52D
        public override string GetDisplayString(float value)
        {
            return this.States[(int)value].Name;
        }

        // Token: 0x06000C39 RID: 3129 RVA: 0x00070340 File Offset: 0x0006E540
        private int aefwvwwuwmej(uint auy)
        {
            if (this.States.Length < 1)
            {
                throw new Exception("Invalid Digital log param, must have at least 1 state");
            }
            auy &= this.Mask;

            for (int i = 0; i < States.Length; i++)
            {
                if (auy == States[i].Value)
                    return i;
            }

            return 0;
        }

        // Token: 0x06000C3A RID: 3130 RVA: 0x0007052B File Offset: 0x0006E72B
        public override object Clone()
        {
            DigitalLogParam digitalLogParam = (DigitalLogParam)base.MemberwiseClone();
            digitalLogParam.States = (DigitalState[])this.States.Clone();
            return digitalLogParam;
        }
    }
}
