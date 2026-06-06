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

        public class Exists : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run()
            {
                return File.Exists(path);
            }
        }

        public class Copy : CmdRoot
        {
            public required string source { get; set; }
            public required string dest { get; set; }
            public bool? overwrite{ get; set; }

            public override object? Run()
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest))
                    throw new Exception("必要参数 `source` 或 `dest` 未填写！");

                File.Copy(source, dest, overwrite ?? false);
                return null;
            }
        }

        public class Move : CmdRoot
        {
            public required string source { get; set; }
            public required string dest { get; set; }
            public bool? overwrite{ get; set; }

            public override object? Run()
            {
                if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest))
                    throw new Exception("必要参数 `source` 或 `dest` 未填写！");

                File.Move(source, dest, overwrite ?? false);
                return null;
            }
        }
    }
}
