using System;
using System.Collections.Generic;
using static System.Math;

class CodeInGame
{
    static string[] input = @"5
3
C X X X C
C X X X C
C X X X C
C X X X C
C X X X C"
.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
    static int ind = 0;
    static string r() => input[ind++];
    static int i() => int.Parse(r());
    struct vec { public int x, y; }
    static void Main()
    {
        int N = i(), L = i(), x, y;
        int[,] arr = new int[N, N];
        List<vec> list = new List<vec>();
        string s;
        for (x = 0; x < N; ++x)
            for (s = r(), y = 0; y < N; ++y)
                if (s[y * 2] == 'C')
                    list.Add(new vec() { x = x, y = y });
        foreach (var v in list)
        {
            for (x = 0; x < N; ++x)
                for (y = 0; y < N; ++y)
                    arr[x, y] = Max(arr[x,y], L - dist(x, y, v));
        }
        //for (x = 0; x < N; ++x) { for (y = 0; y < N; ++y) Console.Write(arr[x, y] + " "); Console.WriteLine(); }
        int count = 0;
        for (x = 0; x < N; ++x)
            for (y = 0; y < N; ++y)
                if (arr[x, y] == 0)
                    ++count;
        Console.WriteLine(count);
        Console.ReadLine();
    }
    static int dist(int x, int y, vec v) => Max(Abs(x - v.x), Abs(y - v.y));
}