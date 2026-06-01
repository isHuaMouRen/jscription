using Jscription.Core.Classes;
using Jscription.Core.Commands;
using Jscription.Core.Exceptions;
using Newtonsoft.Json;

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
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(docContent);

                var doc = jObject.ToObject<JscriptionDoc>();
                if (doc == null)
                    throw new JscriptionParseException("JSON 反序列化结果为空，请检查脚本格式。");

                var commandsArray = jObject["commands"] as Newtonsoft.Json.Linq.JArray;
                if (commandsArray != null && doc.Commands != null)
                {
                    FillLineNumbers(commandsArray, doc.Commands);
                }

                return doc;
            }
            catch (JsonException ex)
            {
                throw new JscriptionParseException($"JSON 语法错误: {ex.Message}", ex);
            }
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

        private static void FillLineNumbers(Newtonsoft.Json.Linq.JArray? jArray, List<JscriptionDoc.CommandInfo> cmdList)
        {
            if (jArray == null) return;

            for (int i = 0; i < jArray.Count && i < cmdList.Count; i++)
            {
                var item = jArray[i] as Newtonsoft.Json.Linq.JObject;
                if (item == null) continue;

                var lineInfo = item as Newtonsoft.Json.IJsonLineInfo;
                if (lineInfo != null && lineInfo.HasLineInfo())
                {
                    cmdList[i].LineNumber = lineInfo.LineNumber;
                }

                if (item.TryGetValue("arguments", out var argsToken) && argsToken is Newtonsoft.Json.Linq.JObject argsObj)
                {
                    if (argsObj.TryGetValue("then", out var thenToken) && thenToken is Newtonsoft.Json.Linq.JArray thenArray && cmdList[i].Arguments != null)
                    {
                        //no anything...
                    }
                }
            }
        }
    }
}