using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
//using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using static System.Linq.Enumerable;
using static System.Math;
using num = System.Numerics;
using big = System.Numerics.BigInteger;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace useless
{
    public static class Program
    {
        public static void Print(string xHeader, __arglist)
        {
            ArgIterator lst = new ArgIterator(__arglist);

            Console.WriteLine(xHeader);
            while (lst.GetRemainingCount() > 0)
            {
                TypedReference r = lst.GetNextArg();
                object o = TypedReference.ToObject(r);
                Console.WriteLine(">{0} -- {1}", o, o.GetType());
            }
            Console.WriteLine();
        }

        public static string ToGothic(string str)
        {
            var sb = new StringBuilder(str); char x;
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                x = sb[i];
                if (x >= '!' && x <= '~')
                    sb[i] = (char)(x + 65248);
            }
            return sb.ToString();
        }

        static long GCD(long a, long b)
        {
            long c = b == 0 ? a : a % b;
            while (c != 0) (a, b, c) = (b, c, b % c);
            return Abs(b);
        }

        static long gcd(long a, long b)
        {
            for (long c = b == 0 ? a : a % b;
                c != 0; a = b, b = c, c = a % b) ;
            return Abs(b);
        }

        static double infSearch(
            Func<double, bool> pred,
            Func<double, double> step,
            double start = 1)
        {
            double value = start; start = double.NaN;
            while (pred(value) && start != value)
            {
                //(value, start) = (step(value), value);
                start = value;
                value = step(value);
            }
            return start;
        }

        static T[] insert<T>(T[] _args, int ind, T value)
        {
            int len = _args.Length;
            T[] args = new T[len + 1];
            Array.Copy(_args, 0, args, 0, ind);
            Array.Copy(_args, ind, args, 1 + ind, len - ind);
            args[ind] = value;
            return args;
        }

        static void testArglist(__arglist)
        {
            const int newint = 10;
            const double newdouble = 0.1;
            var iter = new ArgIterator(__arglist);
            int length = iter.GetRemainingCount();
            for (int i = 0; i < length; i++)
            {
                var type = Type.GetTypeFromHandle(iter.GetNextArgType());
                if (type == typeof(int))
                {
                    int v = __refvalue(iter.GetNextArg(), int);
                    Debug.WriteLine("Found int with value:{0}", v);
                    v = newint;
                    continue;
                }
                if (type == typeof(double))
                {
                    double v = __refvalue(iter.GetNextArg(), double);
                    Debug.WriteLine("Found double with value:{0}", v);
                    v = newdouble;
                    continue;
                }
                Debug.WriteLine("Missing object:{0}", TypedReference.ToObject(iter.GetNextArg()));
            }
        }

        static void printArr<T>(T[] arr, string format = null)
        {
            string tostr()
            {
                int length = arr.Length;
                if (length == 0) return "[]";
                var sb = new StringBuilder("[").AppendFormat(format, arr[0]);
                for (int i = 1; i < length; i++)
                    sb.Append(", ").AppendFormat(format, arr[i]);
                return sb.Append(']').ToString();
            }
            Console.WriteLine(tostr());
        }

        static IFormatProvider provider = CultureInfo.GetCultureInfo("en-en");

        static string Format(this string format, params object[] args)
        {
            return string.Format(provider, format, args: args);
        }

        static string Format(this string format, params (string name, object obj)[] args)
        {
            Regex regex = new Regex(@"^\s*(\w+)\s*$");
            int length = args.Length;
            var objs = new object[length];
            for (int i = 0; i < length; i++)
            {
                var arg = args[i];
                objs[i] = arg.obj;
                var m = regex.Match(arg.name);
                if (!m.Success) throw new ArgumentException(nameof(arg.name));
                var pattern = $@"{{\s*{m.Groups[1].Value}(?=.*}})";
                format = Regex.Replace(format, pattern, "{" + i);
            }
            return string.Format(provider, format, objs);
        }

        static string DynamicFormat(this string format, object obj)
        {
            if (obj is null) return null;
            var list = new Stack<object>();
            var regex = new Regex(@"(?<!{){\s*(\w+).*?(?=})");
            var matches = regex.Matches(format);
            var type = obj.GetType();
            var sb = new StringBuilder(format);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                object value;
                var match = matches[i];
                var name = match.Groups[1].Value;
                if ("this" == name) value = obj;
                else
                {
                    var prop = type.GetProperty(name);
                    value = prop?.GetValue(obj);
                }
                list.Push(value);
                sb.Remove(match.Index + 1, match.Length - 1);
                sb.Insert(match.Index + 1, i);
            }
            return string.Format(sb.ToString(), list.ToArray());
        }

        public static void REWRITE(
            string pathIn,
            string pathOut = "D:\\rand.txt",
            int offset = 0,
            int byteCount = 102_400_000)
        {
            using FileStream read = File.OpenRead(pathIn);
            using FileStream write = File.Open(pathOut, FileMode.OpenOrCreate, FileAccess.Write);
            read.Position = offset * byteCount;
            var bytes = new byte[byteCount];
            read.Read(bytes, 0, byteCount);
            write.Write(bytes, 0, byteCount);
        }

        static int DigCntBad(int num, int basis)
            => Convert.ToString(num, basis).Length;

        static int DigCntFst(int num, int basis)
            => num == 0 ? 1 : (int)(float)Log(num, basis) + 1;

        static int ConvCnt(int cnt, int oldb, int newb)
            => (int)((cnt - 1) * Log(newb, oldb)) + 1;

        static Func<big, uint[]> CreateFunc()
        {
            var x = Expression.Parameter(typeof(big));
            var expr = Expression.Lambda<Func<big, uint[]>>(
                Expression.Field(x, "_bits"), x);
            return expr.Compile();
        }

        public static Lazy<Func<big, uint[]>> getBits = new Lazy<Func<big, uint[]>>(CreateFunc);

        static int DC2(big num)
        {
            var bits = getBits.Value(num);
            if (null == bits)
                return (int)Log((double)num, 2) + 1;
            int len = bits.Length - 1;
            uint lst = bits[len];
            len *= 32;
            if (0 != lst) len += (int)Log(lst, 2) + 1;
            return len;
        }
        static int DC16(big num)
        {
            var bits = getBits.Value(num);
            if (null == bits)
                return (int)Log((double)num, 16) + 1;
            int len = bits.Length - 1;
            uint lst = bits[len];
            len *= 8;
            if (0 != lst) len += (int)Log(lst, 16) + 1;
            return len;
        }
        static int DCN(big num, int basis)
        //  => (int)big.Log(num, basis) + 1;
            => (int)Log((double)num, basis) + 1;
        static int DC10by2(big num)
            => (int)Floor((DC2(num) - 1) * Log10(2)) + 1;
        static int DC10by2_(big num)
            => (int)Floor(DC2(num) * Log10(2)) + 1;
        static int DC10by16(big num)
            => (int)Floor((DC16(num) - 1) * Log10(16)) + 1;

        static double SafePow2(int exp)
        {
            if (exp > +1023) return double.PositiveInfinity;
            long temp;
            if (exp < -1074) return 0;
            else if (exp < -1022)
                temp = 1L << 1074 + exp;
            else
                temp = 1023L + exp << 52;
            return BitConverter.Int64BitsToDouble(temp);
        }
        static double SafePow2_modern(int exp) => exp switch
        {
            _ when exp < -1074 => 0,
            _ when exp > 1023 => double.PositiveInfinity,
            _ => BitConverter.Int64BitsToDouble(exp switch
            {
                _ when exp < -1022 => 1L << 1074 + exp,
                _ => 1023L + exp << 52
            })
        };

        static unsafe long tol(double x) => *(long*)&x;

        [STAThread]
        static void Main()
        {
            Currying.Main();
            //Parallel.For(0, int.MaxValue, TestDigitCount);
            //for (int i = 0; i < int.MaxValue; i++) TestDigitCount(i);
            //nums.MainTest();
            Console.WriteLine(" --end program-- ");
            Console.ReadLine();
        }

        private static void TestDigitCount(int i)
        {
            big num = new big(i)// << 32
                ;
            int dcn = DCN(num, 10),
                dc2 = DC10by2(num),
                dc16 = DC10by16(num);
            if (
                //dcn != dc2 || dcn != dc16
                dcn != Max(dc2, dc16)
                )
                Console.WriteLine("WRONG NUM: {0} | {1}, {2}, {3}",
                    num, dcn, dc2, dc16);
        }

        private static void PrintParserTests(NotationParser parser, string[] expr)
        {
            Console.WriteLine("Parser:{0}", parser);
            foreach (var exp in expr)
                Console.WriteLine("'{0}' => {1}", exp, parser.Parse(exp));
        }

        static void combine(string Str)
        {
            for (int i = 0; i < Str.Length; i++)
                combine(Str, i, null);
            void combine(string str, int ind, string sum)
            {
                int length = str.Length - 1; sum += str[ind];
                if (length == 0) { Console.WriteLine(sum); return; }
                str = str.Remove(ind) + str.Substring(ind + 1);
                for (int i = 0; i < length; i++) combine(str, i, sum);
            }
        }
        static int gray(int g) // Код Грея
        { return g ^ g >> 1; }
        private static void waq(string[] args)
        {
            string strHostName = "";
            if (args.Length == 0)
            {
                // Getting Ip address of local machine...

                // First get the host name of local machine.

                strHostName = Dns.GetHostName();
                Console.WriteLine("Local Machine's Host Name: " + strHostName);
            }
            else
            {
                strHostName = args[0];
            }

            // Then using host name, get the IP address list..

            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;

            for (int i = 0; i < addr.Length; i++)
            {
                Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
            }
        }
        static public string strrange(char v, int before, int after)
        {
            var sb = new StringBuilder();
            after += v;
            v -= (char)before;
            /*for (char x = v; x <= after; x++)
            {
                sb.Append(x);
            }*/
            while (v < after) sb.Append(v++);
            return sb.ToString();
        }
    }
}