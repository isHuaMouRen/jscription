using Jscription.Core.Commands;

namespace Jscription.Core.Exceptions
{
    //基类
    public class JscriptionParseException : Exception
    {
        public JscriptionParseException(string message) : base(message) { }
        public JscriptionParseException(string message, Exception innerException) : base(message, innerException) { }
    }

    //JSON缺少核心字段
    public class JscriptionMissingFieldException : JscriptionParseException
    {
        public string MissingField { get; }
        public JscriptionMissingFieldException(string fieldName, string message) : base(message)
        {
            MissingField = fieldName;
        }
    }

    //无法识别的命令
    public class JscriptionUnknownCommandException : JscriptionParseException
    {
        public string? CommandName { get; }
        public JscriptionUnknownCommandException(string? commandName)
            : base($"未知的命令类型: \"{commandName}\"，请检查拼写或是否未注册该命令。")
        {
            CommandName = commandName;
        }
    }

    //命令参数错误（缺失或类型不对）
    public class JscriptionInvalidArgumentsException : JscriptionParseException
    {
        public string CommandName { get; }
        public string ArgumentKey { get; }

        public JscriptionInvalidArgumentsException(string commandName, string argumentKey, string message)
            : base($"命令 [{commandName}] 的参数 [{argumentKey}] 错误: {message}")
        {
            CommandName = commandName;
            ArgumentKey = argumentKey;
        }
    }

    //脚本运行时错误
    public class JscriptionRuntimeException : JscriptionParseException
    {
        public CmdRoot? Command { get; }

        public JscriptionRuntimeException(CmdRoot? command, string message, Exception innerException)
            : base($"脚本在执行命令 [{command?.CommandName}] 时发生错误：{message}", innerException)
        {
            Command = command;
        }
    }

    //找不到变量
    public class JscriptionVariableNotFoundException : JscriptionParseException
    {
        public string VariableName { get; }
        public JscriptionVariableNotFoundException(string variableName)
            : base($"未找到指定的变量: \"${variableName}$\"，请检查变量列表中是否定义了该变量。")
        {
            VariableName = variableName;
        }
    }
}
