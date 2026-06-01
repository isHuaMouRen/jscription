using Jscription.Core.Classes;
using Jscription.Core.Exceptions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Jscription.Core.Commands
{
    //基类
    public abstract class CmdRoot
    {
        protected Dictionary<string, object>? _globalVariables;
        private Dictionary<string, object>? _rawArgs;

        public string CommandName { get; private set; } = "Unknown";
        public string? ReturnVarName { get; private set; }

        public abstract object? Run();

        public void Initialize(Dictionary<string, object>? args, string commandName, Dictionary<string, object> variables, string? returnVarName = null)
        {
            this.CommandName = commandName;
            this._globalVariables = variables;
            this._rawArgs = args;
            this.ReturnVarName = returnVarName;
        }

        public void Execute()
        {
            if (_rawArgs != null)
            {
                var insensitiveArgs = new Dictionary<string, object>(_rawArgs, StringComparer.OrdinalIgnoreCase);
                var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    if (insensitiveArgs.TryGetValue(prop.Name, out var rawValue))
                    {
                        try
                        {
                            var resolvedValue = ResolveVariable(rawValue, _globalVariables);

                            var token = Newtonsoft.Json.Linq.JToken.FromObject(resolvedValue);
                            var convertedValue = token.ToObject(prop.PropertyType);
                            prop.SetValue(this, convertedValue);
                        }
                        catch (JscriptionVariableNotFoundException) { throw; }
                        catch (Exception ex)
                        {
                            throw new JscriptionInvalidArgumentsException(
                                CommandName,
                                prop.Name,
                                $"参数类型不匹配。详情: {ex.Message}"
                            );
                        }
                    }
                }
            }

            object? result = Run();

            if (!string.IsNullOrWhiteSpace(ReturnVarName) && _globalVariables != null)
            {
                _globalVariables[ReturnVarName] = result!;
            }
        }

        private object ResolveVariable(object rawValue, Dictionary<string, object>? variables)
        {
            if (rawValue is string strValue)
            {
                strValue = strValue.Trim();

                if (strValue.StartsWith("$") && strValue.EndsWith("$") && strValue.Length > 2)
                {
                    string varName = strValue.Substring(1, strValue.Length - 2);
                    if (variables != null && variables.TryGetValue(varName, out var varValue))
                    {
                        return varValue;
                    }
                    throw new JscriptionVariableNotFoundException(varName);
                }

                if (variables != null)
                {
                    int startIndex = 0;
                    while ((startIndex = strValue.IndexOf('$', startIndex)) != -1)
                    {
                        int endIndex = strValue.IndexOf('$', startIndex + 1);
                        if (endIndex == -1) break;

                        string varName = strValue.Substring(startIndex + 1, endIndex - startIndex - 1);
                        if (variables.TryGetValue(varName, out var varValue))
                        {
                            string placeholder = $"${varName}$";
                            strValue = strValue.Replace(placeholder, varValue?.ToString() ?? "");
                            startIndex = 0;
                        }
                        else
                        {
                            throw new JscriptionVariableNotFoundException(varName);
                        }
                    }
                    return strValue;
                }
            }

            return rawValue;
        }

        protected bool EvaluateCondition(object? condition)
        {
            if (condition == null) return false;

            if (condition is bool b) return b;

            string condStr = condition.ToString()?.Trim() ?? "";
            if (string.IsNullOrEmpty(condStr)) return false;

            if (bool.TryParse(condStr, out bool parsedBool)) return parsedBool;

            //支持操作符 == != >= <= > <
            var match = Regex.Match(condStr, @"^(.+?)\s*(==|!=|>=|<=|>|<)\s*(.+)$");
            if (!match.Success)
            {
                return false;
            }

            string left = match.Groups[1].Value.Trim();
            string op = match.Groups[2].Value;
            string right = match.Groups[3].Value.Trim();

            if (double.TryParse(left, out double numL) && double.TryParse(right, out double numR))
            {
                return op switch
                {
                    "==" => numL == numR,
                    "!=" => numL != numR,
                    ">" => numL > numR,
                    "<" => numL < numR,
                    ">=" => numL >= numR,
                    "<=" => numL <= numR,
                    _ => false
                };
            }

            return op switch
            {
                "==" => left.Equals(right, StringComparison.OrdinalIgnoreCase),
                "!=" => !left.Equals(right, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        protected object? GetDynamicArgument(string key)
        {
            if (_rawArgs != null && _rawArgs.TryGetValue(key, out var rawValue))
            {
                return ResolveVariable(rawValue, _globalVariables);
            }
            var prop = this.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return prop?.GetValue(this);
        }
    }

    //==========================================================================================
    //命令实现
    //==========================================================================================
    //控制台
    public class CmdConsole
    {
        public class Print : CmdRoot
        {
            public string? Message { get; set; }

            public override object? Run() { Console.Write(Message); return null; }
        }

        public class PrintLine : CmdRoot
        {
            public string? Message { get; set; }

            public override object? Run() { Console.WriteLine(Message); return null; }
        }
    }

    //文件
    public class CmdFile
    {
        public class Write : CmdRoot
        {
            public required string path { get; set; }
            public string? content { get; set; }

            public override object? Run() { File.WriteAllText(path, content); return null; }
        }

        public class Delete : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run() { File.Delete(path); return null; }
        }

        public class Read : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run()
            {
                return File.ReadAllText(path);
            }
        }
    }

    //变量
    public class CmdVariable
    {
        public class Set : CmdRoot
        {
            public required string VarName { get; set; }
            public required object Value { get; set; }

            public override object? Run()
            {
                if (_globalVariables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                _globalVariables[VarName] = Value;

                return null;
            }
        }
    }

    //流程控制
    public class CmdControl
    {
        public class Sleep : CmdRoot
        {
            public required int time { get; set; }

            public override object? Run()
            {
                Thread.Sleep(time);
                return null;
            }
        }

        public class If : CmdRoot
        {
            public required object condition { get; set; }

            public List<JscriptionDoc.CommandInfo>? then { get; set; }
            public List<JscriptionDoc.CommandInfo>? @else { get; set; }//else为保留关键字，@以作为变量名

            public override object? Run()
            {
                if (_globalVariables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                bool isTrue = EvaluateCondition(condition);
                var targetCommands = isTrue ? then : @else;

                if (targetCommands != null)
                {
                    foreach (var cmdInfo in targetCommands)
                    {
                        var subCmd = CommandRegistry.CreateCommand(cmdInfo.Command, cmdInfo.Arguments);
                        if (subCmd == null)
                            throw new Exception($"If 内部包含未知的命令类型: \"{cmdInfo.Command}\"");

                        string subCmdName = cmdInfo.Command ?? subCmd.GetType().Name;
                        subCmd.Initialize(cmdInfo.Arguments, subCmdName, _globalVariables, cmdInfo.Return);
                        subCmd.Execute();
                    }
                }

                return null;
            }
        }

        public class Loop : CmdRoot
        {
            public object? condition { get; set; }
            public List<JscriptionDoc.CommandInfo>? @do { get; set; }

            public override object? Run()
            {
                if (_globalVariables == null) throw new Exception($"命令 [{CommandName}] 运行时丢失上下文。");
                if (@do == null || @do.Count == 0) return null;

                while (true)
                {
                    var currentCondition = GetDynamicArgument(nameof(condition));

                    if (!EvaluateCondition(currentCondition))
                    {
                        break;
                    }

                    foreach (var cmdInfo in @do)
                    {
                        var subCmd = CommandRegistry.CreateCommand(cmdInfo.Command, cmdInfo.Arguments);
                        if (subCmd == null) throw new Exception($"Loop 内部包含未知命令: \"{cmdInfo.Command}\"");

                        string subCmdName = cmdInfo.Command ?? subCmd.GetType().Name;
                        subCmd.Initialize(cmdInfo.Arguments, subCmdName, _globalVariables, cmdInfo.Return);
                        subCmd.Execute();
                    }
                }
                return null;
            }
        }
    }
}
