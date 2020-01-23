using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace useless
{
    class SpellNums
    {
        static string num = "0123456789ABCDEF";

        StringBuilder str, tmp;
        int radix, counter = 0;

        public SpellNums(int radix, int start)
        {
            if (radix > 16) throw
                new Exception("radix must be less then 16");
            this.radix = radix;
            tmp = new StringBuilder().Append(num[start]);
            str = new StringBuilder();
        }
        public SpellNums(int radix) : this(radix, 1) { }
        public SpellNums() : this(10, 1) { }

        void addDigit(int x)
        {
            if (x >= radix)
                addDigit(x / radix);
            tmp.Append(num[x % radix]);
        }
        public string Step()
        {
            ++counter;
            int length = str.Length, count = 0;
            char digit = length == 0 ? '\0' : str[0];
            for (int i = 0; i < length; i++)
                if (str[i] == digit) ++count;
                else
                {
                    addDigit(count);
                    tmp.Append(digit);
                    count = 1;
                    digit = str[i];
                }
            if (count != 0)
            {
                addDigit(count);
                tmp.Append(digit);
            }
            str.Clear();
            (str, tmp) = (tmp, str);
            return str.ToString();
        }
        public string[] Steps(int stepCount = 10)
        {
            string[] arr = new string[stepCount];
            for (int i = 0; i < stepCount; i++)
                arr[i] = Step();
            return arr;
        }
        public IEnumerable<string> StepSeq(int stepCount = 10)
        {
            while (stepCount --> 0)
                yield return Step();
        }
    }
}