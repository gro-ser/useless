using System;
using System.Collections.Generic;
using System.Text;

namespace LinuxFSEmulator
{
    class FSGroup
    {
        public string Name { get; private set; }
        public override string ToString() => Name;

        public FSGroup(string name) => Name = name;
    }
}