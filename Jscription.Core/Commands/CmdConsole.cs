using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdConsole
    {
        public class Print : CmdRoot
        {
            public string? Message { get; set; }

            public override object? Run() { Console.Write(Message); return null; }
        }

        public class PrintLine : CmdRoot
        {
            public string? Message { get; set; }

            public override object? Run() { Console.WriteLine(Message); return null; }
        }
    }
}
