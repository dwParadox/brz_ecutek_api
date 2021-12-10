using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.ECM
{
    public static class SubaruBRZ
    {
        static SubaruBRZ()
        {

        }

        public static byte[] EcuFlashType { get; } = new byte[]
        {
            34, 63, 63
        };

        public static byte[] EcuFlashVersion { get; } = new byte[]
        {
            34, 63, 62
        };

        public static byte[] SomeDictionary { get; } = new byte[]
        {
            34, 63, 65
        };

        public static byte[] CustomLogParamRequest { get; } = new byte[]
        {
            34, 63, 112
        };
    }
}
