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
                var doc = JsonConvert.DeserializeObject<JscriptionDoc>(docContent);
                if (doc == null)
                    throw new JscriptionParseException("JSON 反序列化结果为空，请检查脚本格式。");
                return doc;
            }
            catch (JsonException ex)
            {
                // 捕获 Newtonsoft.Json 的异常，包装成你自己的解析异常
                throw new JscriptionParseException($"JSON 语法错误: {ex.Message}", ex);
            }
        }

        public static JscriptionExecutInfo AnalysisDoc(JscriptionDoc? doc)
        {
            //基础检查
            if (doc == null)
                throw new Exception("脚本不能为空");

            if (doc.Name == null)
                throw new JscriptionMissingFieldException("name", "脚本名字不能为空，请使用 \"name\" 属性");

            if (doc.Commands == null)
                throw new JscriptionMissingFieldException("commands", "脚本不能没有命令列表，即使没有命令也要使用 \"commands\": []");

            var cmdList = new List<CmdRoot>();
            foreach (var cmd in doc.Commands)
            {
                //转换命令
                var parsedCmd = ConvertStringToCmd(cmd.Command, cmd.Arguments);

                if (parsedCmd == null)
                    throw new JscriptionUnknownCommandException(cmd.Command);

                parsedCmd.Initialize(cmd.Arguments, cmd.Command ?? parsedCmd.GetType().Name);

                cmdList.Add(parsedCmd);
            }

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