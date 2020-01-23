using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace useless
{
    public class OlimpTasks
    {
        public const float deg = (float)(180 / PI);
        static float Angle(float dx, float dy) => (float)Atan2(dy, dx) * deg;
        static float Sqr(float a) => a * a;
        static float Abs(float x, float y) => (float)Sqrt(Sqr(x) + Sqr(y));
        public static (float[], float[]) GetFactorsAndScales(float x, float y, (float x, float y)[] points)
        {
            int len = points.Length;
            float[] scales = new float[len], angles = new float[len];
            float angle = 90;
            var max = points.Max(p => Abs(x - p.x, y - p.y));
            for (int i = 0; i < len; i++)
            {
                var p = points[i];
                var ang = Angle(p.x - x, p.y - y);
                angles[i] = ang - angle;
                angle = ang;
                scales[i] = Abs(p.x - x, p.y - y) / max;
            }
            return (angles, scales);
        }
        public static (float[], float[]) GetFactorsAndScales((float, float, (float, float)[]) args)
            => GetFactorsAndScales(args.Item1, args.Item2, args.Item3);
        public static (float[], float[]) GetFactorsAndScales((float x, float y)[] points)
            => GetFactorsAndScales(points.Average(p => p.x), points.Average(p => p.y), points);
        static (float, float)[]
            Square = (new (float, float)[] { (0, 2), (2, 2), (2, 0), (0, 0) }),
            Square2 = (new (float, float)[] { (0, 2), (1, 2), (2, 2), (2, 1), (2, 0), (1, 0), (0, 0), (0, 1) });

        static void Println<type>(type[] arr, Func<type, string> str = null)
        {
            if (str == null) str = v => v.ToString();
            var length = arr.Length;
            Console.Write("{");
            if (length > 0)
                Console.Write("{0}", str(arr[0]));
            for (int i = 1; i < length; i++)
                Console.Write(", {0}", str(arr[i]));
            Console.WriteLine("}");
        }
        static void Println<type>((type[], type[]) args, Func<type, string> str = null)
        {
            Println(args.Item1, str); Println(args.Item2, str);
            Console.WriteLine();
        }

        static (float, float, (float, float)[])
            square = (1, 1, new (float, float)[] { (0, 2), (2, 2), (2, 0), (0, 0) }),
            square2 = (1, 1, new (float, float)[] { (0, 2), (1, 2), (2, 2), (2, 1), (2, 0), (1, 0), (0, 0), (0, 1) }),
            triangle = (0, 0, new (float, float)[] { (0, 1), (0.866f, -0.5f), (-0.866f, -0.5f) }),
            navalny = (3, 3, new (float, float)[] { (0, 0), (0, 8), (2, 8), (2, 4), (4, 4), (4, 8), (6, 8), (6, 0), (4, 0), (4, 2), (2, 2), (2, 0) }),
            snowflake = (37, 40, new (float, float)[] { (35, 35), (35, 28), (30, 25), (24, 16), (35, 22), (35, 15), (24, 9), (24, 4), (35, 10), (35, 0), (40, 0), (40, 10), (51, 4), (51, 9), (40, 15), (40, 22), (51, 16), (45, 25), (40, 28), (40, 35) }),
            lightning = (1000, 1000, new (float, float)[] { (1185, 496), (1023, 941), (1258, 941), (818, 1506), (980, 1061), (743, 1061) })
            ;

        public static void Test()
        {
            CultureInfo.CurrentCulture = new CultureInfo(1049)
            {
                NumberFormat = new NumberFormatInfo
                { NumberGroupSeparator = "." }
            };
            Println(GetFactorsAndScales(square2));
            Println(GetFactorsAndScales(Square2));
            Println(GetFactorsAndScales(square));
            Println(GetFactorsAndScales(Square));
            triangle.ToString();
            navalny.ToString();
            snowflake.ToString();
            Println(GetFactorsAndScales(lightning), v => v + "f");
        }
    }
}