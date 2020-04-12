using System;
using System.Collections.Generic;
using System.Text;

namespace TestVersions
{
    public class Container
    {
        public const string hello = "hello";
        public static string world = "world";
        public static void say()
        {
            Console.WriteLine(hello + " " + world + "!");
        }
        public static int[] nums = { -1412390192, -1613750240, -1798268720, -1596940592 };
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("HELP!");
        }
    }
}
