using System;

public abstract class Num : IFormattable
{
    public abstract int Value { get; }

    public override string ToString()
        => Value.ToString();

    public string ToString(string format, IFormatProvider formatProvider)
        => Value.ToString(format, formatProvider);

    #region operators

    public static Num operator -(Num num) => (Num)Activator.CreateInstance(
        typeof(Neg<>).MakeGenericType(num.GetType()));

    public static Num operator +(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Add<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator -(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Sub<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator *(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Mul<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator /(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Div<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator %(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Mod<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator &(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(And<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator |(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Or<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator ^(Num l, Num r) => (Num)Activator.CreateInstance(
        typeof(Xor<,>).MakeGenericType(l.GetType(), r.GetType()));

    public static Num operator <<(Num l, int r) => (Num)Activator.CreateInstance(
        typeof(Shl<,>).MakeGenericType(l.GetType(), GetTypeForNum(r)));

    public static Num operator >>(Num l, int r) => (Num)Activator.CreateInstance(
        typeof(Shr<,>).MakeGenericType(l.GetType(), GetTypeForNum(r)));

    #endregion

    private static Type GetTypeForNum(int num)
    {
        if (num == int.MinValue)
            return typeof(_MinValue);
        if (num == int.MaxValue)
            return typeof(_MaxValue);
        if (num < 0)
            return typeof(Neg<>).MakeGenericType(GetTypeForNum(-num));
        switch (num)
        {
            case 0x0:
                return typeof(_0);
            case 0x1:
                return typeof(_1);
            case 0x2:
                return typeof(_2);
            case 0x3:
                return typeof(_3);
            case 0x4:
                return typeof(_4);
            case 0x5:
                return typeof(_5);
            case 0x6:
                return typeof(_6);
            case 0x7:
                return typeof(_7);
            case 0x8:
                return typeof(_8);
            case 0x9:
                return typeof(_9);
            case 0xa:
                return typeof(_a);
            case 0xb:
                return typeof(_b);
            case 0xc:
                return typeof(_c);
            case 0xd:
                return typeof(_d);
            case 0xe:
                return typeof(_e);
            case 0xf:
                return typeof(_f);
        }
        int i,
            min = Math.Max(16, num - 15),
            max = num > int.MaxValue - 15 ? int.MaxValue : num + 15;
        for (i = min; i <= max; ++i)
        {
            double log = Math.Log(i, 2);
            if ((int)log == log)
            {
                Type pow = typeof(Shl<,>).MakeGenericType(typeof(_1), GetTypeForNum((int)log));
                i -= num;
                if (i > 0)
                    return typeof(Sub<,>).MakeGenericType(pow, GetTypeForNum(i));
                if (i < 0)
                    return typeof(Add<,>).MakeGenericType(pow, GetTypeForNum(-i));
                return pow;
            }
        }
        return typeof(JoinHex<,>).MakeGenericType(GetTypeForNum(num >> 4), GetTypeForNum(num & 15));
    }

    public static Num Create(int num) => (Num)Activator.CreateInstance(GetTypeForNum(num));

    public Num Simplify() => Create(Value);
}

public class _0 : Num { public override int Value => 0x0; }
public class _1 : Num { public override int Value => 0x1; }
public class _2 : Num { public override int Value => 0x2; }
public class _3 : Num { public override int Value => 0x3; }
public class _4 : Num { public override int Value => 0x4; }
public class _5 : Num { public override int Value => 0x5; }
public class _6 : Num { public override int Value => 0x6; }
public class _7 : Num { public override int Value => 0x7; }
public class _8 : Num { public override int Value => 0x8; }
public class _9 : Num { public override int Value => 0x9; }
public class _a : Num { public override int Value => 0xa; }
public class _b : Num { public override int Value => 0xb; }
public class _c : Num { public override int Value => 0xc; }
public class _d : Num { public override int Value => 0xd; }
public class _e : Num { public override int Value => 0xe; }
public class _f : Num { public override int Value => 0xf; }

public class _MinValue : Num { public override int Value => int.MinValue; }
public class _MaxValue : Num { public override int Value => int.MaxValue; }

public class Neg<T> : Num
    where T : Num, new()
{
    public override int Value => -new T().Value;
}

public class Or<Left, Right> : Num // |
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value | new Right().Value; }

public class And<Left, Right> : Num // &
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value & new Right().Value; }

public class Xor<Left, Right> : Num // ^
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value ^ new Right().Value; }

public class Shl<Left, Right> : Num // <<
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value << new Right().Value; }

public class Shr<Left, Right> : Num // >>
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value >> new Right().Value; }

public class Add<Left, Right> : Num // +
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value + new Right().Value; }

public class Mul<Left, Right> : Num // *
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value * new Right().Value; }

public class Sub<Left, Right> : Num // -
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value - new Right().Value; }

public class Div<Left, Right> : Num // /
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value / new Right().Value; }

public class Mod<Left, Right> : Num // %
    where Left : Num, new() where Right : Num, new()
{ public override int Value => new Left().Value % new Right().Value; }

public class JoinHex<Left, Right> : Or<Shl<Left, _4>, Right>
    where Left : Num, new() where Right : Num, new()
{ }

public class JoinDec<Left, Right> : Add<Mul<Left, _a>, Right>
    where Left : Num, new() where Right : Num, new()
{ }
