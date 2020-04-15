using System.Diagnostics;
using System.IO;

namespace MicroCalculator
{
    public class MicroCalculator
    {
        private readonly Process calculator;
        private readonly StreamWriter input;
        private readonly StreamReader output;

        public MicroCalculator()
        {
            calculator = new Process()
            {
                StartInfo = new ProcessStartInfo("cmd", "/k")
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            calculator.Start();
            input = calculator.StandardInput;
            output = calculator.StandardOutput;
            input.WriteLine("echo off");
            output.ReadLine();
        }

        public string Exec(string str)
        {
            str = $"set /a \"{str}\"\r\n";
            input.WriteLine(str);
            output.ReadLine();
            return output.ReadLine();
        }

        public void Close()
        {
            calculator.Close();
            input.Close();
            output.Close();
        }

        ~MicroCalculator()
        {
            Close();
        }
    }
}