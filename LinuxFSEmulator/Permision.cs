using System;

namespace LinuxFSEmulator
{
    [Flags]
    enum Permision : byte
    {
        Read = 1,
        Write = 2,
        Execute = 4
    }
}