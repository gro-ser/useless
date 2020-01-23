using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace useless
{
    // "A_DEAD_DAD_CEDED_A_BAD_BABE_A_BEADED_ABACA_BED"
    public class Huffman
    {
        class Tree : IComparable<Tree>
        {
            public Tree L, R;
            public char Char;
            public int Count;

            public Tree(char ch, int co) =>
                (Char, Count) = (ch, co);

            public Tree(Tree l, Tree r, int co)
                : this('\0', co) =>
                (L, R) = (l, r);

            public int CompareTo(Tree other) =>
                Count.CompareTo(other.Count);

            internal Dictionary<char, string> CreateDictionary()
            {
                var dic = new Dictionary<char, string>();
                CreateDictionary("", dic);
                return dic;
            }

            private void CreateDictionary(string v, Dictionary<char, string> dic)
            {
                if (R == null && L == null) dic[Char] = v;
                if (L != null) L.CreateDictionary(v + "0", dic);
                if (R != null) R.CreateDictionary(v + "1", dic);
            }
        }

        public static Dictionary<char,string> GetCodes(string str)
        {
            Dictionary<char, int> dic = new Dictionary<char, int>();
            foreach (var x in str)
                if (dic.ContainsKey(x))
                    ++dic[x];
                else
                    dic.Add(x, 1);
            List<Tree> list = new List<Tree>();
            foreach (var pair in dic)
                list.Add(new Tree(pair.Key, pair.Value));
            while (list.Count>1)
            {
                var l = list.Min();
                list.Remove(l);
                var r = list.Min();
                list.Remove(r);
                list.Add(new Tree(l, r, l.Count + r.Count));
            }
            return list[0].CreateDictionary();
        }

        public static string GetCharsBad(string str)
        {
            Dictionary<char, int> dic = new Dictionary<char, int>();
            foreach (var x in str)
                if (dic.ContainsKey(x))
                    ++dic[x];
                else
                    dic.Add(x, 1);

            Dictionary<string, int> res = new Dictionary<string, int>();
            foreach (var item in dic)
                res.Add(item.Key.ToString(), item.Value);

            KeyValuePair<string,int> findMin()
            {
                int min = int.MaxValue;
                KeyValuePair<string, int> pair = default;
                foreach (var item in res)
                    if (item.Value < min)
                        (pair, min) = (item, item.Value);
                return pair;
            }

            while (res.Count>1)
            {
                var a = findMin();
                res.Remove(a.Key);
                var b = findMin();
                res.Remove(b.Key);
                res.Add(a.Key + b.Key, a.Value + b.Value);
            }

            return findMin().Key;
        }
    }
}
