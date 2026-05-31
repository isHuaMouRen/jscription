using Jscription.Core.Exceptions;
using System.Reflection;

namespace Jscription.Core.Commands
{
    //基类
    public abstract class CmdRoot
    {
        protected Dictionary<string, object>? _globalVariables;

        private Dictionary<string, object>? _rawArgs;

        public string CommandName { get; private set; } = "Unknown";

        public abstract void Run();

        public void Initialize(Dictionary<string, object>? args, string commandName, Dictionary<string, object> variables)
        {
            this.CommandName = commandName;
            this._globalVariables = variables;
            this._rawArgs = args;
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

            Run();
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

            public override void Run() => Console.Write(Message);
        }

        public class PrintLine : CmdRoot
        {
            public string? Message { get; set; }

            public override void Run() => Console.WriteLine(Message);
        }
    }

    //文件
    public class CmdFile
    {
        public class Write : CmdRoot
        {
            public required string path { get; set; }
            public string? content { get; set; }

            public override void Run() => File.WriteAllText(path, content);
        }
        
        public class Delete : CmdRoot
        {
            public required string path { get; set; }

            public override void Run() => File.Delete(path);
        }
    }

    //变量
    public class CmdVariable
    {
        public class Set : CmdRoot
        {
            public required string VarName { get; set; }
            public required object Value { get; set; }

            public override void Run()
            {
                if (_globalVariables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                _globalVariables[VarName] = Value;
            }
        }
    }
}
