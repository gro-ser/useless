//#nullable enable

namespace LinuxFSEmulator
{
    internal abstract class FSObject
    {
        public string Name { get; protected set; }
        public FSObject Parent { get; protected set; }
        public override string ToString() => Parent + Name + (IsDirectory ? "/" : "");


        public virtual bool IsFile => false;
        public virtual bool IsDirectory => false;

    }

    internal class File : FSObject
    {
        public File(string name) => Name = name;
    }
}