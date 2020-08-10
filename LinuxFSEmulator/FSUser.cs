//#nullable enable
using System.Collections.Generic;

namespace LinuxFSEmulator
{
    internal class FSUser
    {
        public string Name { get; private set; }
        public override string ToString() => Name;

        private readonly List<FSGroup> groups = new List<FSGroup>();
        public IEnumerable<FSGroup> Groups => groups.AsReadOnly();

        public void AddToGroup(FSGroup group) => groups.Add(group);
        public void RemoveFromGroup(FSGroup group) => groups.Remove(group);

        public FSUser(string name) => Name = name;
    }
}