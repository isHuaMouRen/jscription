using Jscription.Core.Exceptions;
using Jscription.Core.Utils;
using Jscription.Runner.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Runner.Commands
{
    internal class CommandRun : ICliCommand
    {
        public string Name => "run";
        public string Description => "运行一个 Jscription 脚本文件";
        public string Usage => """
            run --source <文件路径>.json   - 立即执行指定的脚本
            """;

        public int Execute(string[] args)
        {
            string? sourcePath = null;

            // 解析当前命令特有的参数
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("--source", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length)
                    {
                        sourcePath = args[i + 1];
                        i++;
                    }
                    else
                    {
                        Logger.Error("错误: --source 参数后面缺少脚本文件路径。");
                        return 1;
                    }
                }
            }

            if (string.IsNullOrEmpty(sourcePath))
            {
                Logger.Error("错误: 未指定脚本路径。请使用 run --source <路径>");
                return 1;
            }

            if (!File.Exists(sourcePath))
            {
                Logger.Error($"错误: 找不到指定的脚本文件 -> \"{Path.GetFullPath(sourcePath)}\"");
                return 1;
            }

            try
            {
                var jsonContent = File.ReadAllText(sourcePath);
                var jscriptionDoc = JscriptionReader.ReadDoc(jsonContent);
                var jscriptionExecuteInfo = JscriptionReader.AnalysisDoc(jscriptionDoc);

                var executer = new JscriptionExecuter { ExecuteInfo = jscriptionExecuteInfo };
                executer.Run();
            }
            catch (JscriptionParseException ex)
            {
                Logger.Error($"[Jscription 脚本错误] {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Logger.Error($"[运行时未知崩溃] {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }

            return 0;
        }
    }
}
