using System;
using System.Collections.Generic;
using System.Linq;

//namespace useless.Automats{

public class Scheme
{
    private readonly Dictionary<(string, char), string> _states;
    private readonly string _start;
    private readonly string[] _finit;

    public Scheme(Dictionary<(string, char), string> states, string start, string[] finit)
    {
        _states = states;
        _start = start;
        _finit = finit;
    }

    public Scheme(string start, string[] finit, params (string, char, string)[] states)
    {
        _start = start;
        _finit = finit;
        _states = new Dictionary<(string, char), string>();
        foreach ((string curr, char symbol, string next) in states)
        {
            _states.Add((curr, symbol), next);
        }
    }

    public static Scheme ParseStates(string table)
    {
        string start = null;
        Dictionary<(string, char), string> states = new Dictionary<(string, char), string>();
        List<string> finit = new List<string>();
        foreach (string line in table.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            string[] tmp = line.Split(new[] { "→", "->" }, StringSplitOptions.None);
            if (tmp.Length != 2)
            {
                if (tmp.Length == 3 && string.IsNullOrWhiteSpace(tmp[0]))
                {
                    tmp[0] = tmp[1];
                    tmp[1] = tmp[2];
                    if (start != null)
                        throw new Exception("two starts in one scheme!");
                    start = tmp[0].Trim('(', ')');
                }
                else
                {
                    continue;
                }
            }

            string cur = tmp[0].Trim();
            if (cur.StartsWith("(") && cur.EndsWith(")"))
            {
                cur = cur.Substring(1, cur.Length - 2);
                finit.Add(cur);
            }
            tmp = tmp[1].Split('|');
            for (int i = 0; i < tmp.Length; ++i)
            {
                string pair = tmp[i].Trim();
                states.Add((cur, pair[0]), pair.Substring(1).TrimStart());
            }
        }
        return new Scheme(states, start, finit.ToArray());
    }

    public bool IsMatch(string str)
    {
        string state = _start;
        foreach (char sym in str)
        {
            if (_states.TryGetValue((state, sym), out string next))
                state = next;
            else
                return false;
        }
        return Array.IndexOf(_finit, state) >= 0;
    }

    public static void IsSolve()
    {
        Dictionary<(string, char), string> v1 = new Dictionary<(string, char), string>
        {
            [("S", '0')] = "Z1",
            [("S", '1')] = "O1",
            [("O1", '1')] = "S",
            [("Z1", '0')] = "Z2",
            [("Z2", '0')] = "S"
        };
        Dictionary<(string, char), string> v2 = new Dictionary<(string, char), string>();
        for (int z = 0; z < 3; ++z)
        {
            for (int o = 0; o < 2; ++o)
            {
                v2[($"Z{z}O{o}", '0')] = $"Z{(z + 1) % 3}O{o}";
                v2[($"Z{z}O{o}", '1')] = $"Z{z}O{(o + 1) % 2}";
            }
        }

        Scheme s1 = new Scheme(v1, "S", new string[] { "S" });
        Scheme s2 = new Scheme(v2, "Z0O0", new string[] { "Z0O0" });
        bool check(string str)
        {
            int os = str.Count(x => x == '1');
            int zs = str.Count(x => x == '0');
            return (os % 2 == 0) && (zs % 3 == 0) && (zs + os == str.Length);
        }
        for (int i = 0; i < 1000; ++i)
        {
            string str = Convert.ToString(i, 2);
            bool
                f1 = s1.IsMatch(str),
                f2 = s2.IsMatch(str),
                f3 = check(str);
            if (f1 || f2 || f3)
            {
                Console.WriteLine("{0,32}: {1} {2} {3}", str,
                    f1 ? 'T' : 'F',
                    f2 ? 'T' : 'F',
                    f3 ? 'T' : 'F');
            }
        }
    }
}
//}