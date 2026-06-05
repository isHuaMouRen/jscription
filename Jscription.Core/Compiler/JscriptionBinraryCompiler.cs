/*

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Jscription.Core.Compiler
{
    public class JscriptionBinaryCompiler
    {
        public bool CompileToFile(string csharpCode, string outputExePath, out List<string> errors)
        {
            errors = new List<string>();

            string tempProjectDir = Path.Combine(Path.GetTempPath(), $"Jscription_Build_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempProjectDir);

            try
            {
                string csprojContent = """
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>disable</ImplicitUsings>
    <Nullable>disable</Nullable>
    
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    
    <PublishTrimmed>true</PublishTrimmed>
    <TrimMode>link</TrimMode>
    
    <InvariantGlobalization>true</InvariantGlobalization>
    <StackTraceSupport>false</StackTraceSupport>
  </PropertyGroup>
</Project>
""";

                File.WriteAllText(Path.Combine(tempProjectDir, "jscription_compile.csproj"), csprojContent);
                File.WriteAllText(Path.Combine(tempProjectDir, "Program.cs"), csharpCode);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish jscription_compile.csproj -c Release -r win-x64 -o \"{tempProjectDir}/out\"",
                    WorkingDirectory = tempProjectDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();

                    string stdout = process.StandardOutput.ReadToEnd();
                    string stderr = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        errors.Add($"[Dotnet CLI 拒绝编译] 错误码: {process.ExitCode}");
                        if (!string.IsNullOrWhiteSpace(stderr)) errors.Add(stderr.Trim());
                        if (!string.IsNullOrWhiteSpace(stdout)) errors.Add(stdout.Trim());
                        return false;
                    }
                }

                string compiledExePath = Path.Combine(tempProjectDir, "out", "jscription_compile.exe");

                if (!File.Exists(compiledExePath))
                {
                    errors.Add("错误: 找不到生成的二进制目标文件。");
                    return false;
                }

                if (File.Exists(outputExePath)) File.Delete(outputExePath);
                File.Move(compiledExePath, outputExePath);

                return true;
            }
            catch (Exception ex)
            {
                errors.Add($"[编译管道内部崩溃]: {ex.Message}");
                return false;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempProjectDir))
                    {
                        Directory.Delete(tempProjectDir, true);
                    }
                }
                catch {  }
            }
        }
    }
}

*/