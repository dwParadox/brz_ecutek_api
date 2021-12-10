using EcuDox.EcuTek.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EcuDox.XmlData
{
    // Token: 0x0200008A RID: 138
    [XmlType("Ecu")]
    public class XmlEcuReadAddress
    {
        public string CalId { get; set; }

        [XmlIgnore]
        public uint ReadAddress { get; set; }

        [XmlElement("ReadAddress")]
        public string XmlReadAddress
        {
            get
            {
                return XmlUtils.ToHexString(this.ReadAddress);
            }
            set
            {
                this.ReadAddress = XmlUtils.ParseUInt32(value);
            }
        }
    }

    public abstract class XmlLogParam
    {
        public bool LogByDefault { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [XmlIgnore]
        public uint ReadMethod { get; set; }

        [XmlIgnore]
        public uint ReadAddress { get; set; }

        public XmlEcuReadAddress[] ReadAddressesByEcu { get; set; }

        public DataType DataType { get; set; }

        public int SubaruInitStringByteNum { get; set; } = -1;

        public int SubaruInitStringBitIndex { get; set; } = -1;

        [XmlElement("ReadMethod")]
        public string XmlReadMethod
        {
            get
            {
                return XmlUtils.ToHexString(this.ReadMethod);
            }
            set
            {
                this.ReadMethod = XmlUtils.ParseUInt32(value);
            }
        }

        [XmlElement("ReadAddress")]
        public string XmlReadAddress
        {
            get
            {
                return XmlUtils.ToHexString(this.ReadAddress);
            }
            set
            {
                this.ReadAddress = XmlUtils.ParseUInt32(value);
            }
        }
    }
}
