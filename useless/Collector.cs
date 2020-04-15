using System;
using System.Runtime.InteropServices;

namespace useless
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Collector
    {
        [FieldOffset(0)]
        private readonly long lng;
        [FieldOffset(0)]
        private readonly double dbl;
        [FieldOffset(0)]
        private readonly IntPtr ptr;

    }
}
