using System;
using System.Diagnostics;
using System.Text;

namespace TestingTasks
{
    public abstract class TestingTask
    {
        private StringBuilder _input;
        private readonly StringBuilder _output;

        public bool doLogging;

        protected TestingTask() => _output = new StringBuilder();

        private void Initial(string strInput)
        {
            _ = _output.Clear();
            _input = new StringBuilder(strInput);
            int ind = 0, length = _input.Length;
            while (ind < length && char.IsWhiteSpace(_input[ind]))
                ++ind;
            _ = _input.Remove(0, ind);
        }

        protected string ReadLexem()
        {
            int ind = 0, length = _input.Length;
            if (0 == length)
                return null;
            while (ind < length)
            {
                if (char.IsWhiteSpace(_input[ind]))
                    break;
                ind++;
            }
            string result = _input.ToString(0, ind);
            while (ind < length && char.IsWhiteSpace(_input[ind]))
                ++ind;
            _ = _input.Remove(0, ind);
            return result;
        }

        protected int ReadInt() => int.Parse(ReadLexem());

        protected void WriteString(string str) => _output.Append(str);
        protected void WriteString(char[] str) => _output.Append(str);
        protected void Write(object obj) => _output.Append(obj);
        protected void WriteInt(int num) => _output.Append(num);
        protected void WriteChar(int num) => _output.Append((char)num);
        protected void Write(char chr) => _output.Append(chr);

        protected void Writeln() => _output.AppendLine();
        protected void Writeln(string str) => _output.AppendLine(str);
        protected void Writeln(object obj) => Writeln(obj.ToString());
        protected void Writeln(int num) => Writeln(num.ToString());

        protected static void Log(string str) => Console.Write(str);
        protected static void LogLine(string str) => Console.WriteLine(str);
        protected static void Log(string str, ConsoleColor color)
        {
            ConsoleColor col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ForegroundColor = col;
        }
        protected static void LogLine(string str, ConsoleColor color)
        {
            ConsoleColor col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = col;
        }

        protected void LogIt(string str) { if (doLogging) Log(str); }
        protected void LogItLine(string str) { if (doLogging) LogLine(str); }
        protected void LogIt(string str, ConsoleColor color) { if (doLogging) Log(str, color); }
        protected void LogItLine(string str, ConsoleColor color) { if (doLogging) LogLine(str, color); }

        protected abstract void Main();

        public string Solve(string str)
        {
            Initial(str);
            Main();
            return _output.ToString();
        }

        private static (string expected, string got, int index) Diff(string l, string r)
        {
            l = l.Trim();
            r = r.Trim();
            int ll = l.Length, rl = r.Length, len = Math.Min(ll, rl), i;
            for (i = 0; i < len; i++)
            {
                if (l[i] != r[i])
                {
                    break;
                }
            }
            if (i < len || ll != rl)
            {
                char[] x = { '\r', '\n' };
                len = l.IndexOfAny(x, i);
                if (len > 0)
                    ll = len;
                len = r.IndexOfAny(x, i);
                if (len > 0)
                    rl = len;
                return (l.Substring(i, ll - i), r.Substring(i, rl - i), i);
            }
            return default;
        }

        private static bool IsSame(string s1, string s2)
            => s1.Trim().Equals(s2.Trim());

        public bool IsSolved(
            string input,
            string output)
            => IsSame(Solve(input), output);

        public static bool IsSolved(
            TestingTask task,
            bool print,
            params (string input, string output)[] pack)
        {
            Stopwatch sw = new Stopwatch();
            if (print)
                LogLine($"{task}", ConsoleColor.Yellow);
            bool solved = true;
            for (int i = 0; i < pack.Length; i++)
            {
                (string input, string output) = pack[i];
                sw.Restart();
                string res = task.Solve(input);
                string time = sw.Elapsed.ToString("ss\\.ffffff");
                (string expected, string got, int index) dif = Diff(output, res);
                if (print)
                    Log($"Test [{i}] ({time}): ");
                if (dif == default)
                {
                    if (print)
                        LogLine("PASSED", ConsoleColor.Green);
                    continue;
                }
                solved = false;
                if (print)
                    Log("FAILED ", ConsoleColor.Red);
                if (print)
                    LogLine($"expected \"{dif.expected}\" but got \"{dif.got}\" at index {dif.index}");
            }
            return solved;
        }
    }
}