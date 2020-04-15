using System;

namespace useless
{
    internal class ComposeCreater
    {
        private static Func<X, Z> BadCompose<X, Y, Z>(Func<X, Y> f, Func<Y, Z> g) => x => g(f(x));

        private class DoubleCompose<X, Y, Z>
        {
            private readonly Func<X, Y> F;
            private readonly Func<Y, Z> G;
            public DoubleCompose(Func<X, Y> f, Func<Y, Z> g)
            { F = f; G = g; }
            public Z Calculate(X x) => G(F(x));
        }
        private class MultiCompose<T>
        {
            private readonly Func<T, T>[] array;
            internal MultiCompose(Func<T, T>[] Array)
            => array = Array;
            internal T Calculate(T t)
            {
                for (int i = 0, length = array.Length; i < length; i++)
                    t = array[i](t);
                return t;
            }
        }

        private static Func<X, Z> Compose<X, Y, Z>(Func<X, Y> f, Func<Y, Z> g)
            => new DoubleCompose<X, Y, Z>(f, g).Calculate;
        private static Func<T, T> Compose<T>(params Func<T, T>[] array)
            => new MultiCompose<T>(array).Calculate;

        private static Func<X, Z> DelegateCompose<X, Y, Z>(Func<X, Y> f, Func<Y, Z> g)
            => (Func<X, Z>)Delegate.Combine(g, f);

        private static float G(float x)
        {
            Console.WriteLine("G({0})", x);
            return x + 1;
        }

        private static float F(float y)
        {
            Console.WriteLine("F({0})", y);
            return y * y;
        }

        private static void Test()
        {
            Func<float, float> t1 = Compose<float, float, float>(F, G);
            Func<float, float> t2 = BadCompose((Func<float, float>)F, G);
            Func<float, float> t3 = DelegateCompose((Func<float, float>)G, F);
            //t1 = t2 = t => t;
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("i:{0}\tf:{1}\tg:{2}\n\tt1:{3}\n\tt2:{4}\n\tt3:{5}", i, F(i), G(i), t1(i), t2(i), t3(i));
            }
        }
    }
}
