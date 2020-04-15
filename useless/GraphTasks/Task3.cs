using TestingTasks;

namespace GraphTasks
{
    public static class Task3
    {
        public class V1 : TestingTask
        {
            private int _n, _counter, _i, _j;
            private int[] _times;
            private int[][] _inputs;

            private int MaxPath(int from, int to, int sum = 0)
            {
                LogItLine($"maxPath({from}, {to}, {sum})");
                sum += _times[from];
                if (from == to)
                    return sum;
                int[] arr = _inputs[from];
                int
                    len = arr[_counter],
                    max = 0;
                for (int i = 0; i < len; ++i)
                {
                    int j = MaxPath(arr[i], to, sum);
                    if (j > max)
                        max = j;
                }
                return max;
            }

            protected override void Main()
            {
                _n = ReadInt();
                _counter = _n - 1;
                _times = new int[_n];
                for (_i = 0; _i < _n; ++_i)
                    _times[_i] = ReadInt();
                _inputs = new int[_n][];
                for (_i = 0; _i < _n; ++_i)
                {
                    _inputs[_i] = new int[_n];
                    int[] arr = _inputs[_i];
                    for (_j = 0; _j < _n; ++_j)
                    {
                        if (0 != ReadInt())
                        {
                            arr[arr[_counter]++] = _j;
                        }
                    }
                }
                for (_i = 0; _i < _n; ++_i)
                {
                    for (_j = 0; _j < _n; ++_j)
                    {
                        if (0 != _j)
                            WriteChar(32);

                        WriteInt(_i == _j ? 0 : MaxPath(_i, _j));
                    }
                    WriteChar(10);
                }
            }
        }

        public class V2 : TestingTask
        {
            private int _n, _counter, _i, _j;
            private int[] _times;
            private int[][] _inputs;

            private int MaxPath(int from, int to, int sum = 0)
            {
                LogItLine($"maxPath({from}, {to}, {sum})");
                sum += _times[from];
                if (from == to)
                    return sum;
                int[] arr = _inputs[from];
                int
                    len = arr[_counter],
                    max = 0;
                for (int i = 0; i < len; ++i)
                {
                    int j = MaxPath(arr[i], to, sum);
                    if (j > max)
                        max = j;
                }
                return max;
            }

            protected override void Main()
            {
                _n = ReadInt();
                _counter = _n - 1;
                _times = new int[_n];
                for (_i = 0; _i < _n; ++_i)
                    _times[_i] = ReadInt();
                _inputs = new int[_n][];
                for (_i = 0; _i < _n; ++_i)
                {
                    _inputs[_i] = new int[_n];
                    int[] arr = _inputs[_i];
                    for (_j = 0; _j < _n; ++_j)
                    {
                        if (0 != ReadInt())
                        {
                            arr[arr[_counter]++] = _j;
                        }
                    }
                }
                for (_i = 0; _i < _n; ++_i)
                {
                    for (_j = 0; _j < _n; ++_j)
                    {
                        if (0 != _j)
                            WriteChar(32);

                        WriteInt(_i == _j ? 0 : MaxPath(_i, _j));
                    }
                    WriteChar(10);
                }
            }
        }
    }
}