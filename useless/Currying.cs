using System;

internal delegate TResult Curried<in T1, out TResult>(T1 arg);

internal delegate Curried<T2, TR>
    Curried<in T1, in T2, out TR>(T1 arg);

internal delegate Curried<T2, T3, TR>
    Curried<in T1, in T2, in T3, out TR>(T1 arg);

internal delegate Curried<T2, T3, T4, TR>
    Curried<in T1, in T2, in T3, in T4, out TR>(T1 arg);

internal delegate Curried<T2, T3, T4, T5, TR>
    Curried<in T1, in T2, in T3, in T4, in T5, out TR>(T1 arg);

internal delegate Curried<T2, T3, T4, T5, T6, TR>
    Curried<in T1, in T2, in T3, in T4, in T5, in T6, out TR>(T1 arg);

internal delegate Curried<T2, T3, T4, T5, T6, T7, TR>
    Curried<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TR>(T1 arg);

internal delegate Curried<T2, T3, T4, T5, T6, T7, T8, TR>
    Curried<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TR>(T1 arg);

public class Currying
{
    // normalized method signature
    private static int[] CreateRange_normal
        (int start, int step, int count)
    {
        int[] result = new int[count];
        for (int i = 0, value = start; i < count; i++, value += step)
            result[i] = value;
        return result;
    }

    // curried method using Func<>
    private static Func<int, Func<int, int[]>> CreateRange_func
        (int start) => (int step) => (int count) =>
        {
            int[] result = new int[count];
            for (int i = 0, value = start; i < count; i++, value += step)
                result[i] = value;
            return result;
        };

    // curried method using Curried<>
    private static Curried<int, int, int[]> CreateRange_curry
        (int start) => (int step) => (int count) =>
        {
            int[] result = new int[count];
            for (int i = 0, value = start; i < count; i++, value += step)
                result[i] = value;
            return result;
        };

    private static void Print(int[] arr)
    {
        foreach (int x in arr)
            Console.Write("{0} ", x);
        Console.WriteLine();
    }

    public static void Main()
    {
        Console.WriteLine("{0} example:", nameof(CreateRange_normal));
        {
            Print(CreateRange_normal(0, 1, 5));

            Func<int, int, int[]> startsWith2 = (int step, int count)
                => CreateRange_normal(2, step, count);
            Print(startsWith2(0, 3));

            Func<int, int[]> evenNums = (int count)
                => startsWith2(2, count);
            Print(evenNums(3));
            Print(evenNums(5));
        }

        Console.WriteLine("{0} example:", nameof(CreateRange_func));
        {
            Print(CreateRange_func(0)(1)(5));

            Func<int, Func<int, int[]>> startsWith2 = CreateRange_func(2);
            Print(startsWith2(0)(3));

            Func<int, int[]> evenNums = startsWith2(2);
            Print(evenNums(3));
            Print(evenNums(5));
        }

        Console.WriteLine("{0} example:", nameof(CreateRange_curry));
        {
            Print(CreateRange_curry(0)(1)(5));

            Curried<int, int, int[]> startsWith2 = CreateRange_curry(2);
            Print(startsWith2(0)(3));

            Curried<int, int[]> evenNums = startsWith2(2);
            Print(evenNums(3));
            Print(evenNums(5));
        }
    }
}