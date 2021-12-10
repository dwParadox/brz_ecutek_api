using EcuDox.EcuTek.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.EcuTek
{
    public class EcuTekParamBuffer
    {
        public LogParam[] ydveguwfxjqi { get; private set; }

        public EcuTekParamBuffer(EcuTekParamFile file, uint aqj)
        {
            List<LogParam> list = new List<LogParam>();

            uint num = aqj;
            uint aql;
            
            while ((aql = file.juygwkjiglbw(num)) != 0U)
            {
                LogParam logParam = this.eocmbeetaqfe(file, aql);
                
                if (!logParam.DebugOnly)
                    list.Add(logParam);

                num += 4U;
            }

            ydveguwfxjqi = list.ToArray();
        }


        private LogParam eocmbeetaqfe(EcuTekParamFile aqk, uint aql)
        {
            ushort num = aqk.juygwkjiglbx(aql);

            if (num == 0)
            {
                AnalogLogParam analogLogParam = new AnalogLogParam();

                // 0
                // decode shit 1
                WriteGenericLogParam(analogLogParam, aqk, aql);

                // 1
                // decode shit 2
                WriteAnalogLogParam(analogLogParam, aqk, aql);

                // 2
                return analogLogParam;
            }

            if (num != 1)
            {
                throw new Exception("Unsupported param type");
            }

            DigitalLogParam digitalLogParam = new DigitalLogParam();

            // 0
            // decode shit 1
            WriteGenericLogParam(digitalLogParam, aqk, aql);

            // 1
            // decode shit 2
            WriteDigitalLogParam(digitalLogParam, aqk, aql);

            // 2
            return digitalLogParam;
        }

        void WriteGenericLogParam(LogParam aqm, EcuTekParamFile aqn, uint aqo)
        {
            ushort num = aqn.juygwkjiglbx(aqo + 2U);
            aqm.Description = aqn.juygwkjiglcb(aqo + 8U);
            aqm.LogByDefault = ((num & 1) > 0);
            aqm.Origin = ParamOrigin.RaceRom;
            aqm.DataType = (DataType)aqn.juygwkjiglbw(aqo + 20U);
            aqm.DebugOnly = ((num & 2) > 0);
            aqm.Name = aqn.juygwkjiglcb(aqo + 4U);
            aqm.ReadAddress = aqn.juygwkjiglbw(aqo + 16U);
            aqm.ReadMethod = aqn.juygwkjiglbw(aqo + 12U).ToString();
        }

        void WriteAnalogLogParam(AnalogLogParam aqp, EcuTekParamFile aqq, uint aqr)
        {
            uint num = aqq.juygwkjiglbw(aqr + 24U);
            AnalogScaling scaling = new AnalogScaling
            {
                UnitName = aqq.juygwkjiglcb(num),
                Offset = aqq.juygwkjiglbz(num + 4U),
                Multiplier = aqq.juygwkjiglbz(num + 8U),
                Invert = (aqq.juygwkjiglby(num + 12U) > 0),
                Decimals = (int)aqq.juygwkjiglby(num + 13U)
            };

            aqp.Scaling = scaling;
        }

        void WriteDigitalLogParam(DigitalLogParam aqs, EcuTekParamFile aqt, uint aqu)
        {
            uint num = aqt.juygwkjiglbw(aqu + 28U);
            List<DigitalState> list = new List<DigitalState>();
            uint num2 = num;

            uint value = aqt.juygwkjiglbw(num2);
            uint num3 = aqt.juygwkjiglbw(num2 + 8U);

            while (num3 != 0)
            {
                list.Add(new DigitalState { Value = value, Name = aqt.juygwkjiglca(num3) });
                
                num2 += 12U;

                value = aqt.juygwkjiglbw(num2);
                num3 = aqt.juygwkjiglbw(num2 + 8U);
            }

            aqs.Mask = aqt.juygwkjiglbw(aqu + 24U);
            aqs.States = list.ToArray();
        }
    }
}
