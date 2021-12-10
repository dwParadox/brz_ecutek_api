using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EcuDox
{
    public static class XmlUtils
    {
        public static uint ParseUInt32(string value)
        {
            if (value.StartsWith("0x", StringComparison.InvariantCulture))
            {
                return uint.Parse(value.Substring(2), NumberStyles.HexNumber);
            }
            return uint.Parse(value);
        }

        public static string ToHexString(uint value)
        {
            return string.Format("0x{0:X8}", value);
        }
    }
}
