using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdEnv
    {
        public class GetVar : CmdRoot
        {
            public required string varName { get; set; }

            public override object? Run()
            {
                if (string.IsNullOrEmpty(varName))
                    throw new Exception("未填写必要参数 `varName`");

                return Environment.GetEnvironmentVariable(varName);
            }
        }
    }
}
