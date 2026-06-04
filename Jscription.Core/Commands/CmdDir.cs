using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdDir
    {
        public class Create : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run()
            {
                Directory.CreateDirectory(path);
                return null;
            }
        }
    }
}
