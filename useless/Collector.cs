using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace _
{
    [StructLayout(LayoutKind.Explicit)]
    struct Collector
    {
        [FieldOffset(0)]
        long lng;
        [FieldOffset(0)]
        double dbl;
        [FieldOffset(0)]
        IntPtr ptr;

    }
}
