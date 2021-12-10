using EcuDox.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.ECM
{
    public enum Endian
    {
        Little,
        Big
    }

    public static class ECMHelper
    {
        private static Dictionary<string, string> _unitConversions = new Dictionary<string, string>()
        {
            { "bar", "psi" },
            { "°C", "°F" },
            { "km/h", "mph" },
            { "Nm", "ft-lb" }
        };

        public static bool ConvertUnit(ref string unit)
        {
            if (_unitConversions.ContainsKey(unit))
            {
                unit = _unitConversions[unit];
                return true;
            }

            return false;
        }

        public static double ConvertUnitMeasurement(double value, string type)
        {
            double num = 0;

            switch (type)
            {
                case "mph":
                    num = value * 0.6213711922;
                    break;
                case "ft-lb":
                    num = value * 0.7375621493;
                    break;
                case "°F":
                    num = (value * 9 / 5) + 32; 
                    break;
                case "psi":
                    num = value * 14.504;
                    break;
            }

            return num;
        }

        public static void WriteArrayUInt(byte[] array, int index, uint data)
        {
            array[index + 0] = (byte)data;
            array[index + 1] = (byte)(data >> 8);
            array[index + 2] = (byte)(data >> 16);
            array[index + 3] = (byte)(data >> 24);
        }

        public static uint DecodeUIntResult(byte[] input, Endian endian, int cl = 0)
        {
            uint num = 0U;

            if (endian == Endian.Big)
            {
                num = (uint)((uint)input[cl++] << 24);
                num |= (uint)((uint)input[cl++] << 16);
                num |= (uint)((uint)input[cl++] << 8);
                num |= (uint)input[cl++];
            }
            else
            {
                num = (uint)input[cl++];
                num |= (uint)((uint)input[cl++] << 8);
                num |= (uint)((uint)input[cl++] << 16);
                num |= (uint)((uint)input[cl++] << 24);
            }

            return num;
        }

        public static int DecodeIntResult(byte[] input, Endian endian, int cl = 0)
        {
            int num = 0;

            if (endian == Endian.Big)
            {
                num = (int)((int)input[cl++] << 24);
                num |= (int)((int)input[cl++] << 16);
                num |= (int)((int)input[cl++] << 8);
                num |= (int)input[cl++];
            }
            else
            {
                num = (int)input[cl++];
                num |= (int)((int)input[cl++] << 8);
                num |= (int)((int)input[cl++] << 16);
                num |= (int)((int)input[cl++] << 24);
            }

            return num;
        }

        public static byte[] TrimResult(byte[] input, int trimLen)
        {
            byte[] output = new byte[input.Length - trimLen];
            Array.Copy(input, trimLen, output, 0, output.Length);
            return output;
        }

        public static ushort DecodeUShortResult(byte[] cd, Endian ce, int cf = 0)
        {
            ushort num = 0;
            if (ce != Endian.Little)
            {
                if (ce == Endian.Big)
                {
                    num = (ushort)(cd[cf++] << 8);
                    num |= (ushort)cd[cf++];
                }
            }
            else
            {
                num = (ushort)cd[cf++];
                num |= (ushort)(cd[cf++] << 8);
            }
            return num;
        }

        public static float DecodeFloatResult(byte[] cs, Endian ct, int cu = 0)
        {
            byte[] array = cs.tfmuterkgctf(cu, 4);
            if ((BitConverter.IsLittleEndian ? Endian.Little : Endian.Big) != ct)
            {
                Array.Reverse<byte>(array);
            }
            return BitConverter.ToSingle(array, 0);
        }

        public static short DecodeSignedWord(byte[] cm, Endian cn, int co = 0)
        {
            short num = 0;
            if (cn != Endian.Little)
            {
                if (cn == Endian.Big)
                {
                    num = (short)(cm[co++] << 8);
                    num |= (short)cm[co++];
                }
            }
            else
            {
                num = (short)cm[co++];
                num |= (short)(cm[co++] << 8);
            }

            return num;
        }

        public unsafe static float DecodeArmFloat16(byte[] cy, Endian cz, int da = 0)
        {
            ushort num = DecodeUShortResult(cy, cz, da);
            uint num2 = (uint)(num & 32768) >> 15;
            uint num3 = (uint)(num & 31744) >> 10;
            uint num4 = (uint)(num & 1023);
            uint num5 = num2;
            uint num6;
            uint num10;
            uint num11;

            if (num3 == 0U && num4 == 0U)
            {
                num6 = 0U;
                num10 = 0U;
                num11 = (num5 << 31 | num6 << 23 | num10);

                return *(float*)(&num11);
            }

            if (num3 == 0U)
            {
                int num12 = -1;
                uint num13 = num4;
                do
                {
                    num12++;
                    num13 <<= 1;
                }
                while ((num13 & 1024U) == 0U);

                num6 = (uint)(112 - num12);
                num10 = (num13 & 1023U) << 13;
                num11 = (num5 << 31 | num6 << 23 | num10);

                return *(float*)(&num11);
            }

            num6 = num3 - 15U + 127U;
            num10 = num4 << 13;
            num11 = (num5 << 31 | num6 << 23 | num10);

            return *(float*)(&num11);
        }

        public static byte[] GetUIntBytes(uint ad, Endian ae)
        {
            byte[] array = new byte[4];

            if (ae != Endian.Little)
            {
                if (ae == Endian.Big)
                {
                    array[0] = (byte)(ad >> 24);
                    array[1] = (byte)(ad >> 16);
                    array[2] = (byte)(ad >> 8);
                    array[3] = (byte)ad;
                }
            }
            else
            {
                array[0] = (byte)ad;
                array[1] = (byte)(ad >> 8);
                array[2] = (byte)(ad >> 16);
                array[3] = (byte)(ad >> 24);
            }

            return array;
        }
    }
}
