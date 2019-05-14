using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _
{
    public class MyDouble
    {
        double[] arr;
        int length;
        public int Length => length;

        MyDouble(double[] ar, int len) { arr = ar; length = len; }
        public MyDouble(params double[] ar):this((double[])ar.Clone(),ar.Length) { }
        public MyDouble(double val) : this(new[] { val }) { }
        public MyDouble(int count, double val)
        {
            arr = new double[length = count];
            while (count-- > 0) arr[count] = val;
        }

        public double Min
        {
            get
            {
                if (length == 0) throw new Exception("len=0");
                double ans = arr[0];
                for (int i = 1; i < length; i++)
                    if (arr[i] < ans) ans = arr[i];
                return ans;
            }
        }
        public double Max
        {
            get
            {
                if (length == 0) throw new Exception("len=0");
                double ans = arr[0];
                for (int i = 1; i < length; i++)
                    if (arr[i] > ans) ans = arr[i];
                return ans;
            }
        }

        public static implicit operator MyDouble(double val) => new MyDouble(val);
        public static implicit operator MyDouble(double[] ar) => new MyDouble(ar);

        static double sum(double a, double b) => a + b;
        static double sub(double a, double b) => a - b;
        static double mul(double a, double b) => a * b;
        static double div(double a, double b) => a / b;
        static double mod(double a, double b) => a % b;

        private static MyDouble Iter(MyDouble a, MyDouble b, Func<double, double, double> f)
        {
            double[] arr = new double[a.length * b.length];
            for (int ia = 0, i = 0; ia < a.length; ia++)
                for (int ib = 0; ib < b.length; ib++, i++)
                    arr[i] = f(a.arr[ia], b.arr[ib]);
            return new MyDouble(arr, arr.Length);
        }

        public static MyDouble operator +(MyDouble a, MyDouble b) => Iter(a, b, sum);
        public static MyDouble operator -(MyDouble a, MyDouble b) => Iter(a, b, sub);
        public static MyDouble operator *(MyDouble a, MyDouble b) => Iter(a, b, mul);
        public static MyDouble operator /(MyDouble a, MyDouble b) => Iter(a, b, div);
        public static MyDouble operator %(MyDouble a, MyDouble b) => Iter(a, b, mod);

        public static MyDouble Sqrt(MyDouble v)
        {
            var arr = new double[v.length * 2];
            for (int i = 0; i < v.length; i++)
            {
                var t = Math.Sqrt(v.arr[i]);
                arr[2 * i] = -t;
                arr[2 * i + 1] = t;
            }
            return new MyDouble(arr, arr.Length);
        }
        
        public static MyDouble operator +(MyDouble v) => new MyDouble(v.arr);
        public static MyDouble operator -(MyDouble v)
        {
            double[] tmp = new double[v.length];
            for (int i = v.length - 1; i >= 0; i--) tmp[i] = -v.arr[i];
            return new MyDouble(tmp, tmp.Length);
        }

        public override string ToString() => $"({string.Join("; ", arr)})";
    }
}
