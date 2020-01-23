//#nullable enable
using System;

namespace LinuxFSEmulator
{
    class Program
    {
        static unsafe bool getSomeBool() { bool f; *((byte*)&f) = 250; return f; }
        static (int, string) getMax(int a, int b, int c)
        {
            (int, char) maxOp(int x, int y) =>
                x + y > x * y ? (x + y, '+') : (x * y, '*');

            string str;
            int tmp, res; char lop, rop;
            if (a < c)
            {
                (tmp, lop) = maxOp(a, b);
                (res, rop) = maxOp(tmp, c);
                if (lop > rop)
                    str = $"({a} {lop} {b}) {rop} {c}";
                else str = $"{a} {lop} {b} {rop} {c}";
            }
            else
            {
                (tmp, rop) = maxOp(b, c);
                (res, lop) = maxOp(a, tmp);
                if (lop < rop)
                    str = $"{a} {lop} ({b} {rop} {c})";
                else str = $"{a} {lop} {b} {rop} {c}";
            }

            return (res, str);
        }
        static void Main(string[] args)
        {
            var f = getSomeBool();
            Console.WriteLine(f);
            Console.WriteLine(f == true);
            Console.WriteLine(f.Equals(true));

            void print(float x) =>
                Console.WriteLine("x = {0,6} : 0x{1:X}", x,
                BitConverter.DoubleToInt64Bits(x));

            var x = 0f;
            print(x);
            x = MathF.BitDecrement(x);
            print(x);
            x = MathF.BitIncrement(x);
            print(x);
        }
    }
}
