using System;
using System.Collections.Generic;
using Jscription.Runner.Classes;
using Jscription.Runner.Commands;

namespace Jscription.Runner
{
    internal class Program
    {
        // 注册所有可用的 CLI 命令
        public static readonly Dictionary<string, ICliCommand> _cliRegistry = new(StringComparer.OrdinalIgnoreCase)
        {
            { "run", new CommandRun() },
            { "version", new CommandVersion() },
            { "help", new CommandHelp() },
            { "compile", new CommandCompile() }
        };

        static int Main(string[] args)
        {
            Console.WriteLine($"Welcome to Jscription Runner {Global.Version}\n");

            if (args.Length == 0)
            {
                _cliRegistry["help"].Execute([]);
                return 0;
            }

            //提取第一个参数作为命令
            string primaryCommand = args[0];

            if (_cliRegistry.TryGetValue(primaryCommand, out var command))
            {
                string[] subArgs = new string[args.Length - 1];
                Array.Copy(args, 1, subArgs, 0, subArgs.Length);

                return command.Execute(subArgs);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"错误: 未知的指令 \"{primaryCommand}\"");
                Console.ResetColor();
                Console.WriteLine();
                _cliRegistry["help"].Execute([]);
                return 1;
            }
        }
    }
}