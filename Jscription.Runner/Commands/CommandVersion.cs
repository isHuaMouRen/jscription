using Jscription.Runner.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Runner.Commands
{
    internal class CommandVersion : ICliCommand
    {
        public string Name => "version";
        public string Description => "输出当前 Jscription Runner 的版本信息";
        public string Usage => """
            version   - 直接输出版本信息
            """;

        public int Execute(string[] args)
        {
            var information = $"""
                Jscription Engine  version:{Global.Version}
                
                Made by HuaMouRen (https://github.com/isHuaMouRen)
                """;
            Console.WriteLine(information);
            return 0;
        }
        
    }
}
