using Jscription.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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
        public int LineNumber { get; private set; }

        public abstract object? Run();

        public void Initialize(Dictionary<string, object>? args, string commandName, Dictionary<string, object> variables, string? returnVarName = null, int lineNumber = 0)
        {
            this.CommandName = commandName;
            this._globalVariables = variables;
            this._rawArgs = args;
            this.ReturnVarName = returnVarName;
            this.LineNumber = lineNumber;
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
                            if (rawValue is List<CmdRoot> compiledBlock)
                            {
                                prop.SetValue(this, compiledBlock);
                            }
                            else
                            {
                                var resolvedValue = ResolveVariable(rawValue, _globalVariables);

                                var token = Newtonsoft.Json.Linq.JToken.FromObject(resolvedValue);
                                var convertedValue = token.ToObject(prop.PropertyType);
                                prop.SetValue(this, convertedValue);
                            }
                        }
                        catch (JscriptionVariableNotFoundException) { throw; }
                        catch (Exception ex)
                        {
                            throw new JscriptionInvalidArgumentsException(
                                CommandName,
                                prop.Name,
                                $"参数类型不匹配。详情: {ex.Message}",
                                this.LineNumber
                            );
                        }
                    }
                }
            }

            object? result;
            try
            {
                result = Run();
            }
            catch (JscriptionParseException) { throw; }
            catch (Exception ex)
            {
                throw new JscriptionRuntimeException(this, ex.Message, ex);
            }

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
                if (variables == null) return strValue;

                if (strValue.StartsWith("$") && strValue.EndsWith("$") && strValue.Length > 2)
                {
                    if (strValue.IndexOf('$', 1) == strValue.Length - 1)
                    {
                        string varName = strValue.Substring(1, strValue.Length - 2);
                        if (variables.TryGetValue(varName, out var varValue))
                        {
                            return varValue;
                        }
                        throw new JscriptionVariableNotFoundException(varName);
                    }
                }

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
}
