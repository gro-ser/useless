using System;
using System.Numerics;

namespace _
{
    public abstract class Number
    {
        public abstract Number Add(Number num);
        public abstract Number Sub(Number num);
        public abstract Number Mul(Number num);
        public abstract Number Div(Number num);
        public abstract Number Mod(Number num);

        public virtual Number AddTo(Number num) => throw new NotImplementedException();
        public virtual Number SubTo(Number num) => throw new NotImplementedException();
        public virtual Number MulTo(Number num) => throw new NotImplementedException();
        public virtual Number DivTo(Number num) => throw new NotImplementedException();
        public virtual Number ModTo(Number num) => throw new NotImplementedException();

        static public Number operator +(Number left, Number right) => left.Add(right);
        static public Number operator -(Number left, Number right) => left.Sub(right);
        static public Number operator *(Number left, Number right) => left.Mul(right);
        static public Number operator /(Number left, Number right) => left.Div(right);
        static public Number operator %(Number left, Number right) => left.Mod(right);
    }

    public sealed class IntegerNumber : Number
    {
        long value;
        public long Value => value;
        public IntegerNumber(long x) { value = x; }
        public static implicit operator IntegerNumber(long value)
            => new IntegerNumber(value);

        public override Number Add(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n:
                    try { return new IntegerNumber(checked(value + n.value)); }
                    catch { return new BigNumber(checked(value + n.value)); }
                case RealNumber n: return new RealNumber(value + n.Value);
                case BigNumber n: return new BigNumber(value + n.Value);
                case ComplexNumber n: return new ComplexNumber(value + n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Sub(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n:
                    try { return new IntegerNumber(checked(value - n.value)); }
                    catch { return new BigNumber(checked(value - n.value)); }
                case RealNumber n: return new RealNumber(value - n.Value);
                case BigNumber n: return new BigNumber(value - n.Value);
                case ComplexNumber n: return new ComplexNumber(value - n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Mul(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n:
                    try { return new IntegerNumber(checked(value * n.value)); }
                    catch { return new BigNumber(checked(value * n.value)); }
                case RealNumber n: return new RealNumber(value * n.Value);
                case BigNumber n: return new BigNumber(value * n.Value);
                case ComplexNumber n: return new ComplexNumber(value * n.Value);
                default: return num.AddTo(this);
            };
        }
        public override Number Div(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new RealNumber((double)value / n.value);
                case RealNumber n: return new RealNumber(value / n.Value);
                case BigNumber n: return new RealNumber(value / (double)n.Value);
                case ComplexNumber n: return new ComplexNumber(value / n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Mod(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new IntegerNumber(checked(value % n.value));
                case RealNumber n: return new RealNumber(value % n.Value);
                case BigNumber n: return new BigNumber(value % n.Value);
                case ComplexNumber n: throw new InvalidOperationException();
                default: return num.AddTo(this);
            }
        }

        public override string ToString()
            => value.ToString();
    }

    public sealed class RealNumber : Number
    {
        double value;
        public double Value => value;
        public RealNumber(double x) { value = x; }
        public static implicit operator RealNumber(double value)
            => new RealNumber(value);

        public override Number Add(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new RealNumber(value + n.Value);
                case RealNumber n: return new RealNumber(value + n.value);
                case BigNumber n: return new RealNumber(value + (double)n.Value);
                case ComplexNumber n: return new ComplexNumber(value + n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Sub(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new RealNumber(value - n.Value);
                case RealNumber n: return new RealNumber(value - n.value);
                case BigNumber n: return new RealNumber(value - (double)n.Value);
                case ComplexNumber n: return new ComplexNumber(value - n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Mul(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new RealNumber(value * n.Value);
                case RealNumber n: return new RealNumber(value * n.value);
                case BigNumber n: return new RealNumber(value * (double)n.Value);
                case ComplexNumber n: return new ComplexNumber(value * n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Div(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new RealNumber(value / n.Value);
                case RealNumber n: return new RealNumber(value / n.value);
                case BigNumber n: return new RealNumber(value / (double)n.Value);
                case ComplexNumber n: return new ComplexNumber(value / n.Value);
                default: return num.AddTo(this);
            }
        }
        public override Number Mod(Number num)
        {
            switch (num)
            {
                case null: throw new ArgumentNullException(nameof(num));
                case IntegerNumber n: return new RealNumber(value % n.Value);
                case RealNumber n: return new RealNumber(value % n.value);
                case BigNumber n: return new RealNumber(value % (double)n.Value);
                case ComplexNumber n: throw new InvalidOperationException();
                default: return num.AddTo(this);
            }
        }

        public override string ToString()
            => value.ToString();
    }

    public sealed class BigNumber : Number
    {
        BigInteger value;
        public BigInteger Value => value;
        public BigNumber(BigInteger x)
            => value = x;
        public static implicit operator BigNumber(BigInteger value)
            => new BigNumber(value);

        public override string ToString()
            => value.ToString();

        public override Number Add(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Sub(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Mul(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Div(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Mod(Number num)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ComplexNumber : Number
    {
        Complex value;
        public Complex Value => value;
        public ComplexNumber(Complex x)
            => value = x;
        public static implicit operator ComplexNumber(Complex value)
            => new ComplexNumber(value);

        public override string ToString()
            => value.ToString();

        public override Number Add(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Sub(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Mul(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Div(Number num)
        {
            throw new NotImplementedException();
        }

        public override Number Mod(Number num)
        {
            throw new NotImplementedException();
        }
    }
}
