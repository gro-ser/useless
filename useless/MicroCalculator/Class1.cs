using System;
using System.Diagnostics;
using System.IO;

namespace MicroCalculator
{
    public class Calculator
    {
        Process calculator;
        StreamWriter input;
        StreamReader output;

        public Calculator()
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

        ~Calculator()
        {
            Close();
        }
    }
}