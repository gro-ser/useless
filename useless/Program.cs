using GraphTasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestingTasks;
using static System.Linq.Enumerable;
using static System.Math;

#nullable enable

namespace useless
{
    public static class Program
    {
        private static string[] GetTags(string text)
        {
            List<string> list = new List<string>();
            int index = 0, len, length = text.Length;
            while ((index = text.IndexOf('#', index) + 1) != 0)
            {
                len = 0;
                for (int i = index; i < length; ++i, ++len)
                {
                    if (char.IsWhiteSpace(text[i]) || text[i] == '#')
                    {
                        break;
                    }
                }

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
            StringBuilder sb = new StringBuilder(str);
            char x;
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                x = sb[i];
                if (x >= '!' && x <= '~')
                    sb[i] = (char)(x + 65248);
            }
            return sb.ToString();
        }

        private static long GCD(long a, long b)
        {
            long c = b == 0 ? a : a % b;
            while (c != 0)
                (_, b, c) = (b, c, b % c);
            return Abs(b);
        }

        private static long Gcd(long a, long b)
        {
            for (long c = b == 0 ? a : a % b;
                c != 0; a = b, b = c, c = a % b)
            {
                ;
            }

            return Abs(b);
        }

        private static double InfSearch(
            Func<double, bool> pred,
            Func<double, double> step,
            double start = 1)
        {
            double value = start;
            start = double.NaN;
            while (pred(value) && start != value)
            {
                //(value, start) = (step(value), value);
                start = value;
                value = step(value);
            }
            return start;
        }

        private static T[] Insert<T>(T[] _args, int ind, T value)
        {
            int len = _args.Length;
            T[] args = new T[len + 1];
            Array.Copy(_args, 0, args, 0, ind);
            Array.Copy(_args, ind, args, 1 + ind, len - ind);
            args[ind] = value;
            return args;
        }

        private static void testArglist(__arglist)
        {
            const int newint = 10;
            const double newdouble = 0.1;
            ArgIterator iter = new ArgIterator(__arglist);
            int length = iter.GetRemainingCount();
            for (int i = 0; i < length; i++)
            {
                Type type = Type.GetTypeFromHandle(iter.GetNextArgType());
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

        private static void printArr<T>(T[] arr, string? format = null)
        {
            string tostr()
            {
                int length = arr.Length;
                if (length == 0)
                    return "[]";
                StringBuilder sb = new StringBuilder("[").AppendFormat(format, arr[0]);
                for (int i = 1; i < length; i++)
                    sb.Append(", ").AppendFormat(format, arr[i]);
                return sb.Append(']').ToString();
            }
            Console.WriteLine(tostr());
        }

        private static readonly IFormatProvider provider = CultureInfo.GetCultureInfo("en-en");

        private static string Format(this string format, params object[] args) => string.Format(provider, format, args: args);

        private static string Format(this string format, params (string name, object obj)[] args)
        {
            Regex regex = new Regex(@"^\s*(\w+)\s*$");
            int length = args.Length;
            object[] objs = new object[length];
            for (int i = 0; i < length; i++)
            {
                (string name, object obj) arg = args[i];
                objs[i] = arg.obj;
                Match m = regex.Match(arg.name);
                if (!m.Success)
                    throw new ArgumentException(nameof(arg.name));
                string pattern = $@"{{\s*{m.Groups[1].Value}(?=.*}})";
                format = Regex.Replace(format, pattern, "{" + i);
            }
            return string.Format(provider, format, objs);
        }

        private static string DynamicFormat(this string format, object obj)
        {
            if (obj is null)
                return "";
            Stack<object?> list = new Stack<object?>();
            Regex regex = new Regex(@"(?<!{){\s*(\w+).*?(?=})");
            MatchCollection matches = regex.Matches(format);
            Type type = obj.GetType();
            StringBuilder sb = new StringBuilder(format);
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                object? value;
                Match match = matches[i];
                string name = match.Groups[1].Value;
                if ("this" == name)
                {
                    value = obj;
                }
                else
                {
                    PropertyInfo prop = type.GetProperty(name);
                    value = prop?.GetValue(obj);
                }
                list.Push(value);
                sb.Remove(match.Index + 1, match.Groups[1].Length);
                sb.Insert(match.Index + 1, i);
            }
            return string.Format(sb.ToString(), list.ToArray());
        }

        private static object Elapsed(Stopwatch s) => s.ElapsedTicks.ToString("N");


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
            {
                if (string.IsNullOrEmpty(sources[i]))
                    sources[i] = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ\\";
                else if ((dist = sources[i].Distinct()).Count() != sources[i].Length)
                    sources[i] = dist.Aggregate(new StringBuilder(), (sb, v) => sb.Append(v)).ToString();
            }

            List<char>[] results = new List<char>[length];

            for (int i = 0; i < length; i++)
                results[i] = new List<char>();

            global::System.Collections.Generic.IEnumerable<global::System.Text.RegularExpressions.Regex> regexes = patterns.Select(x => new Regex($"^{x}$", RegexOptions.IgnoreCase));
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(sources[i][0]);
            do
            {
                string str = sb.ToString();
                if (regexes.All(r => r.IsMatch(str)))
                {
                    for (int i = 0; i < length; i++)
                        results[i].AddDistinct(str[i]);
                }

                bool lastOfRange = true;

                for (int i = 0; lastOfRange && (i < length); ++i)
                {
                    int ind = sources[i].IndexOf(sb[i]) + 1;
                    if (ind < sources[i].Length)
                        lastOfRange = false;
                    else
                        ind = 0;
                    sb[i] = sources[i][ind];
                }

                if (lastOfRange)
                    break;
            }
            while (true);

            string[] tmp = new string[length];
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
            string[] splitted = new string[count--];
            int index = 0, v = 0, len = str.Length;
            string tmp;
            // action
            StringBuilder sb = new StringBuilder(len);
            while (index < count && v < len)
            {
                if (pred(str[v]))
                {
                    tmp = sb.ToString();
                    if (!removeEmptyEntries || !string.IsNullOrEmpty(tmp))
                        splitted[index++] = tmp;
                    sb.Clear();
                }
                else
                {
                    sb.Append(str[v]);
                }

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

        private static T ChangeBounds<T>(T array, int lower, int length)
            where T : class
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (!(array is Array arr) || arr.Rank != 1)
                throw new ArgumentException("T must be array type with rank = 1.");
            Array res = Array.CreateInstance(
                arr.GetType().GetElementType(), new[] { length }, new[] { lower });
            int minLen = Math.Min(length, arr.Length);
            Array.Copy(arr, arr.GetLowerBound(0), res, lower, minLen);
            return (res as T)!;
        }

        private static T ChangeBounds<T>(T array, int[] lower, int[] length)
            where T : class
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
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
            {
                if (i == 0)
                    Console.Write("[0] " + v[i] + " ");
                else
                    Console.Write(v[i] + " ");
            }

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

        private static void printBounds(Array array)
        {
            Console.Write("Lower bounds: ");
            for (int i = 0; i < array.Rank; i++)
                Console.Write(array.GetLowerBound(i) + " ");
            Console.WriteLine();
        }

        private static void ArrTest(int length = 4)
        {
            for (int i = 1; i < length; i++)
            {
                Array arr = CreateArray(i);
                Console.WriteLine($"Rank = {i}, array type = {arr.GetType()}");
                prntUnsafe(arr, to: 10);
                printBounds(arr);
                prntUnsafe(chBnds(arr, Enumerable.Range(0, i).Select(x => 5).ToArray()), to: 10);
                printBounds(arr);
                Console.WriteLine();
            }
        }

        private static uint[,]? GetMatrix()
            => (uint[,])chBnds(new uint[,] { { 1, 2 }, { 3, 4 } }, new[] { 10, 10 });

        private static string Substract(this string value, string sub)
        {
            char old = '\0';
            while (value.Contains(old))
                ++old;
            int ind = 0;
            StringBuilder sb = new StringBuilder(value);
            foreach (char chr in sub)
            {
                ind = value.IndexOf(chr, ind);
                if (ind == -1)
                    return null!;
                sb[ind] = old;
            }
            return sb.Replace(old.ToString(), "").ToString();
        }

        private static int count(IEnumerable<int> seq)
        {
            int cmp(int l, int r) => l * r < 0 ? 1 : 0;
            if (seq.Count() < 2)
                return 0;
            return cmp(seq.FirstOrDefault(), seq.ElementAt(1)) + count(seq.Skip(1));
        }

        private static int SumOfRangeTo(int x)
        {
            int sum = x;
            while (x-- > 0)
                sum += x;
            return sum;
        }

        private static readonly PropertyInfo debug =
            typeof(Expression).GetProperty("DebugView", (BindingFlags)(-1));

        private static void print(Expression expression)
            => Console.WriteLine(debug.GetValue(expression));

        /*
                #  # #### ###  ####
                #  # #    #  # #   
                #### #### ###  ####
                #  # #    #  # #
                #  # #### #  # ####
         */

        private static string SortDist(this string str)
        => string.Join("", str.OrderBy(x => x));

        public static StackTrace GetStackTrace(int n)
        {
            if (n == 0)
                return new StackTrace();
            return GetStackTrace(n - 1);
        }

        private static decimal StringToDecimal(string str)
        {
            BitArray bytes = new BitArray(Encoding.UTF8.GetBytes(str));
            int[] tmp = new int[4];
            bytes.CopyTo(tmp, 0);
            return new decimal(tmp);
        }

        private static string DecimalToString(decimal x)
        {
            int[] bits = decimal.GetBits(x);
            BitArray arr = new BitArray(bits)
            {
                Length = 3 * 4 * 8
            };
            byte[] res = new byte[3 * 4];
            arr.CopyTo(res, 0);
            string str = Encoding.UTF8.GetString(res);
            return str;
        }

        static Program()
        {
        }

        private static BigInteger StrToBig(string str) => new BigInteger(Encoding.UTF8.GetBytes(str));
        private static string BigToStr(BigInteger big) => Encoding.Default.GetString(big.ToByteArray());

        public static void CalliTest()
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName
            { Name = "mda" };

            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(
                myAsmName,
                AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder myModBuilder = myAsmBuilder.DefineDynamicModule(
                "mda", "mda.dll");

            TypeBuilder myTypeBuilder = myModBuilder.DefineType("CalliTest",
                TypeAttributes.Public);

            MethodBuilder myMthdBuilder = myTypeBuilder.DefineMethod("Calli",
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(void),
                new Type[] { typeof(string) });

            ILGenerator myIL = myMthdBuilder.GetILGenerator();

            myIL.Emit(OpCodes.Ldarg_0);
            myIL.Emit(OpCodes.Ldarg_1);
            myIL.EmitCalli(OpCodes.Calli,
                CallingConvention.StdCall,
                typeof(void),
                new Type[] { typeof(string) });
            myIL.Emit(OpCodes.Ret);
            myTypeBuilder.CreateType();
            myAsmBuilder.Save("mda.dll");
        }

        private static readonly MethodInfo cw = ((Action<int>)Console.WriteLine).Method;
        public static Action<int> Create(CallingConvention conv)
        {
            DynamicMethod dm = new DynamicMethod(
                "prnt",
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(void),
                new Type[] { typeof(int) },
                typeof(object),
                false);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, cw);
            il.EmitCalli(OpCodes.Calli,
                conv,
                typeof(void),
                new Type[] { typeof(int) });
            il.Emit(OpCodes.Ret);
            return (Action<int>)dm.CreateDelegate(typeof(Action<int>));
        }

        private static void AddDistinct(this List<char> list, char ch)
        {
            if (!list.Contains(ch))
                list.Add(ch);
        }

        private static string ExecMacro(string file)
        {
            string[] lines = File.ReadAllLines(file);
            StringBuilder buf = new StringBuilder();
            Dictionary<string, string> macro = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                if (line.StartsWith("#define "))
                {
                    int ind = line.IndexOf(' ', 8);
                    if (-1 == ind)
                    {
                        buf.Append(line);
                    }
                    else
                    {
                        macro.Add(line.Substring(8, ind - 8), line.Substring(ind + 1));
                    }
                }
                else
                {
                    foreach (string part in line.Split())
                    {
                        buf.Append(macro.TryGetValue(part, out string val) ? val : part);
                    }
                }
                buf.AppendLine();
            }
            return buf.ToString();
        }

        private static string GetNumFromOnes(int num)
        {
            if (num == 0)
                return "1 - 1";
            if (num == 1)
                return "1";
            if (num < 0)
            {
                if (num == int.MinValue)
                    return "1 << " + GetNumFromOnes(31);
                return $"-({GetNumFromOnes(-num)})";
            }
            string? shift = null;
            double log;
            if ((log = Math.Log(num, 2)) == (int)log)
                shift = "";
            else if ((log = Math.Log(num - 1, 2)) == (int)log)
                shift = " + 1";
            else if ((log = Math.Log(num + 1, 2)) == (int)log)
                shift = " - 1";
            if (shift != null)
                return $"(1 << {GetNumFromOnes((int)log)}){shift}";
            int x = 1 << (int)log;
            return $"({GetNumFromOnes(x)}) | ({GetNumFromOnes(num ^ x)})";
            return $"{num}";
        }

        [STAThread]
        private static int Main(string[] args)
        {
            //goto start;
            bool print = true;

            (string, string)[] task1Data =
            {
                (@"
3
WXX
WYX
XW", "WXY"),
                (@"
3
X
Y
A", "XYA"),
                (@"
11
XSD
XDS
AXX
AXD
ASSSS
ADSS
DSS
DSDAX
DSDD
DA
DAXS", "XSAD"),
                (@"
1
Z", "Z"),
                (@"
7
ZZ
ZCB
BCZH
BH
BBHC
BBBZ
BBBH", "ZCHB"),
                (@"
2
AAA
ABC", "CAB"),
                (@"400
PTIYGDTXDPOXIBGGFTGC
PTICKIZFTBKUPYWARYEGGPETIPDYBAPZ
PTICKIZFTXIYMZNHPWWOK
PTICKIZFTXIYMZNHPVGJCFULQDDB
DZ
OEIBHQHIFPQOIGKYFVSACE
OEIBHQHIFPQOIGKJWHQMTTIKACA
OEIBHQHIFPQOIGKJWHQMTTIKAKBIYBWOM
OEIBCMXQ
OEIBCUXNV
OEIBCUKEOMYS
OEIBCUKEOLUSDSMQARGJT
OEIBCUKEOLUSDSMQWRUGTVBYNRHA
OEIBCUKEOLUSDSMQWRUGTVKPTUDWNBSUZFLUCSPE
OEIBCUKEOLUSDSMQWRUGTVKPTUDWKBTRSNMCBW
OEIBCUKEOLUSLBMXMLUPCZPMVWXVZMCAQMDILKP
OEIBCUKEOLUSLBMXMLUPCZPMVWXVZMCAQCJGKI
OEIBCUKEOLUSLBMKKLTSYIRRNTKV
OEIBCUKEOLUNPDRGONFPZHFN
OEIBCUKEOLUNSUVOGDWCPVWEILQDYKT
OEIBCUKEOLKY
OBUF
OBBZKSISSJ
OBKQBS
OKZKZVUUAPHCJ
OKZKZCICNEIXLVJLSRDXFFJXGEYVNDPMOIOPV
OKZKZCICNEIXLVLEYQXRJUZAZBDVIJMDJ
OKZKZCICNEIXLVCZPSOL
OKZKZCICNEIXLVCZPXESGT
OKZKHU
OKZKHXZ
OKBQENFAVXJBZWJZPUO
OKBQENFNOYSRYWHUULBQLRSEKQXHGDQWX
OKBQENFNOYSRYWHUULBQLRSEKLTXKTCUTYQJ
OKBQENFNOYSRYWHUULBQLRSEKLKVOBEUZOAXECSF
OKBQENFNOYSRYWHUULBQLRSEKLKVOXZTOUZHJL
OKBQENFNOYSRLXFGWROQCGZKFDNXXZF
OKBQENFNOYSRLXFGWROQCGZKFDNXXMAMB
NPJHGEXXBWAESMRTFRXPNAZRPGRZQZQGW
NPJHGEXXBWAESMRTFRKFVCQMWYCYUDHX
NPJHGEXXBWAESMRTFRKFVCQMWYCYKOSWGWNCC
NPJHGEXKFFCWDPSKVARJW
NPJHGEXKFFCWDPBMKQHHRCHSIXVSHHTHOBGHSOE
NPJHGEXKFFCWDPBMKQHHRCHSIXVSHHTHC
NPJHGEXKFFCWDPBMKQHHRCHSIXVCSNNUEZ
NPJHGEXKFFCWDPBMKQHHRCHSIXVCFDHLUNAUBW
NPJHGEXKFFKEUHR
NPJHGEXKFLCIYXDQMEHROIUFTVVIFZZCKPC
NPJHYVTEUFYXQQBYLHGYIGMMENOGO
NPJHYVTEUFYXQQBCCHEMW
NPJHYYO
NPJHLZVLNVIXHKQEBJAHVUYWGLGBARRXIT
NPJHLZVLNVIXHKQEBJAHVUYWGLGBARRXYPJOKXK
NPJHLZVLJJLFG
NPJHLZVLJLCIXKUACILRPYDZCSEJBHLFO
NPJHLZVLJLCIXKBS
NPJHLZVLJLBZGHJMGALAMHXFGDGDSPWPDOPCJCP
NPJHLZVLJLKRTBYHLPDTDXQPOHFMWTXSY
NPJVEKQOAUKMSUADIFRMFGPASPZMEDNMSL
NPJVEKQOAUKMSUADIFNXGWXNSOVAHEMQSIIWHRJV
NPXXAFOEIRLLQAKQVEJXVZTTW
NPXXAFHWBHGWDSFXNOKABOLPJIQHJAJZUGOED
NPXXAFHWBHGWDSBMHCHIPJRTAYLOGRKVLI
NPXXAFHOLMMTQFDUQCLWCKPI
NPXXAJZFJQKLHSMXJJNYQQGZQPTEWEFSROAMRPP
NPXXAJZFJQKLHSMXJJNYQQGZQPESWDYDKLN
NPXXAJZFXPFXYGWHHPTILVVKXDWRUWAS
UDMLTNBBBMYELBMAFQAD
UDMLTNBBBMYELKCFGMJWSDFXSARNIMTZRDS
UDMLTNBBBMYELKCFGMJWSDFXSARNKETXWJP
UDMLTNBBBMYELKCFGMJWSDFXSARNKETXBGYS
UDMLTNBBBMYCZH
UDMLTNBBBMBFJBQVVARYRPNLAVOZLKMC
UDMLTNBBBMBFJBQVVARBSQOFJRBAXDARMLZJYGB
UDMLTNBBBMBFJBQVVARBSQOFJRBBDSF
UDMLTNBBBMBFJBQVVARBSQOFJRBBDSJNWIXPBRF
UDMLTNBBBMBFJBQVVARBSQOFJRBBDSL
UDMLTNBBKOGEILSLQXOTKWTHPLTVFJRG
UDMLTNBBKOGEGCAPVKTL
UDMLRBMY
UDMLNIZLTYMZQZHYRDZONXXZKHSXOJPQ
UDMLNIZLTYMZQZHYRDM
UDMLNIZLTYMKVFMAEFSKBSGS
UDMLNIZLTYMKVFCQZVQTAURUJKPU
UDMBXTOXUQTB
UNNXZOEDHGYKEAWRZMJLU
UNNXZOEDHGYKEAWRZKZASTAETU
UNNXZOEDHGYKEAWRZKZASTQGTXWUNZARDXRENKB
UNEHFSSCAQYMJLTORKZJHCDSMCCTPIVLYQOEJYEY
UNEHFSSCAQYMJLTORKZJHCDSMCCTPIVLYQOEC
UNEHFSSCAQYMJLTORKZJHCDSMCCTPIVLYQOULP
UBYGXLCZABAAZURIVFBWBXUGSZVHYKEGVOYNQIXT
UBYGXLCZABAAZUU
UBYGXLXDRJVVUVMJWEDNDGFPRPOOENXOX
UBYGXLXDRJVVUVMJWEDNDGFPRPOOENXOKJNMST
UBYGXLXGTDCYEEIW
UBYGXLXGTOEATFVXGTS
UBYGXLXGTOEATFVXGVBVYCQJZUFZXAD
UBYGXLXGTOEATFVXGVBVKK
UXGYNL
UKFN
UKCCRPERYBZCHUXTAR
UKCCRPERUGWFFSETSRQAHPLQV
UKCCRPERUGWFFSETSRQAHPUBHGKARCIST
UKCCRPERUGKGYJ
UKCKIAUGDHBLILUTI
UKCKIAUGBIKYKAUJAFWMDOSWAONJKYHA
UKCKIAUGBIKYKAUJAFWMDOSWAOXKDSFLNAKPSVTP
UKCKIAUGBIKYKAKHBONJZFOWRGY
UKCKIAUGBIKYKAKHBONJZFOKUX
UKBKISOADEMNGO
UKBKISOADEMKJKAJEZ
UKBKISOADEMKJKAJEBQO
UKBKISOADEMKXKEYTRCIYMTJMJMJHHMSSG
UKBKISOADEMKXKEYTRCIYMTJMJMJHMPQEHKXWB
UKBKISOADEMKXKEYTRCIYMTJMJMJHMPNBNOCZR
UKBKISOADEMKXKEYTZY
UKBKISOAYTPQKM
UKKEWRJCFM
CHPDXZWOVFVSZYXWATPSTRUTCZGXJOOB
CHPDXZWNWGGIUQJVYLBLTZVFLPKCVPJHNVKO
CHPDXZWNWGGIUQJVYLBCARFZCTDPGHWXNTQUG
CHPDXZWYNXHSJXWJ
CHPWDBXWDKILWTZIUAGNULSOKVXK
CEOXWCMXSIBXEBEGFSZAJBKZ
CEOXWCMXSIBXEBEGFSZNIP
CEOXWCMXSIBXEBEGFSZEZMERDRQPABBMLDOGAM
CEOXWCMXSIBXEBEGFSZEZMERDRQPABBMKWIO
CEOXWCMXSIBXEBEGFSZEZMERDRQPABBMKJUUIVXI
CEOXWCMXSIBXEBEGFSZEZMCAWVSBC
CEOXYHVRVQRCKF
CEOXYHVRVQRCKJSCVDWWSUCMZNFNMG
CEOXYHVRVQRCKJSCVDWWSUCMXJ
CEOXYVOFTUZXANDCQLZIPPMZ
CEVWXHXOKMQK
CEVHTAYUOBHQVSGLC
CEVHTAYUOBHKPHUFDSDGNZWVTMNEJGIKFBVAIC
CEYNZCXINFTAJZDECUNCH
CEYNZCXINFTAJZDEKODUBWQXVUNWIXOYHIBXDRU
CEYNZCXINFTAJZDEKODUKTSOVWKHCQIHB
CEB
CLDZZIIKGRWXKPUFMOPPT
BQPWMFCCVMESPMEXZTFHINVOY
BQPWMFCCVMESPMEXZTFHINVOB
BQPWMFCCVMESIXFHBFCLVDWECARWAIFAWUOJJJ
BQPWMFCCVMESIXFHBFCKBEOQZJGIJBPDTXH
BQPWMFCCVMEFCBTVQQOIVVSTMBO
BQPWMFCCVMEFCBTVQQOIVVSTMKPAHIIDFRMTS
BQPWMFCCVMEFCBTVQQOIVVSTMKPAYAG
BQPWMEWJMUFAMTHOBNQZLKBREYAQTCERVLGJUEV
BQPWMEWJMUFAMTHOXDPPDKDJQZFAGRXLA
BQPWMEWJMUFAMTHOXDPPDKDJQZFAGOZIM
BQPWMEWJMUFAMTHOXDPPDKDJQZFZIWNAI
BQPWMEWJMUFAMTHOXCKOLXBMFJWITPZFH
BQPWMEWJMUFAMTHOXCKOLXBMFJZ
BQPWMEWJMBWOLQXONGZFWWXUXTUWEWAKWFRF
BQPWFDLKIFMGTKLXHDVBHLWDGUA
BQPWFDLKIFMGTKLXHDVBHLZIBBTI
BQPWFDLKIFMGTKLXHDCUIVLXGFWDNQCZGLTCDTM
BQPWFDLKIFMGTKLXHDCUIVLXGFWDNQCZGLAB
BQPWFDLKIFMGTKLXHDCUIVLXGFWDNQCZGLAKX
BNQNLUDGKPXIM
BNQNLUDGKAYDODMBHRGHOVV
BNQNLUDGKAYDONMKJRNPRCDT
BNLIOOQTURGTRCZNDUNTWCW
BNLIOOQTX
BNLIOOQHRWHE
BNLIOOQCLBLFFFBPTT
BNLIOOQCLBBLIKQZEXUSCXPJEQEFRJON
BNLIOOQCLBBBFJUNNYJUGPOWRMQDRXZLGZ
BNLIOOQCLBBBFJUNNYJUGPYMSPJPWJN
BNLIOOQCLBBBFJUNNYJUGGUOFTTSGYDJCICXNCRF
BNLIOOQCLBBBFJUNNYJUGGUOFTTSXQPPZ
BNLIOOQCLBBBFJUNNYJUGGUOFTTSXXMS
BNLIOOQCLBBBFJUNNXUYNXBZOYUEEMQZTEO
BNLIOOQCLBBBFJUNNXUYLNCPLUHQLUMIJPIMXS
BNLIOOQCLBBBFJUNNXUYLNCPLUHQLBPQPVRITJI
BNLIOOQCLBBBFEWJDOTB
BNLIOOQCLBXKOLJUPNTICNWKZZRDFGUZWC
BNLIOOQCLBXKOLJUPNTICNWKZZRDFGUZUB
BNLIOOQCLBXKHQV
BNLIOOEBLIDKGOAKWOSDBXGR
BNLIOOEBLIDKGOAKWBEESVLP
BNLIOOEBLIDKGOAKWBEXL
BNLIOOEBLIDKUEIUXKISCFLPZWJTI
BNLIOOEBLIDKUENVLXAVQTRTMJBTLODG
BNLIOOEBLIDKUENVLXAVQTRTMJBFBYXMBAHLJNE
BNLIOOEBXPNCTZMUMQXGECUFFTFETOOD
BNLIOOEBXTH
BEIUVMXHKIORHHVUNGBNFXUFJF
BEIUVMXHKIORHHVUNGBNFKELMHTTPKJ
BCBEVAZWDDMTIKNGOKXNZE
BCBEVAZYVQAJAYFMIIBYBNQAYIXGTQWGIWH
BCBEVAZYVQAJAYFMIIBXZNPVIOSVZYYWSAKYWAQ
BCBEVAZYVQAJAYFMIYOQQYSDKHHLZPX
BCBEVJAYLEMQY
BCXHDJAMAQKZSIORGKEPRNMTTUSRU
BCXHDJAMAQKZSIORGKEPRXXJFAIRZEL
BCXHDJAMAQKZSDIJGJYLLVRBG
BCXHWSPKPVCNNWZGCWHXKEF
BCXHWSPKPVCNNWZGCWHXKXGFTCQILXVILHLJRKZE
BCXHWSPKPVCNNWZGCWHXKKHUFTMZAYMRKVSIAU
BCXHWSPKPVCNNWZGCWHXKKHX
BCXHWUJNE
BCXHWUJNCYSFSREZOSBISJAQSUHLCKIQUDNS
BCXHWUJNCYSFSREZOSBISJAQSUHLCKIQUDKIN
BCXHWUJNCYSFSLIVBFNPZ
BCXHXBHIUQABMHBPNGOS
BCXHXBHIUQABNLPIYWTYNHDFRE
BCXHXBHIUQABNLPIYWTYNNRVGKBQIRMVVM
BCXHXBHIUQABNLPIJDMSVZTVXRL
BCXHXBHICQZGOJOUKSLWNUWT
BCXHXBHICZHOFMDTFEFGCWJAGPQMLKZTXP
BCXHXBHICBZLIOZENGZQCAIWPJYTDLEOZUAS
BCXHXBHICBZLIOZENGZQCAILUTTCNZZ
BCXHXBHICBZLIOZENGZQCAILUTTCLNOV
BCXHXBHIXIMWFQVP
BCXHXBHIXKZBXG
BCXHXBVJNAIMOOZUDQOCJDZCLFQKYMLPZ
BCXHXBVJNAIMOOZUDQOCBGYDANBZIKX
BCXHXBVJYVXIJALOOKCOOX
BCXHXBVEOQRRZQJNYHSSPVXIM
BCXHXBVEOQRRZQJNYHSVIWZTGXPCKRJUYXWYKT
BCXHXBVEOQRRZQJNYHSVIWZTGXPCKRJUUDFW
BCXHXBVEZLUNXMBWGJRPYFCOPE
BCXHXKNYSYXLSZZGIXYUYDVLV
BCXHXKNYMQW
BCXHXKNYMQNVDWK
BCXBONBZRRUQNTMGHEKHVZQESPIGCBOMN
BCXBONBZRRUQNTMGHEKHVZQESPIGCBNCAED
BCXBONBZRRUQNTMGHEKHVZQESPIGCBKOOXA
BCXBONBZRRUQNTMGHEKHVZQESFLKYAUUEARJPXQO
BCXBONBZRRUQNTMGHEKHVZQESFLKYAUX
BCXBONBZRRUQNTMJKLRANNWQKMDAVPU
BCXBONBZRRUQEWUTTQVQIJRJCNHQADNGUYWZ
BCXBFUXWKLJBKUW
BCXBFUXWKC
BCXBFUXYMNLJNVDWIQ
BCXBFUXYMNLCELAUSYTROUMKJAQPGNQSIG
BCXBFUXYMNLCELAUSEJBQEUJAHINY
BCKNFKMK
BCKNCALSSLJGUBRHJTWYNVGHDXWQHO
BCKNCALSSLJGUBRHJTWYXOQGUMWXIXEGF
BCKNCALSSLJGUBRFWQOCGGLAKZTBHJDWG
BCKNCALSSLJGUBRFWQKDPQH
BCKNCALSSLEBYXEDFOMIGERYASHPMAQTLJAHSM
BCKNCALSSLEBYXEJOVC
BCKNCALSSLXSEZUEUHOMQJFCCWL
BCKNCKOSICNGVBVRAKWZYKPQN
BCKNCKOCHOTJAUNRJZYJRDNBFHHHQSCTI
BCKNCKUDLFLTCMLJGGVQFTC
BCKNCKUDLFLTCMLJGGVQFBQGLHJSZNCOXHGCF
BCKNCKUDLFLTCMLJGGVQFBQGLHJUNIIXOVW
BCKNCKUDLFLTCMLXCH
BCKNCKUDLFLTCNZM
XPTA
XPMWZRXYQSRAFDUCAR
XPMWZRKXOMJXEJWUCGGLZIXGKVTXHUN
XPMWZRKXOMJXEJWUCGGUWQWVNP
XPMWZRKXOMJXEUBWOKASDR
XPMWZRKXOMJXEUBWHSPBQKYQVGJLOVN
XPMWZRKXOMCWYVBDFMXOVHYTAGIBPFAPAMGVCA
XPMWZRKXOMCWYVBDFMXOVHYTAGIBPFAPAMGVXKGY
XPMWZRKXOMCWYVBDFMXOVHCMXWBRJANFIVUK
XPMWZRKXOMCWYVBDFMXOVHXGSVWOM
XPMWZRKXOMCWYCIIYPVHJGDLTCPGSKBMXIKHECSJ
XPMWZRKXJDRJJLNRVMHVAJJYATFXGTZSUK
XPMWZRKXJDRJJLNRVMHVAJJYATFXGTZELE
XPMWZRKXJDRJJLNRVMHVAJJYATFXGTZKBR
XPMWZYAMUZLFCUTNMTOHMHQIZKCSBGLEARJRT
XWAVLJSHVHHMSUZXDRWNYYM
XWAVLJSHVHHMSUZXDRWNBWPF
XWAVLJSHVHHMSUZXDRWNBKPQA
XWAVLJSHVHHMSUZXDRENDNEH
XWAYPVKCEWTESFVZIJDYCGCXYATQCZ
XWAYPURXLXOMAYNQZUZZVCHLMUJCAAEG
XWAYPURXLXOMAYNQZUZNCZVZJELZPWIBIMXSYXY
XWAYPURXLXOMAYNQZUZNCZVXPSUYWYTAD
XWAYPURXLXOMAYNQZUZNCZJTXVYZLJKZSHFN
XWAYPURXLKPENONMJEXJDCOKBE
XWODEHNWGPINDB
XWOVLISYDSFIF
XWQNMOEJNBUXTCBKMVGRW
XCKHMUKJJKEUESWOVTKWVKXOJETEPDIIWDIEKAZR
XCKHMBQQYGBCQQDAPEIBCXZXEGHNLXNY
XCKHMBQQYGBCQQDAPEZLMGBMXMKAKZJZJ
XCKHMBQQYGBCCTQPVQGLLKLMGMPTDO
XCKHMBQQYGBCCTQPVQGLLKBGTUBH
XKIGFDJQBFYJICRZX
XKIGFDJQBFYKTGPAG
XKIGFDJVDQUSYNEUBBFKAMWVKNPBTWFKXCRLK
XKUVHAGKTRSDBWQSLBGOIKBRAGXVZZ
XKUVHAGKTRLLKYXHKJYOULYB
XKUVHAGKTRLLKYXHKJYOULEVGNROURCCFYLL
XKUVHAGKTRLLKYXHKJYOULEVGNROURCCFCJLUY
XKUVHAGKTRLLKYXHKJYMHCTDICDLOITLBAA
XKUVHAGKTRLLKYXHKJYMHCTQRC
XKUVHAGKTRUOMPMBMRGPSWRSYFIBEAEPLNCL
XKUVHAGKTRUOMWSTUBNCM
XKUVHAGKTRUOMWKPWLQGIQNLJOUFTXLQXLMTCV
XKUVHACSUHZGPJZBKAIRQZ
XKC
KLLKTWPBELNRAADTKVFOAUYROZHPGUPUWXXQ
KLLKTWPBELNRAANBCIKWNYIPWQCXNO
KLLKTWPKFRXGXGLMFYGKWSSAKWQWIF
KLLKTWPKFRXGXWBFPGFH
KUGEPDEFQMHYXUIUA
KUGEPDEFQMHUWCLKHGKYIAKCGHMIVNRJ
KUGEPDEFMHZQJJWITKIPZHXHNQHDEUYH
KUGEPDEFMHZQJJWITKIPZHXHNQHDEKYBOLYAH
KUGEPDEFMHZQJJWITKIPZHXHNLKVREZLVQ
KUGEPDEFMHZQJJWITKIPZHXHNLKVVZJGLXRM
KUGEPDEKEVXVC
KUBODIULZGPYQGHSCQZLHKPUBQTTBH
KUBODIULZGPYQGHSCQZLHKPUBQTZYXBBLMBK
KUBODIULZGPYQGHEIADDAXV
KUBODIULZGPYQGUOPDIVVIFTMTDFSSGM
KUBODIULZGPYQCIBMBNKIJ
KUBODIULZGPBFXXXABZYWAMJINSOKFBXSYGPDMZH
KUBODIULZGPBFXXXABZYWAMJINMVNAWYSHAIJY
KUBODIULZGPBFXXXABZYWAMJINMVFYFJHGYCQGX
KUBOMKOHGATMDXKFT
KUBOMKOHGATMDKDDQETRSKOMUAE
KUBOMKOHGATMDKDDQETVGZE
KBYFOEYETNSWPIGJRQUEUHYFWE
KBYFOEYETNSNJEJKFUG
KBYJJONBKLJTSWJGDVYKXXBEXOJCVZA
KBYJJONBKLJTSWJGDVKZHIFTDXYIXXXVASY
KBYJJONBKLJTSWJGDVKZHJORCMDDGZBXJW
KBYBBFIK
KBYBKDODIJKCESSX
KBYBKDODHFEPXIBRZBP
KKPTIZQEKIXZPIMZMPVPDBSWHHQAYXZDRKFUDDFF
KKPTIZQEKIXZPCQMOZAPKQDUFTSWVVQTHW
KKPTIZQEKIXZPCQMOZAPKQO
KKPTIZQEKIXZPCQMOZAPKQNUSGQGYBEJHZCDLJ
KKPTIZQEKIXZPCQMOZAPKKQFNFZ
KKPTIZQEKIXZPCQMOZAPKKQFNFHZAYDEIIW
KKPTIZQEKIXZPCQMOZAPKKCJZIDJEFCYPNUKL
KKEZWDJJIWCBEFHI
KKEZWDJJIJC
KKEZWULCUMAOSKHBXXHJQZWLJFLFIJDKHJUHNEOR
KKEZWULCUMAOSKHBXXHJQZWLXRTDXGSAENTJJ
KKEZWULCUMAOSKHBXXHJQZWLXRTDXS
KKEZNDP
KKEZNDZPZVHDFQEXBPWHI
KKEZNCUDABIJHSUUFJKALNWKOWROYFPHD
KKEZNCUDABIJHSUUFJKALNWKOWVNKK
KKEZNCUDKGDRHUJLBEYFQQKVFXKM
KKEZNCUDKGDRHUJLBEYFQQKJOQJRNHYNRHZFN
KKEZNCUDKGDRHUJLBEYFQQKJOQJRNHYNRL
KKUKHNBHWSTWLEUYXELKRIGJNBFDEVBZ
KKUKHNBHWSTWLEUYXELKRIGJNBFAVHSHFIRLDHI
KKUKHNBHWSTWLEUYXELKRIGJNBFAVHSHFVHH
KKXPWKYZAVCMOMLBZGAIGUP
KKXPWKYZAVKEWVDETLHZEBC
KKXSGZ
KKXSGYWHEXAELCDM
KKXSGYWJEKVXATA
KKXSGYWJLPTBVOXQWGFCCQKEVTGE
KKXSGYWJLPTBVOXQWGKRLDB
KKXSGYWJLPTBNUETFJNXHMGMBGXSAGWNFUWAU
KKXSGYWJLPTBNUETFJLUWYCWORLKSMQTWN
KKXSGYWJLPTBNUETFJLUWYBRWBUK
KKXMYWRBHKWTVYRMXAAZK
KKXMYWRBHKWTVXAAEWEVNCT
KKXMYWRBHKWTVXAAEWBDRSKY
KKXMYWRBL
KKXMYWRXETHJNBWYFOGMFAJORZHBGQEYAWGEO
KKXMYWRXETHJNBZYAJRBMCMQCNVWZVSO
KKXMYWRXETHJNBZYAJRBMCMQCNVWZVJVKVFQ
KKXMYWRXETHJNBZYAJRBMCMUDMLKX
KKXMYWRXETHJNBZEWHDOGYCUUBLT
KKXMYNNISSQOIFDASERQRCGUULFFSBJABRMTNCHB
KKXMYNNISSQOIFDASERK
KKXMYNNISSQOIFDASEOLCUXWVERCMOLYHZCA
KKXMYNNISSQOIFDASCXHFIZUOAQLD
KKXMYNNISSQOIFDASCXHFIZUOASJKDGRYCLCPR
KKXMYNCIUHTKAUMIKZXGX
KKXMYNCIUHTKAUMMOUQAQAGVJYQRZNCBP
KKXMYNCIUHTKAUMMOUQAQAGVJYQRZULSKGNSW
KKXMYNCIUHTKACZJLWXXGZEXKKDDBMAWIZ
KKXMYNCIUHTKACZJLWXXGHBKUMDCGSAQ
KKXMYNCIUHTKACZJLWXXUOXXAQM
KKXMYF
KKXMYBUOSYVOHUYDEKSQTRMXW
KKXMYBUOSYVOHUYXPHVXSCFOE
KKXMYBUOSYVOHUYXJYEW
KKXNLNBMYPNBFXSEAUBFSRVCZWC
KKXNLNBMYPNBFKSPU
KKXNLXXHQWH
KKXNLXXHQSQOMVMQBEVHORJHPGXHYRKOEHCFUVV
KKXNLXXHQSQOMVMQBEVHORJHPGXHYRKOEHCFUYH
KKXNLXXHQSQOMVMQBEVHORJHPGXVGBXQWK
KKXNLXXHQSQOMVJIRRA
KKXNLXXHQSQOMVJIRUOWQQKBCPVSI
KKXNLXXHQSQOMYJXZIPYZLGOXXHLP
KKXNLXXHQSQOMYJXZIPYZLGOXKSANGYDDW
KKXNLXXHQSQOMYJXZIPYZLGOXKHPEXSDBBS
KKXNLXXHQSQOMYJXZLUSGUSOSDKKRY", "PTIDGAWROQZSHVMNYFJELUCBXK")
            };
            //Console.WriteLine("Task1 passed: {0}", TestingTask.IsSolved(new Task1.V1(), print, task1Data));
            //Console.WriteLine("Task1 passed: {0}", TestingTask.IsSolved(new Task1.V2(), print, task1Data));

            (string, string)[] task3Data =
            {
                (@"
4
1 2 3 4
0 1 1 0
0 0 0 1
0 0 0 1
0 0 0 0", @"
0 3 4 8
0 0 0 6
0 0 0 7
0 0 0 0"),
                (@"
4
1 2 3 4
0 0 0 0
1 0 0 0
0 1 0 0
0 0 1 0", @"
0 0 0 0
3 0 0 0
6 5 0 0
10 9 7 0"),
                (@"
1
1
1", "0"),
                (@"35
52 90 75 97 94 50 34 51 4 72 29 33 44 92 15 68 83 96 30 62 47 54 62 79 80 94 12 23 1 2 12 91 60 10 33
0 1 1 1 1 1 0 1 0 0 1 1 1 1 0 0 0 1 0 1 1 1 1 0 1 0 1 0 1 1 1 0 0 1 0
0 0 1 1 0 0 0 0 0 0 1 0 0 0 0 0 0 1 0 1 0 0 0 1 0 0 0 0 0 0 0 1 0 0 0
0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0
0 0 0 0 0 0 0 0 0 0 1 0 1 0 0 0 0 0 1 0 1 0 1 1 0 0 0 0 0 0 0 0 0 1 0
0 1 1 0 0 0 0 1 0 0 1 0 1 0 0 0 0 0 1 1 1 0 0 1 0 0 0 0 0 0 0 1 0 1 0
0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
0 0 0 1 0 1 0 0 1 1 0 0 1 1 1 1 1 0 0 1 1 0 1 1 0 1 0 0 1 0 0 0 0 1 0
0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 0 0 0 1 0 0 1 0 0 0 0 0 0 0 0 0 0 1 0
0 1 1 1 1 0 0 0 1 0 0 0 1 1 1 1 0 1 0 1 1 0 0 1 0 1 1 1 0 1 0 0 1 0 0
0 0 1 0 0 1 0 1 0 0 0 0 1 0 0 0 0 0 1 0 1 0 1 1 0 0 0 0 0 0 0 1 0 0 0
0 1 0 1 1 1 1 1 1 1 0 0 0 0 0 1 1 0 1 0 0 1 1 1 0 0 0 0 1 1 1 1 1 0 1
0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0
0 0 0 1 0 1 0 0 1 0 0 0 1 0 0 0 0 1 1 0 0 0 1 1 0 0 0 0 0 0 0 1 0 1 0
0 1 0 1 1 0 0 1 0 0 1 0 0 0 0 0 0 1 0 0 1 0 1 1 1 0 0 0 0 1 0 1 0 0 0
0 1 1 1 1 0 0 0 1 0 0 0 1 0 0 0 0 0 0 1 0 0 1 1 0 0 0 0 1 0 0 1 1 1 1
0 1 1 1 0 1 0 0 1 0 1 0 0 1 1 0 0 0 1 1 0 0 0 0 1 0 0 1 1 1 0 0 0 1 0
0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 1 1 0 0 0 0 0 0 0 1 0 0 0
0 0 1 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 1 0 0 0
0 0 0 0 0 0 0 1 0 0 1 0 0 0 0 0 0 0 1 0 1 0 0 0 0 0 0 0 0 0 0 1 0 0 0
0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0
0 1 1 1 0 0 1 0 1 0 0 0 0 0 1 0 0 1 1 0 1 0 0 0 0 1 1 1 0 1 1 0 0 1 0
0 0 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 0 0 0 1 0 0 1 0 0 0 0 0 0 0 1 0 0 0
0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0
0 0 1 1 1 0 0 0 0 0 1 0 0 0 0 0 0 1 1 0 1 0 0 1 0 0 0 0 0 0 0 0 0 0 0
0 1 1 1 0 0 0 1 0 0 1 0 1 0 0 0 0 1 0 1 1 0 1 0 1 0 0 0 0 0 0 0 0 0 0
0 1 0 1 0 1 0 0 1 0 1 0 0 1 0 0 0 1 1 0 1 0 0 0 0 1 0 0 1 0 0 0 1 1 0
0 0 0 1 1 0 0 1 1 0 1 0 1 1 1 0 0 1 1 0 0 0 0 1 0 1 0 0 1 0 0 1 0 0 1
0 0 0 1 1 1 0 0 1 0 1 0 1 1 0 0 0 1 1 1 1 0 1 1 1 1 0 0 0 0 0 1 0 0 0
0 1 1 1 0 1 0 0 0 0 0 0 1 1 0 0 0 1 0 1 1 0 0 0 0 1 0 0 0 0 0 0 0 0 0
0 1 0 1 1 1 0 1 0 0 0 0 1 0 0 1 0 1 0 0 1 0 0 1 1 1 0 1 0 0 0 0 1 1 1
0 0 0 0 0 1 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
0 1 1 1 0 1 0 1 0 0 0 0 1 1 0 0 0 1 1 0 0 0 0 0 0 1 0 0 1 0 0 1 0 0 0
0 0 1 0 0 1 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0
0 1 0 1 1 0 0 0 0 0 1 0 1 1 1 0 0 0 1 1 1 0 1 1 0 0 0 0 0 0 0 1 0 1 0", @"0 732 995 829 642 1215 173 1216 470 245 858 85 964 466 361 313 256 828 888 794 967 139 920 1074 548 468 257 279 374 363 151 1165 373 839 346
0 0 353 187 0 573 0 574 0 0 216 0 322 0 0 0 0 186 246 152 325 0 278 432 0 0 0 0 0 0 0 523 0 197 0
0 0 0 0 0 295 0 296 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 154 0 0 0 0 0 0 0 245 0 0 0
0 0 263 0 0 483 0 484 0 0 126 0 232 0 0 0 0 0 156 0 235 0 188 342 0 0 0 0 0 0 0 433 0 107 0
0 184 447 281 0 667 0 668 0 0 310 0 416 0 0 0 0 280 340 246 419 0 372 526 0 0 0 0 0 0 0 617 0 291 0
0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
0 593 856 690 503 1076 0 1077 331 106 719 0 825 327 222 174 117 689 749 655 828 0 781 935 409 329 118 140 235 224 0 1026 234 700 207
0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
0 94 357 191 0 577 0 578 0 0 220 0 326 0 0 0 0 190 250 156 329 0 282 436 0 0 0 0 0 0 0 527 0 201 0
0 559 822 656 469 1042 0 1043 297 0 685 0 791 293 188 140 0 655 715 621 794 0 747 901 375 295 84 95 201 190 0 992 200 666 173
0 0 166 0 0 386 0 387 0 0 0 0 135 0 0 0 0 0 59 0 138 0 91 245 0 0 0 0 0 0 0 336 0 0 0
0 680 943 777 590 1163 121 1164 418 193 806 0 912 414 309 261 204 776 836 742 915 87 868 1022 496 416 205 227 322 311 99 1113 321 787 294
0 0 0 0 0 185 0 186 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 135 0 0 0
0 186 449 283 0 669 0 670 96 0 312 0 418 0 0 0 0 282 342 248 421 0 374 528 0 0 0 0 0 0 0 619 0 293 0
0 375 638 472 285 858 0 859 113 0 501 0 607 109 0 0 0 471 531 437 610 0 563 717 191 111 0 0 0 17 0 808 0 482 0
0 487 750 584 397 970 0 971 225 0 613 0 719 221 116 0 0 583 643 549 722 0 675 829 303 223 0 0 129 118 0 920 128 594 101
0 514 777 611 424 997 0 998 252 0 640 0 746 248 154 0 0 610 670 576 749 0 702 856 330 250 0 106 107 156 0 947 0 621 139
0 0 233 0 0 453 0 454 0 0 0 0 202 0 0 0 0 0 0 0 205 0 158 312 0 0 0 0 0 0 0 403 0 0 0
0 0 105 0 0 325 0 326 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 184 0 0 0 0 0 0 0 275 0 0 0
0 0 228 0 0 448 0 449 0 0 91 0 197 0 0 0 0 0 121 0 200 0 153 307 0 0 0 0 0 0 0 398 0 0 0
0 0 0 0 0 267 0 268 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 126 0 0 0 0 0 0 0 217 0 0 0
0 647 910 744 557 1130 88 1131 385 160 773 0 879 381 276 228 171 743 803 709 882 0 835 989 463 383 172 194 289 278 66 1080 288 754 261
0 0 137 0 0 357 0 358 0 0 0 0 106 0 0 0 0 0 0 0 109 0 0 216 0 0 0 0 0 0 0 307 0 0 0
0 0 0 0 0 220 0 221 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 170 0 0 0
0 264 527 361 174 747 0 748 0 0 390 0 496 0 0 0 0 360 420 326 499 0 452 606 0 0 0 0 0 0 0 697 0 371 0
0 358 621 455 268 841 0 842 0 0 484 0 590 0 0 0 0 454 514 420 593 0 546 700 174 0 0 0 0 0 0 791 0 465 0
0 431 694 528 341 914 0 915 169 0 557 0 663 165 0 0 0 527 587 493 666 0 619 773 247 167 0 0 73 0 0 864 72 538 0
0 431 694 528 341 914 0 915 169 0 557 0 663 165 71 0 0 527 587 493 666 0 619 773 247 167 0 0 24 73 0 864 0 538 56
0 359 622 456 269 842 0 843 97 0 485 0 591 93 0 0 0 455 515 421 594 0 547 701 175 95 0 0 0 0 0 792 0 466 0
0 360 623 457 270 843 0 844 98 0 486 0 592 94 0 0 0 456 516 422 595 0 548 702 176 96 0 0 0 0 0 793 0 467 0
0 499 762 596 409 982 0 983 237 0 625 0 731 233 128 80 0 595 655 561 734 0 687 841 315 235 0 35 141 130 0 932 140 606 113
0 0 0 0 0 141 0 142 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
0 419 682 516 329 902 0 903 157 0 545 0 651 153 0 0 0 515 575 481 654 0 607 761 235 155 0 0 61 0 0 852 0 526 0
0 0 85 0 0 305 0 306 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 164 0 0 0 0 0 0 0 255 0 0 0
0 408 671 505 318 891 0 892 146 0 534 0 640 142 48 0 0 504 564 470 643 0 596 750 224 144 0 0 0 50 0 841 0 515 0")
            };
            Console.WriteLine("Task3 passed: {0}", TestingTask.IsSolved(new Task3.V2(), print, task3Data));
            Console.WriteLine("Task3 passed: {0}", TestingTask.IsSolved(new Task3.V1(), print, task3Data));

            Console.ReadLine();

            return 0;
            string? bfsrc = @"";
            Expression<Action> lambda = BrainFuck.LambdaSource(
                bfsrc, true);

            //Program.print(lambda);
            Action? wtf = lambda.Compile();
            wtf();
            return 0;
            Console.ReadLine();
        start:
            Application.EnableVisualStyles();
            new RegexSolver().ShowDialog();
            //Parallel.For(0, int.MaxValue, TestDigitCount);
            //for (int i = 0; i < int.MaxValue; i++) TestDigitCount(i);
            //nums.MainTest();
            Console.WriteLine(" --end program-- ");
            //Console.ReadLine();
            return 0;
        }

        private static void write(params object[] array)
            => Console.Write(string.Concat(array));

        private static void writeln(params object[] array)
            => Console.WriteLine(string.Concat(array));

        private static string readlexem()
        {
            StringBuilder buf = new StringBuilder();
            char x;
            while (char.IsWhiteSpace(x = (char)Console.Read()))
                ;
            buf.Append(x);
            while (!char.IsWhiteSpace(x = (char)Console.Read()))
                buf.Append(x);
            return buf.ToString();
        }

        private static unsafe void read_t<T>(in T a)
            where T : unmanaged
        {
            fixed (T* p = &a)
            {
                *p = (T)Convert
      .ChangeType(readlexem(), typeof(T));
            }
        }

        private static void read<A>(in A a)
            where A : unmanaged => read_t(a);

        private static void read<A, B>(in A a, in B b)
            where A : unmanaged
            where B : unmanaged
        {
            read_t(a);
            read_t(b);
        }

        private static void read<A, B, C>(in A a, in B b, in C c)
            where A : unmanaged
            where B : unmanaged
            where C : unmanaged
        {
            read_t(a);
            read_t(b);
            read_t(c);
        }

        private class StringContainear
        {
            private readonly string str;
            private readonly List<int> list;
            private readonly object locker;
            private readonly int length, max;
            public StringContainear(string str, List<int> list, object locker, int max = int.MaxValue) => (this.str, this.list, this.locker, this.max, length) = (str, list, locker, max, str.Length);
            public void Invoke(int seed)
            {
                Random random = new Random(seed);
                for (int i = 0; i < length; i++)
                {
                    if (random.Next(max) != str[i])
                        return;
                }

                lock (locker)
                    list.Add(seed);
            }
        }

        public static int[] GetSeedByStr(string str)
        {
            List<int> list = new List<int>();
            object obj = new object();
            StringContainear q = new StringContainear(str, list, obj);
            ParallelLoopResult hndl = Parallel.For(0, int.MaxValue, q.Invoke);
            while (!hndl.IsCompleted)
                Thread.Sleep(1000);
            return list.ToArray();
        }

        private static double[] convert(double value, double radix)
        {
            List<double> list = new List<double>();
            do
            {
                double tmp = value % radix;
                list.Add(tmp);
                value = (value - tmp) / radix;
            }
            while (value != 0 && list.Count < 15);
            list.Reverse();
            return list.ToArray();
        }

        private static List<string> combine(string Str)
        {
            List<string> list = new List<string>();
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

        private static int gray(int g) // Код Грея
=> g ^ g >> 1;
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
        public static string strrange(char v, int before, int after)
        {
            StringBuilder sb = new StringBuilder();
            after += v;
            v -= (char)before;
            /*for (char x = v; x <= after; x++)
            {
                sb.Append(x);
            }*/
            while (v < after)
                sb.Append(v++);
            return sb.ToString();
        }
    }
}