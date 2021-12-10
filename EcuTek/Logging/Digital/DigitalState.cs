using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace EcuDox.EcuTek.Logging
{
    // Token: 0x020001E0 RID: 480
    public class DigitalState
    {
        // Token: 0x17000319 RID: 793
        // (get) Token: 0x06000C48 RID: 3144 RVA: 0x000705AB File Offset: 0x0006E7AB
        // (set) Token: 0x06000C49 RID: 3145 RVA: 0x000705B3 File Offset: 0x0006E7B3
        [XmlIgnore]
        public uint Value { get; set; }

        // Token: 0x1700031A RID: 794
        // (get) Token: 0x06000C4A RID: 3146 RVA: 0x000705BC File Offset: 0x0006E7BC
        // (set) Token: 0x06000C4B RID: 3147 RVA: 0x000705C4 File Offset: 0x0006E7C4
        public string Name { get; set; }

        // Token: 0x1700031B RID: 795
        // (get) Token: 0x06000C4C RID: 3148 RVA: 0x000705CD File Offset: 0x0006E7CD
        // (set) Token: 0x06000C4D RID: 3149 RVA: 0x000705DA File Offset: 0x0006E7DA
        [XmlElement("Value")]
        public string XmlValue
        {
            get
            {
                return XmlUtils.ToHexString(this.Value);
            }
            set
            {
                this.Value = XmlUtils.ParseUInt32(value);
            }
        }
    }
}
