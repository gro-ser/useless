using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public class PSCalc
{
    static readonly IEnumerable<string> members = typeof(Math).GetMembers().Select(mi => mi.Name);
    static readonly Regex pattern = new Regex(string.Join("|", members), RegexOptions.IgnoreCase);

    public static double[] Execute(string fun, params double[] values)
    {
        // support for math members (sin, log, pi, etc.)
        fun = pattern.Replace(fun, "[math]::$0");
        // 'x' is a parameter
        fun = fun.Replace("x", "$_");
        // delimeter is dot, not comma
        var nfi = new NumberFormatInfo
        { NumberDecimalSeparator = "." };
        // all values to string array such "1,2,3.4"
        var array = string.Join(",", values.Select(x => x.ToString(nfi)));
        // <values> | % { <function body> }
        fun = $"{array} | % {{ {fun} }}";
        Process ps = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                // no trash strings at start and run only one command
                Arguments = $"-nologo -command \"{fun}\"",
                // to read output values
                RedirectStandardOutput = true,
                // create no window
                UseShellExecute = false
            }
        };
        ps.Start();
        var output = ps.StandardOutput.ReadToEnd();
        // each line is one result
        var results = output.Split(new[] { "\r\n" }, StringSplitOptions.None);
        // pass one empty line at end [1]
        if (results.Length - 1 == values.Length)
            try
            {
                // pass one empty line at end [2]
                return results.Take(values.Length)
                    // parse to numbers
                    .Select(double.Parse).ToArray();
            }
            catch { }
        return null;
    }
}