using System;
using System.Reflection;
using System.Reflection.Emit;

class CalliTest
{
    public static void MethodA(int a)
    {
        Console.WriteLine("method a {0}", a);
    }

    public static void MethodB(int b)
    {
        Console.WriteLine("method b {0}", b);
    }

    public static void Main()
    {
        var ma = typeof(CalliTest).GetMethod("MethodA");
        var mb = typeof(CalliTest).GetMethod("MethodB");
        // the dynamic method must have the same parameters
        // as the jumped to method(s)
        var paramTypes = new Type[] { typeof(int) };

        var m = new DynamicMethod(
            "",
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            typeof(void),
            paramTypes,
            // just use the module of one of those methods, it's handy
            // something like typeof(Program).Assembly.ManifestModule works as well
            ma.Module,
            false);

        var il = m.GetILGenerator();

        // code for a calli

        il.Emit(OpCodes.Ldc_I4_2);  // int parameter
        il.Emit(OpCodes.Ldftn, ma); // func pointer
        il.EmitCalli(
            OpCodes.Calli,
            m.CallingConvention,
            typeof(void),
            paramTypes, null);

        // produce a bool to have something to
        // test in the jmp example

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4, 84);
        il.Emit(OpCodes.Clt);

        // if (b) jmp MethodB else jmp MethodA

        var lb = il.DefineLabel();
        il.Emit(OpCodes.Brtrue, lb);
        il.Emit(OpCodes.Jmp, ma);
        il.MarkLabel(lb);
        il.Emit(OpCodes.Jmp, mb);

        // call the dynamic method

        var act = (Action<int>)m.CreateDelegate(typeof(Action<int>));

        act(42);
    }
}