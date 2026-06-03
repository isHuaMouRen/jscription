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

        public class ReadLine : CmdRoot
        {
            public override object? Run()
            {
                return Console.ReadLine();
            }
        }

        public class SetColor : CmdRoot
        {
            public required string Color { get; set; }

            public override object? Run()
            {
                if (Enum.TryParse(Color, true, out ConsoleColor result))
                    Console.ForegroundColor = result;
                else
                    throw new Exception($"未找到控制台颜色 \"{Color}\" 请查阅官方文档获得支持的颜色！");

                
                return null;
            }
        }

        public class GetColor : CmdRoot
        {
            public override object? Run()
            {
                return ((ConsoleColor)Console.ForegroundColor).ToString();
            }
        }
    }
}
