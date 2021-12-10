using EcuDox.ECM;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.EcuTek.Logging
{
    public class AnalogLogParam : LogParam
    {
        public AnalogScaling Scaling { get; set; }

        public override string CsvHeaderString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Scaling.UnitName))
                    return string.Format("{0} ({1})", base.Name, this.Scaling.UnitName);

                return base.Name;
            }
        }

        public override string DisplayUnitString
        {
            get
            {
                return this.Scaling.UnitName;
            }
        }

        public override float ScaleFromRawValue(byte[] buffer, int offset, Endian endian)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            float num;
            switch (base.DataType)
            {
                case DataType.UnsignedByte:
                    num = (float)buffer[offset];
                    break;
                case DataType.UnsignedWord:
                    num = (float)ECMHelper.DecodeUShortResult(buffer, endian, offset);
                    break;
                case DataType.Float:
                    num = ECMHelper.DecodeFloatResult(buffer, endian, offset);
                    break;
                case DataType.SignedByte:
                    num = (float)((sbyte)buffer[offset]);
                    break;
                case DataType.SignedWord:
                    num = (float)ECMHelper.DecodeSignedWord(buffer, endian, offset);
                    break;
                case DataType.UnsignedLong:
                    num = ECMHelper.DecodeUIntResult(buffer, endian, offset);
                    break;
                case DataType.SignedLong:
                    num = (float)ECMHelper.DecodeIntResult(buffer, endian, offset);
                    break;
                case DataType.UnsignedTriple:
                    num = (((int)buffer[offset] << 8 | (int)buffer[offset + 1]) << 8 | (int)buffer[offset + 2]);
                    break;
                case DataType.ArmFloat16:
                    num = ECMHelper.DecodeArmFloat16(buffer, endian, offset);
                    break;
                default:
                    throw new Exception("Unsupported DataType");
            }

            if (this.Scaling.Invert)
            {
                if (num != 0f)
                {
                    num = 1f / num;
                }
            }

            return (float)Math.Round((double)(num * this.Scaling.Multiplier + this.Scaling.Offset), this.Scaling.Decimals);
        }

        public override byte[] ScaleToRawValue(float value, Endian endian)
        {
            return new byte[] { 0 };
        }

        public override string GetCsvString(float value)
        {
            return LogParam.FloatToString(value, this.Scaling.Decimals);
        }

        public override string GetDisplayString(float value)
        {
            return this.GetCsvString(value);
        }

        public override object Clone()
        {
            AnalogLogParam analogLogParam = (AnalogLogParam)base.MemberwiseClone();
            analogLogParam.Scaling = (AnalogScaling)this.Scaling.Clone();
            return analogLogParam;
        }
    }
}
