using Jscription.Core.Compiler;
using Jscription.Core.Utils;
using Jscription.Runner.Utils;

namespace Jscription.Runner.Commands
{
    internal class CommandCompile : ICliCommand
    {
        public string Name => "compile";
        public string Description => "将 Jscription 脚本编译为可执行的二进制文件";
        public string Usage => """
            格式:
              compile [脚本文件路径]
              compile --csharp-code [脚本文件路径]

            参数说明:
              compile --csharp-code   - 仅查看脚本翻译后的 C# 源代码，不生成二进制文件。
              compile   - 编译为二进制文件
            """;

        public int Execute(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Logger.Error("错误: 缺少必要参数或脚本文件路径。");
                Console.WriteLine(Usage);
                return 1;
            }

            bool onlyViewCSharp = false;
            string? filename = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("--csharp-code", StringComparison.OrdinalIgnoreCase))
                {
                    onlyViewCSharp = true;
                }
                else if (args[i].StartsWith("-"))
                {
                    Logger.Error($"错误: 无法识别的编译参数 '{args[i]}'");
                    return 1;
                }
                else
                {
                    filename = args[i];
                }
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                Logger.Error("错误: 未指定 Jscription 脚本文件的路径。");
                return 1;
            }

            if (!File.Exists(filename))
            {
                Logger.Error($"错误: 找不到指定的脚本文件 -> \"{filename}\"");
                return 1;
            }

            try
            {
                string jsonContent = File.ReadAllText(filename);
                var jscriptionDoc = JscriptionReader.ReadDoc(jsonContent);

                if (jscriptionDoc == null)
                {
                    Logger.Error("错误: 脚本反序列化失败，请检查 JSON 结构。");
                    return 1;
                }

                var compiler = new JscriptionCompiler();
                string csharpCode = compiler.TranspileToCSharp(jscriptionDoc);

                if (onlyViewCSharp)
                {
                    Console.WriteLine(csharpCode);
                    return 0;
                }

                string outputExeName = !string.IsNullOrWhiteSpace(jscriptionDoc.Name)
                    ? $"{jscriptionDoc.Name}.exe"
                    : $"{Path.GetFileNameWithoutExtension(filename)}.exe";

                string outputExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputExeName);


                Logger.Warn("[Compile] 编译功能目前仍在测试阶段，出现错误是正常的");
                Console.WriteLine($"[Compile] 正在将 '{jscriptionDoc.Name}' 编译为控制台程序...");

                var binaryCompiler = new JscriptionBinaryCompiler();
                if (binaryCompiler.CompileToFile(csharpCode, outputExePath, out var errors))
                {
                    Console.WriteLine($"[成功] 二进制文件已输出至: {outputExePath}");
                    return 0;
                }
                else
                {
                    Logger.Error("[失败] Dotnet CLI 拒绝了这段代码，编译错误详情：");
                    foreach (var err in errors)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"  -> {err}");
                    }
                    Console.ResetColor();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[Jscription 编译期硬件崩溃]: {ex.Message}");
                return 1;
            }
        }
    }
}