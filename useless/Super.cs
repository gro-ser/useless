using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        public static SafePtr Create(object obj)
        {
            return new SafePtr { Obj = new ReferenceType { Reference = obj } };
        }

        public static SafePtr Create(IntPtr rIntPtr)
        {
            return new SafePtr { Pointer = new IntPtrWrapper { IntPtr = rIntPtr } };
        }

        // (5)
        public IntPtr IntPtr
        {
            get { return Pointer.IntPtr; }
            set { Pointer.IntPtr = value; }
        }

        // (6)
        public Object Object
        {
            get { return Obj.Reference; }
            set { Obj.Reference = value; }
        }

        public void SetPointer(SafePtr another)
        {
            Pointer.IntPtr = another.Pointer.IntPtr;
        }
    }
    abstract class Super
    {
        public string name;
        public abstract string Say();
        static string me = "groser";
        static public readonly Super
            _a = new ChildA() { name = me },
            _b = new ChildB() { name = me },
            _c = new ChildC() { name = me };
        
        unsafe static int GV(IntPtr p) => ((int*)p.ToPointer())[0];
        unsafe static void SV(IntPtr p, int value) => ((int*)p.ToPointer())[0] = value;
        static IntPtr GP(Super super) => SafePtr.Create(super).IntPtr;

        public void ToChildA() => SV(GP(this), GV(GP(_a)));
        public void ToChildB() => SV(GP(this), GV(GP(_b)));
        public void ToChildC() => SV(GP(this), GV(GP(_c)));
    }
    class ChildA:Super
    {
        public override string Say() => $"Hello, {name}!";
    }
    class ChildB : Super
    {
        public override string Say() => $"Привет, {name}!";
    }
    class ChildC : Super
    {
        public override string Say() => $"Amigo, {name}!";
    }
}
