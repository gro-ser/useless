using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace useless
{
    using Enum = System.Enum;
    public interface IOperator<T>
    {
        T sum(T a, T b);// +
        T sub(T a, T b);// -
        T mul(T a, T b);// *
        T div(T a, T b);// /
        T mod(T a, T b);// %
    }
    public interface IConvert<T>
    {
        T convert(string str);
        T convert(IConvertible value);
    }
    public interface IComparator<T>
    {
        bool less(T a, T b);
        bool equal(T a, T b);
        bool greater(T a, T b);
    }
    public abstract class VeryMath<T> : IOperator<T>, IConvert<T>, IComparator<T>
    {
        public abstract T convert(IConvertible value);
        public abstract T convert(string str);
        public abstract bool equal(T a, T b);
        public abstract bool greater(T a, T b);
        public abstract bool less(T a, T b);
        public abstract T sum(T a, T b);
        public abstract T sub(T a, T b);
        public abstract T mul(T a, T b);
        public abstract T div(T a, T b);
        public abstract T mod(T a, T b);
        public virtual T zero => default(T);

        static Dictionary<Type, object>
            dic = new Dictionary<Type, object>()
        {
            { typeof(int), new IntCalc() },
            { typeof(long), new LongCalc() },
            { typeof(float), new FloatCalc() },
            { typeof(double), new DoubleCalc() },
            { typeof(decimal), new DecimalCalc() },
            { typeof(BigInteger), new BigIntegerCalc() }
        };

        public static VeryMath<T> GetDefault()
        {
            if (!dic.ContainsKey(typeof(T))) return null;
            return dic[typeof(T)] as VeryMath<T>;
        }
    }
    public class IntCalc : VeryMath<int>
    {
        public override int convert(IConvertible value) => value.ToInt32(null);
        public override int convert(string str) => int.Parse(str);

        public override bool equal(int a, int b) => a == b;
        public override bool greater(int a, int b) => a > b;
        public override bool less(int a, int b) => a < b;

        public override int sum(int a, int b) => a + b;
        public override int sub(int a, int b) => a - b;
        public override int mul(int a, int b) => a * b;
        public override int div(int a, int b) => a / b;
        public override int mod(int a, int b) => a % b;
    }
    public class LongCalc : VeryMath<long>
    {
        public override long convert(IConvertible value) => value.ToInt64(null);
        public override long convert(string str) => long.Parse(str);

        public override bool equal(long a, long b) => a == b;
        public override bool greater(long a, long b) => a > b;
        public override bool less(long a, long b) => a < b;

        public override long sum(long a, long b) => a + b;
        public override long sub(long a, long b) => a - b;
        public override long mul(long a, long b) => a * b;
        public override long div(long a, long b) => a / b;
        public override long mod(long a, long b) => a % b;
    }
    public class FloatCalc : VeryMath<float>
    {
        public override float convert(IConvertible value) => value.ToSingle(null);
        public override float convert(string str) => float.Parse(str);

        public override bool equal(float a, float b) => a == b;
        public override bool greater(float a, float b) => a > b;
        public override bool less(float a, float b) => a < b;

        public override float sum(float a, float b) => a + b;
        public override float sub(float a, float b) => a - b;
        public override float mul(float a, float b) => a * b;
        public override float div(float a, float b) => a / b;
        public override float mod(float a, float b) => a % b;
    }
    public class DoubleCalc : VeryMath<double>
    {
        public override double convert(IConvertible value) => value.ToDouble(null);
        public override double convert(string str) => double.Parse(str);

        public override bool equal(double a, double b) => a == b;
        public override bool greater(double a, double b) => a > b;
        public override bool less(double a, double b) => a < b;

        public override double sum(double a, double b) => a + b;
        public override double sub(double a, double b) => a - b;
        public override double mul(double a, double b) => a * b;
        public override double div(double a, double b) => a / b;
        public override double mod(double a, double b) => a % b;
    }
    public class DecimalCalc : VeryMath<decimal>
    {
        public override decimal convert(IConvertible value) => value.ToDecimal(null);
        public override decimal convert(string str) => decimal.Parse(str);

        public override bool equal(decimal a, decimal b) => a == b;
        public override bool greater(decimal a, decimal b) => a > b;
        public override bool less(decimal a, decimal b) => a < b;

        public override decimal sum(decimal a, decimal b) => a + b;
        public override decimal sub(decimal a, decimal b) => a - b;
        public override decimal mul(decimal a, decimal b) => a * b;
        public override decimal div(decimal a, decimal b) => a / b;
        public override decimal mod(decimal a, decimal b) => a % b;
    }
    public class BigIntegerCalc : VeryMath<BigInteger>
    {
        public override BigInteger convert(IConvertible value) =>
            BigInteger.Parse(value.ToString());
        public override BigInteger convert(string str) => BigInteger.Parse(str);

        public override bool equal(BigInteger a, BigInteger b) => a == b;
        public override bool greater(BigInteger a, BigInteger b) => a > b;
        public override bool less(BigInteger a, BigInteger b) => a < b;

        public override BigInteger sum(BigInteger a, BigInteger b) => a + b;
        public override BigInteger sub(BigInteger a, BigInteger b) => a - b;
        public override BigInteger mul(BigInteger a, BigInteger b) => a * b;
        public override BigInteger div(BigInteger a, BigInteger b) => a / b;
        public override BigInteger mod(BigInteger a, BigInteger b) => a % b;
    }

    public class EnumCalc<T> : VeryMath<T> where T : Enum
    {
        public override T convert(IConvertible value) => (T)Enum.ToObject(typeof(T), value);
        public override T convert(string str) => (T)Enum.Parse(typeof(T), str, true);

        public override bool equal(T a, T b) => a.CompareTo(b) == 0;
        public override bool greater(T a, T b) => a.CompareTo(b) > 0;
        public override bool less(T a, T b) => a.CompareTo(b) < 0;

        static long l(T x) => Convert.ToInt64(x);
        static T t(long x) => (T)Enum.ToObject(typeof(T), x);
        public override T sum(T a, T b) => t(l(a) + l(b));
        public override T sub(T a, T b) => t(l(a) - l(b));
        public override T mul(T a, T b) => t(l(a) * l(b));
        public override T div(T a, T b) => t(l(a) / l(b));
        public override T mod(T a, T b) => t(l(a) % l(b));
    }

    public static class VeryMath
    {
        const string @class = @"using System;
using {1};
class {0}Calculator : SafeVeryMath<{0}>
{{
{2}}}";
        // Format(<classname>, <namespace>, <operators>)
        const string method
            //= "\tpublic override {0} {1}({2} a, {2} b) => a {3} b;\r\n";
            = "static public num operator {3} (num a, num b) => new num(a.self {3} b.self);\r\n";
        // Format(<ret.type>, <opname>, <<T>>, <operator>)
        static Dictionary<Type, object>
    dic = new Dictionary<Type, object>()
{
            { typeof(int), new IntCalc() },
            { typeof(long), new LongCalc() },
            { typeof(float), new FloatCalc() },
            { typeof(double), new DoubleCalc() },
            { typeof(decimal), new DecimalCalc() },
            { typeof(BigInteger), new BigIntegerCalc() }
};

        const int operatorsCout = 8;
        static (string Name, string Method, string Operator)[]
            methodInfo = new (string,string,string)[operatorsCout]
        {
            ("op_Addition",     "sum",      "+"),
            ("op_Subtraction",  "sub",      "-"),
            ("op_Multiply",     "mul",      "*"),
            ("op_Division",     "div",      "/"),
            ("op_Modulus",      "mod",      "%"),
            ("op_Equality",     "equal",    "=="),
            ("op_LessThan",     "less",     "<"),
            ("op_GreaterThan",  "greater",  ">")
        };

        public static VeryMath<T> GetDefault<T>()
        {
            if (!dic.ContainsKey(typeof(T))) return CompileNew<T>();
            return dic[typeof(T)] as VeryMath<T>;
        }

        public static VeryMath<T> CompileNew<T>()
        {
            var sb = new StringBuilder();
            var type = typeof(T);
            var types = new Type[] { type, type };

            for (int i = 0; i < operatorsCout; i++)
            {
                var met = type.GetMethod(methodInfo[i].Name, types);
                if (met == null) continue;
                sb.AppendFormat(method, met.ReturnType.Name,
                    methodInfo[i].Method, type.Name, methodInfo[i].Operator);
            }
            var nspace = type.Namespace;
            if (string.IsNullOrWhiteSpace(nspace)) nspace = "System";
            var newClassCode = string.Format(@class, type.Name, nspace, sb.ToString());
            File.WriteAllText(type.Name + "Calculator.cs", newClassCode);
            return null;
            //newClassCode= nameof(VeryMath<T>.sum);
        }
    }


    public static class Tests
    {
        public static T Fibonacci<T>(int n, VeryMath<T> calc = null)
        {
            if (calc == null) calc = VeryMath<T>.GetDefault();
            T c, b = calc.convert(1), a = calc.convert(0);
            while (n-- > 0)
            {
                c = a; a = calc.sum(a, b); b = c;
            }
            return a;
        }
        public static T Max<T>(IComparator<T> comp, T a, T b) => comp.greater(a, b) ? a : b;
        public static T Sum<T>(IOperator<T> op, params T[] arr) => Sum<T>(op, arr, arr.Length - 1);
        static T Sum<T>(IOperator<T> op, T[] arr, int len) => len == 0 ? arr[0] :
            op.sum(arr[len], Sum<T>(op, arr, len - 1));

        public static void WTF()
        {
            VeryMath<int> vm;
            var sw = Stopwatch.StartNew();
            vm = new IntCalc();
            Console.WriteLine("IntCalc()    = {0}", sw.ElapsedTicks); sw.Restart();
            vm = new IntCalc();
            Console.WriteLine("IntCalc()    = {0}", sw.ElapsedTicks); sw.Restart();
            vm = VeryMath<int>.GetDefault();
            Console.WriteLine("GetDefault() = {0}", sw.ElapsedTicks); sw.Restart();
            vm = VeryMath<int>.GetDefault();
            Console.WriteLine("GetDefault() = {0}", sw.ElapsedTicks); sw.Restart();
            new Calculator<int>();
            Console.WriteLine("Calculator() = {0}", sw.ElapsedTicks); sw.Restart();
            new Calculator<float>();
            Console.WriteLine("Calculator() = {0}", sw.ElapsedTicks); sw.Restart();
        }
        static (string, string) wtf(string str)
        {
            string res = "", abc = new string(str.OrderBy(z => z).Distinct().ToArray());
            int length = str.Length;
            for (int i = 0; i < length; i++) res += abc.IndexOf(str[i]) + ", ";
            return (abc, res);
        }
    }
}