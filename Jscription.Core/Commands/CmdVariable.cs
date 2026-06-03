using Jscription.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdVariable
    {
        public class Set : CmdRoot
        {
            public required string varName { get; set; }
            public required object value { get; set; }

            public override object? Run()
            {
                if (_globalVariables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                _globalVariables[varName] = value;

                return null;
            }
        }

        public class Get : CmdRoot
        {
            public required string varName { get; set; }

            public override object? Run()
            {
                if (_globalVariables == null)
                    throw new Exception($"命令 [{CommandName}] 运行时丢失了上下文变量字典。");

                if (_globalVariables.TryGetValue(varName, out var value))
                {
                    return value;
                }

                throw new JscriptionVariableNotFoundException(varName);
            }
        }
    }
}
