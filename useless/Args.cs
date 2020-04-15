using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Args
{
    public string RawArgs { get; protected set; }
    public string Keys { get; protected set; }
    public bool HasKey(char key) => Keys.Contains(key);
    public string[] Params { get; protected set; }
    public Dictionary<string, string> NamedArgs { get; protected set; }
    public bool HasNamedArg(string name) => NamedArgs.ContainsKey(name);

    protected Args(string raw) => RawArgs = raw;

    private const char keySeparator = '-';
    private const string quots = "'\"`";

    private static char escape(char x)
    {
        switch (x)
        {
            case '0':
                return '\0';
            case 'a':
                return '\a';
            case 'b':
                return '\b';
            case 'f':
                return '\f';
            case 'n':
                return '\n';
            case 'r':
                return '\r';
            case 't':
                return '\t';
            case 'v':
                return '\v';
            case '\'':
                return '\'';
            case '\"':
                return '\"';
            case '\\':
                return '\\';
            default:
                throw new Exception("Unrecognized escape sequence.");
        }
    }

    private static string escape(string raw, int start, int length)
    {
        StringBuilder buf = new StringBuilder();
        for (int i = start; 0 < length; --length, ++i)
        {
            char x = raw[i];
            if (x == '\\')
            {
                ++i;
                --length;
                x = escape(raw[i]);
            }
            buf.Append(x);
        }
        return buf.ToString();
    }

    private static void skip(ref int v, int len, string raw)
    {
        while (v < len &&
        char.IsWhiteSpace(raw[v]))
        {
            ++v;
        }
    }
    public static Args Parse(string raw)
    {
        int v = 0, len = raw.Length, start;
        List<string> list = new List<string>();
        StringBuilder keys = new StringBuilder();
        Dictionary<string, string> dic = new Dictionary<string, string>();

        while (v < len)
        {
            skip(ref v, len, raw);
            if (v == len)
                break;

            if (raw[v] == keySeparator)
            {
                ++v;
                if (v == len)
                    throw new Exception("EOL found in key declaration.");
                while (v < len && !char.IsWhiteSpace(raw[v]))
                    keys.Append(raw[v++]);
                continue;
            }
            if (quots.Contains(raw[v]))
            {
                char quot = raw[v++];
                start = v;
                while (v < len && raw[v] != quot)
                    ++v;
                if (v == len || raw[v] != quot)
                    throw new Exception("EOL found in quoted string.");
                if (quot == '`')
                    list.Add(raw.Substring(start, v - start));
                else
                    list.Add(escape(raw, start, v - start));
                ++v;
                continue;
            }
            start = v;
            while (v < len && !char.IsWhiteSpace(raw[v]))
                ++v;
            string arg = raw.Substring(start, v - start);
            start = arg.IndexOf(':');
            if (start == -1)
            {
                list.Add(arg);
                continue;
            }
            string name = arg.Remove(start), value = arg.Substring(1 + start);
            if ("" == name)
                throw new Exception("Empty name in named argument.");
            if ("" == value)
                throw new Exception("Empty value in named argument.");
            dic.Add(name, value);
        }
        return new Args(raw) { Keys = keys.ToString(), Params = list.ToArray(), NamedArgs = dic };
    }
}

public class ArgsTest
{
    private static string read() => Console.ReadLine();

    private static void PrintArgs(Args args)
    {
        Console.WriteLine($"RawArgs:{args.RawArgs}");
        Console.WriteLine($"  Keys :{args.Keys}");
        foreach (KeyValuePair<string, string> wat in args.NamedArgs)
            Console.WriteLine($"  {wat.Key,5}:{wat.Value}");
        for (int i = 0, l = args.Params.Length; i < l; ++i)
            Console.WriteLine($"  [{i,3}]:{args.Params[i]}");
        Console.WriteLine();
    }
    public static void Main()
    {
        string str;
        Args args;
        while ((str = read()) != null)
        {
            try
            { args = Args.Parse(str); PrintArgs(args); }
            catch (Exception ex)
            { Console.WriteLine($"<{str}>\n{ex.Message}\n"); }
        }
    }
}