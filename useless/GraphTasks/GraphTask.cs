using System;
using System.Text;

namespace GraphTasks
{
    public abstract class GraphTask
    {
        private StringBuilder input;
        private StringBuilder output;

        public bool DoLogging;

        public GraphTask()
        {
            output = new StringBuilder();
        }

        private void Initial(string strInput)
        {
            output.Clear();
            input = new StringBuilder(strInput);
            int ind = 0, length = input.Length;
            while (ind < length && char.IsWhiteSpace(input[ind])) ++ind;
            input.Remove(0, ind);
        }

        protected string ReadLexem()
        {
            int ind = 0, length = input.Length;
            if (0 == length)
                return null;
            while (ind < length)
            {
                if (char.IsWhiteSpace(input[ind]))
                    break;
                ind++;
            }
            string result = input.ToString(0, ind);
            while (ind < length && char.IsWhiteSpace(input[ind])) ++ind;
            input.Remove(0, ind);
            return result;
        }

        protected int ReadInt() => int.Parse(ReadLexem());

        protected void WriteString(string str) { output.Append(str); }
        protected void Write(object obj) { output.Append(obj); }
        protected void WriteInt(int num) { output.Append(num); }
        protected void WriteChar(int num) { output.Append((char)num); }
        protected void Write(char chr) { output.Append(chr); }

        protected void Writeln() { output.AppendLine(); }
        protected void Writeln(string str) { output.AppendLine(str); }
        protected void Writeln(object obj) { Writeln(obj.ToString()); }
        protected void Writeln(int num) { Writeln(num.ToString()); }

        protected static void Log(string str)
        {
            Console.Write(str);
        }
        protected static void LogLine(string str)
        {
            Console.WriteLine(str);
        }
        protected static void Log(string str, ConsoleColor color)
        {
            var col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ForegroundColor = col;
        }
        protected static void LogLine(string str, ConsoleColor color)
        {
            var col = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = col;
        }

        protected void LogIt(string str) { if (DoLogging) Log(str); }
        protected void LogItLine(string str) { if (DoLogging) LogLine(str); }
        protected void LogIt(string str, ConsoleColor color) { if (DoLogging) Log(str, color); }
        protected void LogItLine(string str, ConsoleColor color) { if (DoLogging) LogLine(str, color); }

        protected abstract void Main();

        public string Solve(string str)
        {
            Initial(str);
            Main();
            return output.ToString();
        }

        static private (string expected, string got, int index) Diff(string l, string r)
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
                char[] x = { '\t', '\n' };
                len = l.IndexOfAny(x, i);
                if (len > 0) ll = len;
                len = r.IndexOfAny(x, i);
                if (len > 0) rl = len;
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
            GraphTask task,
            bool print,
            params (string input, string output)[] pack)
        {
            if(print) LogLine($"{task}", ConsoleColor.Yellow);
            bool solved = true;
            for (int i = 0; i < pack.Length; i++)
            {
                var (input, output) = pack[i];
                var res = task.Solve(input);
                var dif = Diff(output, res);
                if (print) Log($"Test [{i}]: ");
                if (dif == default)
                {
                    if (print) LogLine("PASSED", ConsoleColor.Green);
                    continue;
                }
                if (print) Log("FAILED ", ConsoleColor.Red);
                if (print) LogLine($"expected \"{dif.expected}\" but got \"{dif.got}\" at index {dif.index}");
            }
            return solved;
        }
    }
}