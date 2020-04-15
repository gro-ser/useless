using System;
using System.Diagnostics;
using System.Numerics;

namespace useless
{
    internal class nums
    {
        private static readonly BigInteger[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
        public static BigInteger exp_ez(BigInteger x, BigInteger y)
        {
            if (x == 0)
                return -1;
            BigInteger pow = 0;
            while (x % y == 0)
            {
                x /= y;
                pow++;
            }
            return pow;
        }
        public static BigInteger exp_ez(BigInteger x, int n) => exp_ez(x, primes[n]);
        public static BigInteger exp_my(BigInteger x, BigInteger y)
        {
            const int len = 7;
            if (x == 0)
                return -1;
            BigInteger[] pows = new BigInteger[len];
            int ipow = 0;
            pows[0] = y;
            for (int i = 1; i < len; ++i)
            {
                BigInteger lst = pows[i - 1];
                BigInteger tmp = lst * lst;
                tmp = tmp * tmp;
                if (tmp > x)
                    break;
                ++ipow;
                pows[i] = tmp;
            }
            BigInteger pow = 0, div;
            do
            {
                div = pows[ipow];
                if (x % div == 0)
                {
                    pow += 1 << (ipow << 1);
                    x /= div;
                }
                else
                {
                    --ipow;
                }
            }
            while (ipow >= 0);
            return pow;
        }
        public static BigInteger exp_my(BigInteger x, int n) => exp_my(x, primes[n]);
        public static void MainTest()
        {
            const int length = 500_000;
            Stopwatch sw = new Stopwatch();

            expTest(length, sw, (BigInteger)Math.Pow(2 * 3, 2), "small");
            expTest(length, sw, (BigInteger)Math.Pow(2 * 3, 8), "medium");
            expTest(length, sw, (BigInteger)Math.Pow(2 * 3, 24), "large");
            //expTest(length, sw, BigInteger.Pow(6, 64000), "big");
        }
        private static void expTest(int length, Stopwatch sw, BigInteger test, string category)
        {
            const int mod = 12;
            Console.WriteLine($"\nTest on {category} numbers");
            sw.Restart();
            for (int i = 0; i < length; i++)
                exp_ez(test, i % mod);
            Console.WriteLine("exp_ez() => {0}", sw.Elapsed);
            sw.Restart();
            for (int i = 0; i < length; i++)
                exp_my(test, i % mod);
            Console.WriteLine("exp_my() => {0}", sw.Elapsed);
        }
    }
}