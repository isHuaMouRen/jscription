using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Runner.Commands
{
    internal class CommandHelp : ICliCommand
    {
        public string Name => "help";
        public string Description => "显示所有可用的命令描述或某个指定命令的描述";
        public string Usage => """
            help   - 显示所有命令及其简述
            help <命令名>   - 显示某个命令的用法
            """;

        public int Execute(string[] args)
        {
            string? commandName = null;

            if (args.Length >= 1)
                commandName = args[0];

            if (commandName == null)
            {
                Console.WriteLine("可用命令:");
                foreach (var cmd in Program._cliRegistry.Values)
                    Console.WriteLine($"{cmd.Name}   - {cmd.Description}");
                Console.WriteLine();
            }
            else
            {
                foreach (var cmd in Program._cliRegistry.Values)
                {
                    if (cmd.Name == commandName)
                    {
                        Console.WriteLine($"""
                            {cmd.Name}: {cmd.Description}

                            用法:
                            {cmd.Usage}

                            """);
                        return 0;
                    }
                }

                Console.WriteLine($"命令 {commandName} 不存在！\n");
                return 1;
            }

            return 0;
        }
    }
}
