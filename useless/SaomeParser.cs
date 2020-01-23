using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace useless
{
    public class SaomeParser
    {
        public static (float a, float b, float c) Parse(string expr)
        {
            var (a, b, c) = (0f, 0f, 0f);

            bool part = false;
            int len = expr.Length, i = 0, sub;
            while (i<len)
            {
                // skip whitespaces
                while (char.IsWhiteSpace(expr[i])) ++i;
                sub = i;
                while ("0123456789+-.,".Contains(expr[i])) ++i;
                float tmp;
                if (!float.TryParse(expr.Substring(sub, i - sub), out tmp))
                    throw new Exception("bad expr");

            }

            return (a, b, c);
        }
    }
}
