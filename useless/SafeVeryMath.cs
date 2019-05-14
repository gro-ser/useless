using System;
public class SafeVeryMath<T> : _.VeryMath<T>
{
    public override T convert(IConvertible value)
    {
        throw new NotImplementedException();
    }
    public override T convert(string str)
    {
        throw new NotImplementedException();
    }

    public override bool equal(T a, T b)
    {
        throw new NotImplementedException();
    }
    public override bool less(T a, T b)
    {
        throw new NotImplementedException();
    }
    public override bool greater(T a, T b)
    {
        throw new NotImplementedException();
    }

    public override T mod(T a, T b)
    {
        throw new NotImplementedException();
    }
    public override T mul(T a, T b)
    {
        throw new NotImplementedException();
    }
    public override T sub(T a, T b)
    {
        throw new NotImplementedException();
    }
    public override T sum(T a, T b)
    {
        throw new NotImplementedException();
    }
    public override T div(T a, T b)
    {
        throw new NotImplementedException();
    }

}
class UselessCalc : SafeVeryMath<int>
{
    public static void USELESSCALC() { }
}