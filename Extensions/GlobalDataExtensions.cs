using System;
using System.Collections.Generic;
using System.Text;

namespace EcuDox.Extensions
{
    public static class GlobalDataExtensions
    {
        public static a[] tfmuterkgctf<a>(this a[] o, int p, int q)
        {
            if (p < 0 || p + q > o.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            a[] array = new a[q];
            Array.Copy(o, p, array, 0, q);
            return array;
        }
    }
}
