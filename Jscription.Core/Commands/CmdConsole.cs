using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdConsole
    {
        public class Write : CmdRoot
        {
            public string? Message { get; set; }

            public override object? Run() { Console.Write(Message); return null; }
        }

        public class WriteLine : CmdRoot
        {
            public string? Message { get; set; }

            public override object? Run() { Console.WriteLine(Message); return null; }
        }
    }
}
