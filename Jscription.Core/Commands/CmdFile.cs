using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdFile
    {
        public class Write : CmdRoot
        {
            public required string path { get; set; }
            public string? content { get; set; }

            public override object? Run() { File.WriteAllText(path, content); return null; }
        }

        public class Delete : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run() { File.Delete(path); return null; }
        }

        public class Read : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run()
            {
                return File.ReadAllText(path);
            }
        }
    }
}
