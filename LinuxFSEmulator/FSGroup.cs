namespace LinuxFSEmulator
{
    internal class FSGroup
    {
        public string Name { get; private set; }
        public override string ToString() => Name;

        public FSGroup(string name) => Name = name;
    }
}