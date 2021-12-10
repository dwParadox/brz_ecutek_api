using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.XmlData
{
    public class AnalogScaling : ICloneable
    {
        public string UnitName { get; set; }

        public float Offset { get; set; }

        public float Multiplier { get; set; }

        public bool Invert { get; set; }

        public int Decimals { get; set; }

        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }

    public class XmlAnalogLogParam : XmlLogParam
    {
        public AnalogScaling Scaling { get; set; }
    }
}
