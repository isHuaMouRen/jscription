using Jscription.Core.Exceptions;

namespace Jscription.Core.Commands
{
    //基类
    public abstract class CmdRoot
    {
        public abstract void Run();

        public abstract void Initialize(Dictionary<string, object>? args);
    }


    /// <summary>
    /// 打印
    /// </summary>
    public class CmdPrint : CmdRoot
    {
        public string? Message { get; set; }

        public override void Initialize(Dictionary<string, object>? args)
        {
            if (args == null || !args.ContainsKey("message"))
                throw new JscriptionInvalidArgumentsException("print", "message", "缺少必要的 'message' 参数。");
            if (args["message"] is not string messageStr)
                throw new JscriptionInvalidArgumentsException("print", "message", "参数类型必须是字符串。");

            Message = messageStr;
        }

        public override void Run() => Console.Write(Message);
    }
}
