using System;
using System.Linq;

namespace useless
{
    public class SaomeParser
    {
        public static (float a, float b, float c) Parse(string expr)
        {
            (float a, float b, float c) = (0f, 0f, 0f);
            int len = expr.Length, i = 0, sub;
            while (i < len)
            {
                // skip whitespaces
                while (char.IsWhiteSpace(expr[i]))
                    ++i;
                sub = i;
                while ("0123456789+-.,".Contains(expr[i]))
                    ++i;
                if (!float.TryParse(expr.Substring(sub, i - sub), out float tmp))
                    throw new Exception("bad expr");

            }

            return (a, b, c);
        }
    }
}
