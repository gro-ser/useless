using System;
using System.Collections.Generic;
using System.Text;
using fun = System.Func<double, double, double>;

public abstract class NotationParser
{
    #region MathOperators
    static double add(double a, double b) => a + b;
    static double sub(double a, double b) => a - b;
    static double mul(double a, double b) => a * b;
    static double div(double a, double b) => a / b;
    static double mod(double a, double b) => a % b;
    #endregion
    protected static Dictionary<string, fun> operators =
        new Dictionary<string, fun>()
        {
            ["+"] = add,
            ["-"] = sub,
            ["*"] = mul,
            ["/"] = div,
            ["%"] = mod,
            ["^"] = Math.Pow,
            ["log"] = Math.Log,
            ["max"] = Math.Max,
            ["min"] = Math.Min
        };
    protected static Dictionary<string, double> constants =
        new Dictionary<string, double>()
        {
            ["e"] = Math.E,
            ["pi"] = Math.PI,
            ["eps"] = double.Epsilon,
            ["nan"] = double.NaN,
            ["inf"] = double.PositiveInfinity,
            ["-inf"] = double.NegativeInfinity,
        };
    public abstract double? Parse(string str);
    protected static void skip(string str, ref int index)
    {
        while (char.IsWhiteSpace(str, index)) ++index;
    }
    protected static void unskip(string str, ref int index)
    {
        while (index >= 0 && char.IsWhiteSpace(str, index)) --index;
    }
    protected static string next(string str, ref int index, in int len)
    {
        var sb = new StringBuilder();
        while (index <= len && !char.IsWhiteSpace(str, index))
            sb.Append(str[index++]);
        return sb.ToString();
    }
}

public class PostFixNotationParser : NotationParser
{
    static PostFixNotationParser parser = new PostFixNotationParser();
    public static double? Eval(string str) => parser.Parse(str);
    public override double? Parse(string str)
    {
        var stack = new Stack<double>();
        int length = str.Length - 1, index = 0;
        double value;
        unskip(str, ref length);
        if (length < 0) return null;
        while (index <= length)
        {
            skip(str, ref index);
            string key = next(str, ref index, length);
            if (constants.ContainsKey(key)) stack.Push(constants[key]);
            else if (operators.ContainsKey(key))
                if (stack.Count > 1)
                {
                    value = stack.Pop();
                    stack.Push(operators[key](stack.Pop(), value));
                }
                else return null;
            else if (double.TryParse(key, out value))
                stack.Push(value);
            else return null;
        }
        if (stack.Count == 1) return stack.Pop();
        stack.Clear();
        return default;
    }
}
public class PreFixNotationParser : NotationParser
{
    static PreFixNotationParser parser = new PreFixNotationParser();
    public static double? Eval(string str) => parser.Parse(str);
    public override double? Parse(string str)
    {
        var stack = new Stack<fun>();
        int length = str.Length - 1, index = 0, i = 0;
        double value;
        double[] tmp = { 0, 0 };
        unskip(str, ref length);
        if (length < 0) return null;
        while (index <= length)
        {
            skip(str, ref index);
            string key = next(str, ref index, length);
            if (constants.ContainsKey(key)) tmp[i++] = (constants[key]);
            else if (operators.ContainsKey(key))
                if (i != 2) // TODO i == 0
                    stack.Push(operators[key]);
                else return null;
            else if (double.TryParse(key, out value))
                tmp[i++] = (value);
            else return null;
            if (i == 2)
                if (stack.Count == 0) return null;
                else
                    tmp[0] = stack.Pop()(tmp[0], tmp[i = 1]);
        }
        if (stack.Count == 0 && i == 1) return tmp[0];
        stack.Clear();
        return default;
    }
}