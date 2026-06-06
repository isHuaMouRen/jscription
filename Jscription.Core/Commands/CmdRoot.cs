using Jscription.Core.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Jscription.Core.Commands
{
    //基类
    public abstract class CmdRoot
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _propertyCache = new();//反射缓存
        private Dictionary<string, PropertyInfo> GetCachedProperties()
        {
            var type = this.GetType();
            return _propertyCache.GetOrAdd(type, t =>
            {
                return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .ToDictionary(
                            p => p.Name,
                            p => p,
                            StringComparer.OrdinalIgnoreCase
                        );
            });
        }




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

        public object? Execute()
        {
            BindArguments();

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

            return result;
        }

        private void BindArguments()
        {
            if (_rawArgs == null) return;

            var insensitiveArgs = new Dictionary<string, object>(_rawArgs, StringComparer.OrdinalIgnoreCase);
            var properties = GetCachedProperties();

            foreach (var prop in properties.Values)
            {
                if (!insensitiveArgs.TryGetValue(prop.Name, out var rawValue))
                    continue;

                try
                {
                    if (rawValue is List<CmdRoot> compiledBlock)
                    {
                        prop.SetValue(this, compiledBlock);
                        continue;
                    }

                    object finalValue;

                    if (IsNestedCommandToken(rawValue))
                    {
                        finalValue = EvaluateNestedCommand(rawValue);
                    }
                    else
                    {
                        finalValue = ResolveVariable(rawValue, _globalVariables);
                    }

                    var token = JToken.FromObject(finalValue);
                    var convertedValue = token.ToObject(prop.PropertyType);
                    prop.SetValue(this, convertedValue);
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

        //递归求值，用于解析直接输入到参数内的命令
        private object EvaluateNestedCommand(object rawValue)
        {
            if (rawValue is JObject jObj)
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
                    var subArgs = new Dictionary<string, object>();
                    if (subArgsToken != null)
                    {
                        foreach (var argProp in subArgsToken.Properties())
                        {
                            subArgs[argProp.Name] = argProp.Value;
                        }
                    }

                    var tempCmd = CommandRegistry.CreateCommand(subCmdName, subArgs);
                    if (tempCmd != null)
                    {
                        tempCmd.Initialize(subArgs, subCmdName, _globalVariables ?? new(), null, this.LineNumber);

                        object? finalResult = tempCmd.Execute() ?? "";

                        if (_rawArgs != null)
                        {
                            foreach (var kp in new List<string>(_rawArgs.Keys))
                            {
                                if (_rawArgs[kp] == rawValue)
                                {
                                    _rawArgs[kp] = finalResult;
                                    break;
                                }
                            }
                        }

                        return finalResult;
                    }
                }
            }

            return rawValue;
        }

        private bool IsNestedCommandToken(object rawValue)
        {
            if (rawValue is JObject jObj)
            {
                foreach (var property in jObj.Properties())
                {
                    if (!property.Name.Equals("return", StringComparison.OrdinalIgnoreCase))
                    {
                        return CommandRegistry.CreateCommand(property.Name, null) != null;
                    }
                }
            }
            return false;
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
                if (IsNestedCommandToken(rawValue))
                {
                    return EvaluateNestedCommand(rawValue);
                }
                return ResolveVariable(rawValue, _globalVariables);
            }

            var properties = GetCachedProperties();
            if (properties.TryGetValue(key, out var prop))
            {
                return prop.GetValue(this);
            }

            return null;
        }
    }
}
