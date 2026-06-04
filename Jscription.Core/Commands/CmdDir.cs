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

        public class Delete : CmdRoot
        {
            public required string path { get; set; }
            public bool? recursive { get; set; }

            public override object? Run()
            {
                if (recursive != null)
                    Directory.Delete(path, (bool)recursive);
                else
                    Directory.Delete(path);

                return null;
            }
        }
    }
}
