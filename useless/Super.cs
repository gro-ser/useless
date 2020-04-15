using System;
using System.Runtime.InteropServices;

namespace useless
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SafePtr
    {
        // (1)
        public class ReferenceType
        {
            public object Reference;
        }

        // (2)
        public class IntPtrWrapper
        {
            public IntPtr IntPtr;
        }

        // (3)
        [FieldOffset(0)]
        private ReferenceType Obj;

        // (4)
        [FieldOffset(0)]
        private IntPtrWrapper Pointer;

        public static SafePtr Create(object obj) => new SafePtr { Obj = new ReferenceType { Reference = obj } };

        public static SafePtr Create(IntPtr rIntPtr) => new SafePtr { Pointer = new IntPtrWrapper { IntPtr = rIntPtr } };

        // (5)
        public IntPtr IntPtr
        {
            get => Pointer.IntPtr;
            set => Pointer.IntPtr = value;
        }

        // (6)
        public object Object
        {
            get => Obj.Reference;
            set => Obj.Reference = value;
        }

        public void SetPointer(SafePtr another) => Pointer.IntPtr = another.Pointer.IntPtr;
    }

    internal abstract class Super
    {
        public string name;
        public abstract string Say();

        private static readonly string me = "groser";
        public static readonly Super
            _a = new ChildA() { name = me },
            _b = new ChildB() { name = me },
            _c = new ChildC() { name = me };

        private static unsafe int GV(IntPtr p) => ((int*)p.ToPointer())[0];
        private static unsafe void SV(IntPtr p, int value) => ((int*)p.ToPointer())[0] = value;
        private static IntPtr GP(Super super) => SafePtr.Create(super).IntPtr;

        public void ToChildA() => SV(GP(this), GV(GP(_a)));
        public void ToChildB() => SV(GP(this), GV(GP(_b)));
        public void ToChildC() => SV(GP(this), GV(GP(_c)));
    }

    internal class ChildA : Super
    {
        public override string Say() => $"Hello, {name}!";
    }

    internal class ChildB : Super
    {
        public override string Say() => $"Привет, {name}!";
    }

    internal class ChildC : Super
    {
        public override string Say() => $"Amigo, {name}!";
    }
}
