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
using System.Linq;
using System.Threading;

#nullable enable

namespace useless
{
    public static class Program
    {
        static string[] GetTags(string text)
        {
            List<string> list = new List<string>();
            int index = 0, len, length = text.Length;
            while ((index = text.IndexOf('#', index) + 1) != 0)
            {
                len = 0;
                for (int i = index; i < length; ++i, ++len)
                    if (char.IsWhiteSpace(text[i]) || text[i] == '#') break;
                list.Add(text.Substring(index, len));
            }
            return list.ToArray();
        }
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
            while (c != 0) (_, b, c) = (b, c, b % c);
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

        static void printArr<T>(T[] arr, string? format = null)
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
            if (obj is null) return "";
            var list = new Stack<object?>();
            var regex = new Regex(@"(?<!{){\s*(\w+).*?(?=})");
            var matches = regex.Matches(format);
            var type = obj.GetType();
            var sb = new StringBuilder(format);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                object? value;
                var match = matches[i];
                var name = match.Groups[1].Value;
                if ("this" == name) value = obj;
                else
                {
                    var prop = type.GetProperty(name);
                    value = prop?.GetValue(obj);
                }
                list.Push(value);
                sb.Remove(match.Index + 1, match.Groups[1].Length);
                sb.Insert(match.Index + 1, i);
            }
            return string.Format(sb.ToString(), list.ToArray());
        }

        static object Elapsed(Stopwatch s) => s.ElapsedTicks.ToString("N");


        public static void AddDistinct<T>(
            this IList<T> list, T value)
        {
            if (!list.Contains(value))
                list.Add(value);
        }

        public static string[] GetStringsByRegex(
            int length,
            string[] sources,
            params string[] patterns)
        {
            IEnumerable<char> dist;
            if (sources == null)
                sources = new string[length];
            else
            if (sources.Length != length)
                throw new Exception();
            for (int i = 0; i < length; i++)
                if (string.IsNullOrEmpty(sources[i]))
                    sources[i] = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ\\";
                else if ((dist = sources[i].Distinct()).Count() != sources[i].Length)
                    sources[i] = dist.Aggregate(new StringBuilder(), (sb, v) => sb.Append(v)).ToString();

            var results = new List<char>[length];

            for (int i = 0; i < length; i++)
                results[i] = new List<char>();

            var regexes = patterns.Select(x => new Regex($"^{x}$", RegexOptions.IgnoreCase));
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(sources[i][0]);
            do
            {
                var str = sb.ToString();
                if (regexes.All(r => r.IsMatch(str)))
                    for (int i = 0; i < length; i++)
                        results[i].AddDistinct(str[i]);

                bool lastOfRange = true;

                for (int i = 0; lastOfRange && (i < length); ++i)
                {
                    int ind = sources[i].IndexOf(sb[i]) + 1;
                    if (ind < sources[i].Length)
                        lastOfRange = false;
                    else ind = 0;
                    sb[i] = sources[i][ind];
                }

                if (lastOfRange) break;
            }
            while (true);

            var tmp = new string[length];
            for (int i = 0; i < length; i++)
                tmp[i] = string.Join("", results[i]);

            return tmp;
        }

        public static string[] Split(
            this string str,
            Predicate<char> pred,
            int count = short.MaxValue,
            bool removeEmptyEntries = false)
        {
            // setup
            var splitted = new string[count--];
            int index = 0, v = 0, len = str.Length;
            string tmp;
            // action
            var sb = new StringBuilder(len);
            while (index < count && v < len)
            {
                if (pred(str[v]))
                {
                    tmp = sb.ToString();
                    if (!removeEmptyEntries || !string.IsNullOrEmpty(tmp))
                        splitted[index++] = tmp;
                    sb.Clear();
                }
                else sb.Append(str[v]);
                ++v;
            }
            while (v < len)
                sb.Append(str[v++]);
            tmp = sb.ToString();
            if (!removeEmptyEntries || !string.IsNullOrEmpty(tmp))
                splitted[index++] = tmp;
            // result
            Array.Resize(ref splitted, index);
            return splitted;
        }

        static T ChangeBounds<T>(T array, int lower, int length)
            where T : class
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (!(array is Array arr) || arr.Rank != 1)
                throw new ArgumentException("T must be array type with rank = 1.");
            Array res = Array.CreateInstance(
                arr.GetType().GetElementType(), new[] { length }, new[] { lower });
            int minLen = Math.Min(length, arr.Length);
            Array.Copy(arr, arr.GetLowerBound(0), res, lower, minLen);
            return (res as T)!;
        }
        static T ChangeBounds<T>(T array, int[] lower, int[] length)
            where T : class
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (!(array is Array arr))
                throw new ArgumentException("T must be array type with rank = 1.");
            Array res = Array.CreateInstance(
                arr.GetType().GetElementType(), length, lower);
            int minLen = arr.Length;
            Array.Copy(arr, arr.GetLowerBound(0), res, res.GetLowerBound(0), minLen);
            return (res as T)!;
        }

        public static unsafe Array chBnds(Array array, int[] newLowBounds)
        {
            int rank = array.Rank;
            if (rank != newLowBounds.Length)
                throw new ArgumentException();
            // https://habr.com/ru/company/luxoft/blog/219619/
            IntPtr p = SafePtr.Create(array).IntPtr;
            int* v = (int*)p;
            for (int i = 0, j = -(rank + 3) / 2 * 2, k = rank + 4; i < rank; ++i, ++j, ++k)
                v[k] = v[j] = newLowBounds[i];
            return array;
        }

        public static unsafe void prntUnsafe(Array array, int from = -10, int? to = null)
        {
            IntPtr p = SafePtr.Create(array).IntPtr;
            int* v = (int*)p;
            int end = to ?? array.Length;
            for (int i = from; i < end; ++i)
                if (i == 0)
                    Console.Write("[0] " + v[i] + " ");
                else
                    Console.Write(v[i] + " ");
            Console.WriteLine();
        }

        public static Array CreateArray(
            int rank, int fst = 2, int mdl = 1, int end = 3, int len = 1)
        {
            int[] length = new int[rank],
                bounds = new int[rank];
            for (int i = 0; i < rank; i++)
            {
                length[i] = len;
                bounds[i] = mdl;
            }
            bounds[rank - 1] = end;
            bounds[0] = fst;
            return Array.CreateInstance(typeof(byte), length, bounds);
        }

        static void printBounds(Array array)
        {
            Console.Write("Lower bounds: ");
            for (int i = 0; i < array.Rank; i++)
                Console.Write(array.GetLowerBound(i) + " ");
            Console.WriteLine();
        }

        static void ArrTest(int length = 4)
        {
            for (int i = 1; i < length; i++)
            {
                var arr = CreateArray(i);
                Console.WriteLine($"Rank = {i}, array type = {arr.GetType()}");
                prntUnsafe(arr, to: 10);
                printBounds(arr);
                prntUnsafe(chBnds(arr, Enumerable.Range(0, i).Select(x => 5).ToArray()), to: 10);
                printBounds(arr);
                Console.WriteLine();
            }
        }

        static uint[,]? GetMatrix()
            => (uint[,])chBnds(new uint[,] { { 1, 2 }, { 3, 4 } }, new[] { 10, 10 });

        static string Substract(this string value, string sub)
        {
            char old = '\0';
            while (value.Contains(old)) ++old;
            int ind = 0;
            var sb = new StringBuilder(value);
            foreach (var chr in sub)
            {
                ind = value.IndexOf(chr, ind);
                if (ind == -1) return null!;
                sb[ind] = old;
            }
            return sb.Replace(old.ToString(), "").ToString();
        }

        static int count(IEnumerable<int> seq)
        {
            int cmp(int l, int r) => l * r < 0 ? 1 : 0;
            if (seq.Count() < 2) return 0;
            return cmp(seq.FirstOrDefault(), seq.ElementAt(1)) + count(seq.Skip(1));
        }

        static int SumOfRangeTo(int x)
        {
            int sum = x;
            while (x-- > 0)
                sum += x;
            return sum;
        }

        static readonly PropertyInfo debug =
            typeof(Expression).GetProperty("DebugView", (BindingFlags)(-1));

        static void print(Expression expression)
            => Console.WriteLine(debug.GetValue(expression));

        /*
                #  # #### ###  ####
                #  # #    #  # #   
                #### #### ###  ####
                #  # #    #  # #
                #  # #### #  # ####
         */

        static string SortDist(this string str)
        => string.Join("", str.OrderBy(x => x));
        [STAThread]
        static void Main()
        {
            Enumeration.TypeCode code = Enumeration.TypeCode.Object;

            float addBit(float x, int bit = 1) =>
                BitConverter.ToSingle(
                    BitConverter.GetBytes(
                        bit + BitConverter.ToInt32(
                            BitConverter.GetBytes(x), 0)), 0);

            float x = 0;
            for (int i = 0; i < 50; i++)
            {
                Console.WriteLine("{0}: {1}", i, x);
                x = addBit(x);
            }
            return;

            var lambda = BrainFuck.LambdaSource(@"+[>+]");
            print(lambda);
            //var wtf = lambda.Compile();wtf();
            Application.EnableVisualStyles();
            new RegexSolver().ShowDialog();
            //Parallel.For(0, int.MaxValue, TestDigitCount);
            //for (int i = 0; i < int.MaxValue; i++) TestDigitCount(i);
            //nums.MainTest();
            Console.WriteLine(" --end program-- ");
            //Console.ReadLine();
        }
        static void write(params object[] array)
            => Console.Write(string.Concat(array));

        static void writeln(params object[] array)
            => Console.WriteLine(string.Concat(array));

        static string readlexem()
        {
            var buf = new StringBuilder();
            char x;
            while (char.IsWhiteSpace(x = (char)Console.Read())) ;
            buf.Append(x);
            while (!char.IsWhiteSpace(x = (char)Console.Read()))
                buf.Append(x);
            return buf.ToString();
        }
        unsafe static void read_t<T>(in T a)
            where T : unmanaged
        {
            fixed (T* p = &a) *p = (T)Convert
                    .ChangeType(readlexem(), typeof(T));
        }
        static void read<A>(in A a)
            where A : unmanaged
        {
            read_t(a);
        }
        static void read<A, B>(in A a, in B b)
            where A : unmanaged
            where B : unmanaged
        {
            read_t(a);
            read_t(b);
        }
        static void read<A, B, C>(in A a, in B b, in C c)
            where A : unmanaged
            where B : unmanaged
            where C : unmanaged
        {
            read_t(a);
            read_t(b);
            read_t(c);
        }

        class StringContainear
        {
            string str;
            List<int> list;
            object locker;
            int length, max;
            public StringContainear(string str, List<int> list, object locker, int max = int.MaxValue)
            {
                (this.str, this.list, this.locker, this.max, length) = (str, list, locker, max, str.Length);
            }
            public void Invoke(int seed)
            {
                Random random = new Random(seed);
                for (int i = 0; i < length; i++)
                    if (random.Next(max) != str[i]) return;
                lock (locker)
                    list.Add(seed);
            }
        }

        public static int[] GetSeedByStr(string str)
        {
            List<int> list = new List<int>();
            object obj = new object();
            var q = new StringContainear(str, list, obj);
            var hndl = Parallel.For(0, int.MaxValue, q.Invoke);
            while (!hndl.IsCompleted) Thread.Sleep(1000);
            return list.ToArray();
        }

        static double[] convert(double value, double radix)
        {
            List<double> list = new List<double>();
            do
            {
                var tmp = value % radix;
                list.Add(tmp);
                value = (value - tmp) / radix;
            }
            while (value != 0 && list.Count < 15);
            list.Reverse();
            return list.ToArray();
            
        }

        static List<string> combine(string Str)
        {
            var list = new List<string>();
            for (int i = 0; i < Str.Length; i++)
                combine(Str, i, "");
            return list;

            void combine(string str, int ind, string sum)
            {
                int length = str.Length - 1;
                sum += str[ind];
                if (length == 0)
                {
                    list.AddDistinct(sum);
                    return;
                }
                str = str.Remove(ind) + str.Substring(ind + 1);
                for (int i = 0; i < length; i++)
                    combine(str, i, sum);
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