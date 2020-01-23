using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace useless
{
    public class RandomString
    {
        private static string
            _digits = "09",
            _lettersUp = "AZ",
            _lettersLow = "az",
            //()*+,-./
            _operators = "(/",
            //!\"#$%&
            _specialOne = "!&",
            //:;<=>?@
            _specialTwo = ":@",
            //[\\]^_`
            _specialThree = "[`",
            //{|}
            _specialFoure = "{}";
        Random rand = new Random();
        string ranges = "";
        public RandomString(
            bool Digits,
            bool LettersUp,
            bool LettersLow)
        {
            if (!(LettersLow || LettersUp || Digits)) throw new Exception("bad");
            if (Digits) ranges += _digits;
            if (LettersUp) ranges += _lettersUp;
            if (LettersLow) ranges += _lettersLow;
        }

        public RandomString(
            bool Digits,
            bool LettersUp,
            bool LettersLow,
            bool Operators,
            bool SpecialOne = false,
            bool SrecialTwo = false,
            bool SpecialThree = false,
            bool SpetialFoure = false)
            :this(Digits,LettersUp,LettersLow)
        {
            if (Operators) ranges += _operators;
            if (SpecialOne) ranges += _specialOne;
            if (SrecialTwo) ranges += _specialTwo;
            if (SpecialThree) ranges += _specialThree;
            if (SpetialFoure) ranges += _specialFoure;
        }

        public char NextChar()
        {
            int i = rand.Next(ranges.Length / 2) * 2;
            char from = ranges[i],
                to = ranges[i + 1],
                x = (char)rand.Next(from, to);
            return x;
        }

        public string Next(int length)
        {
            if (length <= 0) return "";
            var sb = new StringBuilder(length);
            while (length-- > 0) sb.Append(NextChar());
            return sb.ToString();
        }
    }
}