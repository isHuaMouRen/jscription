using Jscription.Core.Compiler;
using Jscription.Core.Utils;
using Jscription.Runner.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Jscription.Runner.Commands
{
    internal class CommandCompile:ICliCommand
    {
        public string Name => "compile";
        public string Description => "将 Jscription 脚本编译为可执行的二进制文件";
        public string Usage => """
            格式:
            compile <参数> [脚本文件路径]

            compile --csharp-code   - 查看脚本翻译后的C#代码，仅查看。不做任何编译
            """;

        public int Execute(string[] args)
        {
            bool? csharp = null;
            string? filename = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("--csharp-code", StringComparison.OrdinalIgnoreCase))
                {
                    csharp = true;
                    if (i + 1 < args.Length)
                    {
                        filename = args[i + 1];
                        i++;
                    }
                    else
                    {
                        Logger.Error("错误: --csharp-code 参数后面缺少脚本文件路径。");
                        return 1;
                    }
                }
            }




            if (csharp == true)
            {
                var jsonContent = File.ReadAllText(filename!);
                var jscriptionDoc = JscriptionReader.ReadDoc(jsonContent);

                var compiler = new JscriptionCompiler();
                var result = compiler.TranspileToCSharp(jscriptionDoc!);

                Console.WriteLine(result);

                return 0;
            }


            return 0;

        }
    }
}
