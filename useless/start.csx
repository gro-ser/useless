using System.Threading;
//using static useless.Program;
PrintOptions.MaximumOutputLength = 2048;
PrintOptions.MemberDisplayFormat = (dynamic)Enum.ToObject(PrintOptions.MemberDisplayFormat.GetType(), 1);
void Safe<T>(Func<T> f)
{
    Thread t = new Thread(() =>
    {
        try
        {
            Print(f());
        }
        catch (Exception e)
        {
            Print(e.Message);
        }
    });
    t.Start();
}
static string Distinct(this string str)
{
    return new string(str.Distinct<char>().ToArray());
}
/*
Safe(() => GetStringsByRegex(1, new string[]
{ "" },
@"", @"", @"", @""))
*/