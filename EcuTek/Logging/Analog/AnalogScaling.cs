using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.EcuTek.Logging
{
    public class AnalogScaling : ICloneable
    {
        // Token: 0x17000314 RID: 788
        // (get) Token: 0x06000C3C RID: 3132 RVA: 0x0007054E File Offset: 0x0006E74E
        // (set) Token: 0x06000C3D RID: 3133 RVA: 0x00070556 File Offset: 0x0006E756
        public string UnitName { get; set; }

        // Token: 0x17000315 RID: 789
        // (get) Token: 0x06000C3E RID: 3134 RVA: 0x0007055F File Offset: 0x0006E75F
        // (set) Token: 0x06000C3F RID: 3135 RVA: 0x00070567 File Offset: 0x0006E767
        public float Offset { get; set; }

        // Token: 0x17000316 RID: 790
        // (get) Token: 0x06000C40 RID: 3136 RVA: 0x00070570 File Offset: 0x0006E770
        // (set) Token: 0x06000C41 RID: 3137 RVA: 0x00070578 File Offset: 0x0006E778
        public float Multiplier { get; set; }

        // Token: 0x17000317 RID: 791
        // (get) Token: 0x06000C42 RID: 3138 RVA: 0x00070581 File Offset: 0x0006E781
        // (set) Token: 0x06000C43 RID: 3139 RVA: 0x00070589 File Offset: 0x0006E789
        public bool Invert { get; set; }

        // Token: 0x17000318 RID: 792
        // (get) Token: 0x06000C44 RID: 3140 RVA: 0x00070592 File Offset: 0x0006E792
        // (set) Token: 0x06000C45 RID: 3141 RVA: 0x0007059A File Offset: 0x0006E79A
        public int Decimals { get; set; }

        // Token: 0x06000C46 RID: 3142 RVA: 0x000705A3 File Offset: 0x0006E7A3
        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
