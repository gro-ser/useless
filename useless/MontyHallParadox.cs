using System;
using System.Linq;

public class MontyHallParadox
{
    const int TestsCount = 1000000;
    static bool[] test = new bool[TestsCount];
    static Random rnd = new Random();
    static int rand(int n) => rnd.Next(n);
    static bool cmp(bool x) => x;

    int doorCount, index;
    bool[] doors;

    public MontyHallParadox(int DoorCount) => doorCount = DoorCount;
    void load()
    {
        (doors = new bool[doorCount])[rand(doorCount)] = true; index = -1;
    }
    void select(int x) { index = x; }
    int open()
    {
        for (int i = rand(doorCount); i < doorCount; ++i)
            if ((index != i) && !doors[i])
                return i;
        return index == 0 ? 1 : 0;
    }
    bool iswin() => doors[index];
    bool game(bool isopen)
    {
        int b, a = rand(doorCount); load(); select(a);
        if (isopen && doorCount > 2)
        {
            int[] idx = { a, open() };
            while (idx.Contains(b = rand(doorCount))) continue;
            select(b);
        }
        return iswin();
    }
    bool OpenFirsDoor(bool x) => game(false);
    bool OpenAnotherDoor(bool x) => game(true);

    static void GameIteration(int MyDoorCount)
    {
        var game = new MontyHallParadox(MyDoorCount);
        Console.Write("{0,2}{1}\nOpen first door:\t{2:P}\nOpen another door:\t{3:P}\n\n",
            MyDoorCount, " Doors:  Method |  win count",
            test.Select(game.OpenFirsDoor).Count(cmp) * 1f / TestsCount,
            test.Select(game.OpenAnotherDoor).Count(cmp) * 1f / TestsCount);
    }
    public static void Main()
    {
        for (int i = 3; i < 11; i++)
            GameIteration(MyDoorCount: i);
    }
}