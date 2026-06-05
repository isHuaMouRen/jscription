/*

using Jscription.Core.Classes;
using Jscription.Core.Utils;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Jscription.Core.Compiler
{
    public class JscriptionCompiler
    {
        public string TranspileToCSharp(JscriptionDoc doc)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            var sb = new StringBuilder();

            sb.Append($$$"""
            //---------------------------------------------------------
            // 此代码由 Jscription Compiler 自动生成
            // 脚本名: {{{doc.Name}}}
            //---------------------------------------------------------
            using System;
            using System.IO;
            using System.Threading;
            using System.Collections.Generic;

            class Program
            {
                static Dictionary<string, object> _globalVariables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                static void Main(string[] args)
                {
                    try
                    {
            
            """);

            // 注入变量定义
            if (doc.Variables != null)
            {
                foreach (var varPair in doc.Variables)
                {
                    string safeValue = FormatLiteralValue(varPair.Value);
                    sb.AppendLine($"            _globalVariables[\"{varPair.Key}\"] = {safeValue};");
                }
                sb.AppendLine();
            }

            // 递归解析核心命令
            if (doc.Commands != null)
            {
                TranspileCommands(doc.Commands, sb, indentLevel: 3);
            }

            sb.Append("""
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[Jscription 运行时硬件崩溃]: {ex.Message}");
                            Console.ResetColor();
                        }
                    }
            """);

            // 注入辅助运行方法
            InjectHelperMethods(sb);

            sb.AppendLine("}");

            return sb.ToString();
        }

        private void TranspileCommands(List<JscriptionDoc.CommandInfo> commands, StringBuilder sb, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);

            foreach (var cmd in commands)
            {
                if (string.IsNullOrWhiteSpace(cmd.Command)) continue;

                var processedArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (cmd.Arguments != null)
                {
                    foreach (var arg in cmd.Arguments)
                    {
                        processedArgs[arg.Key] = EmitArgumentValue(arg.Value);
                    }
                }

                string? csharpCodeLine = cmd.Command.ToLower() switch
                {
                    "console.write" => $"Console.Write(ResolveVar({processedArgs.GetValueOrDefault("message", "\"\"")}));",
                    "console.writeline" => $"Console.WriteLine(ResolveVar({processedArgs.GetValueOrDefault("message", "\"\"")}));",
                    "console.readline" => "Console.ReadLine();",
                    "console.setcolor" => $"if (Enum.TryParse(ResolveVar({processedArgs.GetValueOrDefault("color", "\"\"")}).ToString(), true, out ConsoleColor cc)) Console.ForegroundColor = cc; else throw new Exception(\"未找到控制台颜色\");",
                    "console.getcolor" => "Console.ForegroundColor.ToString();",

                    "file.write" => $"File.WriteAllText(ResolveVar({processedArgs.GetValueOrDefault("path", "\"\"")}).ToString(), ResolveVar({processedArgs.GetValueOrDefault("content", "\"\"")})?.ToString());",
                    "file.read" => $"File.ReadAllText(ResolveVar({processedArgs.GetValueOrDefault("path", "\"\"")}).ToString());",
                    "file.delete" => $"File.Delete(ResolveVar({processedArgs.GetValueOrDefault("path", "\"\"")}).ToString());",
                    "file.exists" => $"File.Exists(ResolveVar({processedArgs.GetValueOrDefault("path", "\"\"")}).ToString());",

                    "dir.create" => $"Directory.CreateDirectory(ResolveVar({processedArgs.GetValueOrDefault("path", "\"\"")}).ToString());",
                    "dir.exists" => $"Directory.Exists(ResolveVar({processedArgs.GetValueOrDefault("path", "\"\"")}).ToString());",
                    "dir.delete" => processedArgs.ContainsKey("recursive")
                        ? $"Directory.Delete(ResolveVar({processedArgs["path"]}).ToString(), Convert.ToBoolean(ResolveVar({processedArgs["recursive"]})));"
                        : $"Directory.Delete(ResolveVar({processedArgs["path"]}).ToString());",

                    "variable.set" => $"_globalVariables[ResolveVar({processedArgs.GetValueOrDefault("varName", "\"\"")}).ToString()] = ResolveVar({processedArgs.GetValueOrDefault("value", "null")});",
                    "variable.get" => $"_globalVariables.TryGetValue(ResolveVar({processedArgs.GetValueOrDefault("varName", "\"\"")}).ToString(), out var v) ? v : throw new Exception(\"变量未找到\");",

                    "control.sleep" => $"Thread.Sleep(Convert.ToInt32(ResolveVar({processedArgs.GetValueOrDefault("time", "0")})));",

                    "control.if" => null,
                    "control.loop" => null,

                    _ => $"throw new Exception(\"编译器未实现的命令: {cmd.Command}\");"
                };

                if (csharpCodeLine != null)
                {
                    if (!string.IsNullOrWhiteSpace(cmd.Return))
                    {
                        sb.AppendLine($"{indent}_globalVariables[\"{cmd.Return}\"] = {csharpCodeLine.TrimEnd(';')};");
                    }
                    else
                    {
                        sb.AppendLine($"{indent}{csharpCodeLine}");
                    }
                }
                else
                {
                    if (cmd.Command.Equals("control.if", StringComparison.OrdinalIgnoreCase))
                    {
                        string cond = processedArgs.GetValueOrDefault("condition", "false");
                        sb.AppendLine($"{indent}if (EvaluateConditionExpression(ResolveVar({cond})))");
                        sb.AppendLine($"{indent}{{");

                        if (cmd.Arguments != null && cmd.Arguments.TryGetValue("then", out var thenVal) && thenVal is JArray thenArray)
                        {
                            var subInfos = ParseSubCommands(thenArray);
                            TranspileCommands(subInfos, sb, indentLevel + 1);
                        }
                        sb.AppendLine($"{indent}}}");

                        if (cmd.Arguments != null && cmd.Arguments.TryGetValue("else", out var elseVal) && elseVal is JArray elseArray)
                        {
                            sb.AppendLine($"{indent}else");
                            sb.AppendLine($"{indent}{{");
                            var subInfos = ParseSubCommands(elseArray);
                            TranspileCommands(subInfos, sb, indentLevel + 1);
                            sb.AppendLine($"{indent}}}");
                        }
                    }
                    else if (cmd.Command.Equals("control.loop", StringComparison.OrdinalIgnoreCase))
                    {
                        string cond = processedArgs.GetValueOrDefault("condition", "false");
                        sb.AppendLine($"{indent}while (EvaluateConditionExpression(ResolveVar({cond})))");
                        sb.AppendLine($"{indent}{{");

                        if (cmd.Arguments != null && cmd.Arguments.TryGetValue("do", out var doVal) && doVal is JArray doArray)
                        {
                            var subInfos = ParseSubCommands(doArray);
                            TranspileCommands(subInfos, sb, indentLevel + 1);
                        }
                        sb.AppendLine($"{indent}}}");
                    }
                }
            }
        }

        private string EmitArgumentValue(object argValue)
        {
            if (argValue is JObject jObj)
            {
                string? subCmdName = null;
                JObject? subArgsToken = null;

                foreach (var property in jObj.Properties())
                {
                    if (!property.Name.Equals("return", StringComparison.OrdinalIgnoreCase))
                    {
                        subCmdName = property.Name;
                        subArgsToken = property.Value as JObject;
                        break;
                    }
                }

                if (subCmdName != null)
                {
                    var subArgs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    if (subArgsToken != null)
                    {
                        foreach (var argProp in subArgsToken.Properties())
                        {
                            subArgs[argProp.Name] = EmitArgumentValue(argProp.Value);
                        }
                    }

                    return subCmdName.ToLower() switch
                    {
                        "file.read" => $"File.ReadAllText(ResolveVar({subArgs.GetValueOrDefault("path", "\"\"")}).ToString())",
                        "file.exists" => $"File.Exists(ResolveVar({subArgs.GetValueOrDefault("path", "\"\"")}).ToString())",
                        "dir.exists" => $"Directory.Exists(ResolveVar({subArgs.GetValueOrDefault("path", "\"\"")}).ToString())",
                        "console.readline" => "Console.ReadLine()",
                        "variable.get" => $"(_globalVariables.TryGetValue(ResolveVar({subArgs.GetValueOrDefault("varName", "\"\"")}).ToString(), out var v) ? v : throw new Exception(\"变量未找到\"))",
                        _ => "null"
                    };
                }
            }

            // 最底层的字面量，通过这里穿上双引号
            return FormatLiteralValue(argValue);
        }

        private string FormatLiteralValue(object val)
        {
            if (val is string str) return $"\"{str.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
            if (val is bool b) return b ? "true" : "false";
            return val?.ToString() ?? "null";
        }

        private List<JscriptionDoc.CommandInfo> ParseSubCommands(JArray jArray)
        {
            var method = typeof(JscriptionReader).GetMethod("ParseCommandsArray", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (List<JscriptionDoc.CommandInfo>)method!.Invoke(null, new object[] { jArray })!;
        }

        private void InjectHelperMethods(StringBuilder sb)
        {
            sb.AppendLine("""""
                static object ResolveVar(object rawValue)
                {
                    if (rawValue is string strValue)
                    {
                        strValue = strValue.Trim();
                        if (strValue.StartsWith("$") && strValue.EndsWith("$") && strValue.Length > 2)
                        {
                            if (strValue.IndexOf('$', 1) == strValue.Length - 1)
                            {
                                string varName = strValue.Substring(1, strValue.Length - 2);
                                if (_globalVariables.TryGetValue(varName, out var varValue)) return varValue;
                                throw new Exception($"未找到变量: ${varName}$");
                            }
                        }
                        int startIndex = 0;
                        while ((startIndex = strValue.IndexOf('$', startIndex)) != -1)
                        {
                            int endIndex = strValue.IndexOf('$', startIndex + 1);
                            if (endIndex == -1) break;
                            string varName = strValue.Substring(startIndex + 1, endIndex - startIndex - 1);
                            if (_globalVariables.TryGetValue(varName, out var varValue))
                            {
                                strValue = strValue.Replace($"${varName}$", varValue?.ToString() ?? "");
                                startIndex = 0;
                            }
                            else { throw new Exception($"未找到变量: ${varName}$"); }
                        }
                        return strValue;
                    }
                    return rawValue;
                }

                static bool EvaluateConditionExpression(object condition)
                {
                    if (condition == null) return false;
                    if (condition is bool b) return b;
                    string condStr = condition.ToString()?.Trim() ?? "";
                    if (string.IsNullOrEmpty(condStr)) return false;
                    if (bool.TryParse(condStr, out bool parsedBool)) return parsedBool;

                    var match = System.Text.RegularExpressions.Regex.Match(condStr, @"^(.+?)\s*(==|!=|>=|<=|>|<)\s*(.+)$");
                    if (!match.Success) return false;

                    string left = match.Groups[1].Value.Trim();
                    string op = match.Groups[2].Value;
                    string right = match.Groups[3].Value.Trim();

                    if (double.TryParse(left, out double numL) && double.TryParse(right, out double numR))
                    {
                        return op switch {
                            "==" => numL == numR, "!=" => numL != numR, ">" => numL > numR, "<" => numL < numR, ">=" => numL >= numR, "<=" => numL <= numR, _ => false
                        };
                    }
                    return op switch {
                        "==" => left.Equals(right, StringComparison.OrdinalIgnoreCase), "!=" => !left.Equals(right, StringComparison.OrdinalIgnoreCase), _ => false
                    };
                }
            """"");
        }
    }
}

*/