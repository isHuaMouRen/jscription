using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Runner.Utils
{
    internal class Logger
    {
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
