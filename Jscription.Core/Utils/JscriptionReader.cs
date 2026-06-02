using Jscription.Core.Classes;
using Jscription.Core.Commands;
using Jscription.Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Jscription.Core.Utils
{
    public class JscriptionReader
    {
        public static JscriptionDoc? ReadDoc(string docContent)
        {
            if (string.IsNullOrWhiteSpace(docContent))
                throw new ArgumentException("脚本内容不能为空。", nameof(docContent));

            try
            {
                var jObject = JObject.Parse(docContent);

                var doc = new JscriptionDoc
                {
                    Name = jObject["name"]?.ToString(),
                    Variables = jObject["variables"]?.ToObject<Dictionary<string, object>>(),
                    Commands = new List<JscriptionDoc.CommandInfo>()
                };

                // 解析核心的 commands 数组
                if (jObject["commands"] is JArray commandsArray)
                {
                    doc.Commands = ParseCommandsArray(commandsArray);
                }

                return doc;
            }
            catch (JsonException ex)
            {
                throw new JscriptionParseException($"JSON 语法错误: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 支持新旧语法双重嗅探的通用解析方法
        /// </summary>
        private static List<JscriptionDoc.CommandInfo> ParseCommandsArray(JArray jArray)
        {
            var resultList = new List<JscriptionDoc.CommandInfo>();

            foreach (var item in jArray)
            {
                if (item is not JObject cmdOuterObj) continue;

                var cmdInfo = new JscriptionDoc.CommandInfo();

                if (cmdOuterObj is IJsonLineInfo lineInfo && lineInfo.HasLineInfo())
                {
                    cmdInfo.LineNumber = lineInfo.LineNumber;
                }

                JObject? argsToken = null;

                //旧语法兼容
                if (cmdOuterObj.ContainsKey("command"))
                {
                    cmdInfo.Command = cmdOuterObj["command"]?.ToString();
                    cmdInfo.Return = cmdOuterObj["return"]?.ToString();
                    argsToken = cmdOuterObj["arguments"] as JObject;
                }
                else//新语法
                {
                    foreach (var property in cmdOuterObj.Properties())
                    {
                        if (property.Name.Equals("return", StringComparison.OrdinalIgnoreCase))
                        {
                            cmdInfo.Return = property.Value.ToString();
                        }
                        else
                        {
                            cmdInfo.Command = property.Name;
                            argsToken = property.Value as JObject;
                        }
                    }
                }

                if (cmdInfo.Command != null)
                {
                    cmdInfo.Arguments = new Dictionary<string, object>();

                    if (argsToken != null)
                    {
                        foreach (var argProp in argsToken.Properties())
                        {
                            string argKey = argProp.Name;

                            if ((argKey.Equals("then", StringComparison.OrdinalIgnoreCase) ||
                                 argKey.Equals("else", StringComparison.OrdinalIgnoreCase) ||
                                 argKey.Equals("do", StringComparison.OrdinalIgnoreCase))
                                && argProp.Value is JArray subArray)
                            {
                                cmdInfo.Arguments[argKey] = ParseCommandsArray(subArray);
                            }
                            else
                            {
                                cmdInfo.Arguments[argKey] = argProp.Value.ToObject<object>()!;
                            }
                        }
                    }

                    resultList.Add(cmdInfo);
                }
            }

            return resultList;
        }

        public static JscriptionExecutInfo AnalysisDoc(JscriptionDoc? doc)
        {
            //基础检查
            if (doc == null) throw new Exception("脚本不能为空");
            if (doc.Name == null) throw new JscriptionMissingFieldException("name", "脚本名字不能为空，请使用 \"name\" 属性");
            if (doc.Commands == null) throw new JscriptionMissingFieldException("commands", "脚本不能没有命令列表，即使没有命令也要使用 \"commands\": []");

            var variables = doc.Variables ?? new Dictionary<string, object>();
            var cmdList = new List<CmdRoot>();

            void ProcessCommandInfos(List<JscriptionDoc.CommandInfo> infos, List<CmdRoot> targetList)
            {
                foreach (var cmd in infos)
                {
                    var parsedCmd = ConvertStringToCmd(cmd.Command, cmd.Arguments);
                    if (parsedCmd == null)
                        throw new JscriptionUnknownCommandException(cmd.Command);

                    string cmdName = cmd.Command ?? parsedCmd.GetType().Name;

                    //注入LineNumber!!!
                    parsedCmd.Initialize(cmd.Arguments, cmdName, variables, cmd.Return, cmd.LineNumber);
                    targetList.Add(parsedCmd);
                }
            }

            ProcessCommandInfos(doc.Commands, cmdList);

            return new JscriptionExecutInfo
            {
                Name = doc.Name,
                Commands = cmdList
            };
        }

        private static CmdRoot? ConvertStringToCmd(string? cmd, Dictionary<string, object>? args)
            => CommandRegistry.CreateCommand(cmd, args);
    }
}