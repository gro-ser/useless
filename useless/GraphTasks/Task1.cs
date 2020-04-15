using System;
using System.Collections.Generic;
using TestingTasks;

namespace GraphTasks
{
    internal class Task1 : TestingTask
    {
        protected override void Main()
        {
            int length = ReadInt();
            SortedSet<char> alphabet = new SortedSet<char>();
            Dictionary<char, SortedSet<char>> graph = new Dictionary<char, SortedSet<char>>();
            string pred, curr = ReadLexem();
            foreach (char c in curr)
                alphabet.Add(c);
            for (int i = 1; i < length; i++)
            {
                pred = curr;
                curr = ReadLexem();
                bool added = false;
                for (int j = 0,
                    len = Math.Min(pred.Length, curr.Length);
                    j < len; j++)
                {
                    if (added)
                    {
                        alphabet.Add(curr[j]);
                    }
                    else if (pred[j] != curr[j])
                    {
                        char from = pred[j], to = curr[j];
                        if (graph.TryGetValue(from, out SortedSet<char> edge))
                        {
                            edge.Add(to);
                        }
                        else
                        {
                            graph[from] = new SortedSet<char> { to };
                        }
                        alphabet.Add(from);
                        alphabet.Add(to);
                        added = true;
                        len = curr.Length;
                    }
                }
            }
            string buf = "";
            while (alphabet.Count > 0)
            {
                char x = '\0';
                foreach (char y in alphabet)
                {
                    if (!graph.ContainsKey(y))
                    {
                        x = y;
                        break;
                    }
                }
                if (0 == x)
                {
                    buf = "-";
                    break;
                }
                buf = x + buf;
                foreach (char y in alphabet)
                {
                    if (graph.TryGetValue(y, out SortedSet<char> edge) && edge.Contains(x))
                    {
                        edge.Remove(x);
                        if (0 == edge.Count)
                            graph.Remove(y);
                    }
                }
                alphabet.Remove(x);
            }
            WriteString(buf);
        }
    }
}