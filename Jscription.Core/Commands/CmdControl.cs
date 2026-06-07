using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
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
            public object? condition { get; set; }

            public List<CmdRoot>? then { get; set; }
            public List<CmdRoot>? @else { get; set; }

            public override object? Run()
            {
                if (_context?.Variables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                var currentCondition = GetDynamicArgument(nameof(condition));
                bool isTrue = EvaluateCondition(currentCondition);

                var targetCommands = isTrue ? then : @else;

                if (targetCommands != null)
                {
                    foreach (var subCmd in targetCommands)
                    {
                        subCmd.Execute();
                    }
                }

                return null;
            }
        }

        public class Loop : CmdRoot
        {
            public object? condition { get; set; }

            public List<CmdRoot>? @do { get; set; }

            public override object? Run()
            {
                if (_context?.Variables == null) throw new Exception($"命令 [{CommandName}] 运行时丢失上下文。");
                if (@do == null || @do.Count == 0) return null;

                while (true)
                {
                    var currentCondition = GetDynamicArgument(nameof(condition));

                    if (!EvaluateCondition(currentCondition))
                    {
                        break;
                    }

                    foreach (var subCmd in @do)
                    {
                        subCmd.Execute();
                    }
                }
                return null;
            }
        }
    }
}
