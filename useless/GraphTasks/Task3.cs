using System;

namespace GraphTasks
{
    public class Task3 : GraphTask
    {
        int n, counter, i, j;
        int[] times;
        int[][] inputs;

        int maxPath(int from, int to, int sum = 0)
        {
            LogItLine($"maxPath({from}, {to}, {sum})");
            sum += times[from];
            if (from == to)
                return sum;
            int[] arr = inputs[from];
            int
                len = arr[counter],
                max = 0;
            for (int i = 0; i < len; ++i)
            {
                int j = maxPath(arr[i], to, sum);
                if (j > max) max = j;
            }
            return max;
        }

        protected override void Main()
        {
            n = ReadInt();
            counter = n - 1;
            times = new int[n];
            for (i = 0; i < n; ++i)
                times[i] = ReadInt();
            inputs = new int[n][];
            for (i = 0; i < n; ++i)
            {
                inputs[i] = new int[n];
                int[] arr = inputs[i];
                for (j = 0; j < n; ++j)
                {
                    if (0 != ReadInt())
                    {
                        arr[arr[counter]++] = j;
                    }
                }
            }
            for (i = 0; i < n; ++i)
            {
                for (j = 0; j < n; ++j)
                {
                    if (0 != j)
                        WriteChar(32);

                    WriteInt(i == j ? 0 : maxPath(i, j));
                }
                WriteChar(10);
            }
        }
    }
}