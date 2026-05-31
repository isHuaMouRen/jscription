using Jscription.Core.Exceptions;
using System.Reflection;

namespace Jscription.Core.Commands
{
    //基类
    public abstract class CmdRoot
    {
        public abstract void Run();

        public void Initialize(Dictionary<string, object>? args, string commandName)
        {
            if (args == null) return;

            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                string targetKey = prop.Name;

                if (!args.ContainsKey(targetKey))
                {
                    targetKey = prop.Name.ToLower();
                }

                if (args.TryGetValue(targetKey, out var rawValue))
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(rawValue, prop.PropertyType);
                        prop.SetValue(this, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        throw new JscriptionInvalidArgumentsException(
                            "CmdRoot",
                            targetKey,
                            $"参数类型不匹配。无法将 {rawValue?.GetType().Name} 转换为 {prop.PropertyType.Name}。"
                        );
                    }
                }
            }
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
    }
}
