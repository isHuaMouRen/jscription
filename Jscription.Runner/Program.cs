using System;
using System.IO;
using Jscription.Core.Utils;
using Jscription.Core.Exceptions;

namespace Jscription.Runner
{
    internal class Program
    {
        static int Main(string[] args)
        {
            string? sourcePath = null;

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--source":
                            if (i + 1 < args.Length)
                            {
                                sourcePath = args[i + 1];
                                i++;
                            }
                            else
                            {
                                LogError("错误: --source 参数后面缺少脚本文件路径。");
                                return 1;
                            }
                            break;
                    }
                }

                if (string.IsNullOrEmpty(sourcePath))
                {
                    LogError("错误: 未指定输入脚本。");
                    return 1;
                }

                if (!File.Exists(sourcePath))
                {
                    LogError($"错误: 找不到指定的脚本文件 -> \"{Path.GetFullPath(sourcePath)}\"");
                    return 1;
                }

                // 解析与执行
                var jsonContent = File.ReadAllText(sourcePath);
                var jscriptionDoc = JscriptionReader.ReadDoc(jsonContent);
                var jscriptionExecuteInfo = JscriptionReader.AnalysisDoc(jscriptionDoc);

                // 运行
                var executer = new JscriptionExecuter { ExecuteInfo = jscriptionExecuteInfo };
                executer.Run();
            }
            catch (JscriptionParseException ex)
            {
                LogError($"[Jscription 脚本错误] {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                LogError($"[运行时未知崩溃] {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }

            return 0;
        }

        private static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}