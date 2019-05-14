using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _
{
    class BoldTyper
    {
        static Dictionary<char, string> symbols
            = new Dictionary<char, string>()
            {
                ['A'] = "𝔸",
                ['a'] = "𝕒",
                ['B'] = "𝔹",
                ['b'] = "𝕓",
                ['C'] = "ℂ",
                ['c'] = "𝕔",
                ['D'] = "𝔻",
                ['d'] = "𝕕",
                ['E'] = "𝔼",
                ['e'] = "𝕖",
                ['F'] = "𝔽",
                ['f'] = "𝕗",
                ['G'] = "𝔾",
                ['g'] = "𝕘",
                ['H'] = "ℍ",
                ['h'] = "𝕙",
                ['I'] = "𝕀",
                ['i'] = "𝕚",
                ['J'] = "𝕁",
                ['j'] = "𝕛",
                ['K'] = "𝕂",
                ['k'] = "𝕜",
                ['L'] = "𝕃",
                ['l'] = "𝕝",
                ['M'] = "𝕄",
                ['m'] = "𝕞",
                ['N'] = "ℕ",
                ['n'] = "𝕟",
                ['O'] = "𝕆",
                ['o'] = "𝕠",
                ['P'] = "ℙ",
                ['p'] = "𝕡",
                ['Q'] = "ℚ",
                ['q'] = "𝕢",
                ['R'] = "ℝ",
                ['r'] = "𝕣",
                ['S'] = "𝕊",
                ['s'] = "𝕤",
                ['T'] = "𝕋",
                ['t'] = "𝕥",
                ['U'] = "𝕌",
                ['u'] = "𝕦",
                ['V'] = "𝕍",
                ['v'] = "𝕧",
                ['W'] = "𝕎",
                ['w'] = "𝕨",
                ['X'] = "𝕏",
                ['x'] = "𝕩",
                ['Y'] = "𝕐",
                ['y'] = "𝕪",
                ['Z'] = "ℤ",
                ['z'] = "𝕫",
                ['0'] = "𝟘",
                ['1'] = "𝟙",
                ['2'] = "𝟚",
                ['3'] = "𝟛",
                ['4'] = "𝟜",
                ['5'] = "𝟝",
                ['6'] = "𝟞",
                ['7'] = "𝟟",
                ['8'] = "𝟠",
                ['9'] = "𝟡",
            };

        static string Enc(string str)
        {
            var sb = new StringBuilder(str.Length * 2);
            foreach (var chr in str)
            {
                if (symbols.ContainsKey(chr))
                    sb.Append(symbols[chr]);
                else sb.Append(chr);
            }
            return sb.ToString();
        }
    }
}
