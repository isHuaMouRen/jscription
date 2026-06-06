using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdPath
    {
        public class Combine : CmdRoot
        {
            public string? path1 { get; set; }
            public string? path2 { get; set; }
            public string? path3 { get; set; }
            public string? path4 { get; set; }

            public override object? Run()
            {
                var list = new List<string>();

                if (!string.IsNullOrEmpty(path1))
                    list.Add(path1);
                if (!string.IsNullOrEmpty(path2))
                    list.Add(path2);
                if (!string.IsNullOrEmpty(path3))
                    list.Add(path3);
                if (!string.IsNullOrEmpty(path4))
                    list.Add(path4);

                return Path.Combine(list.ToArray());
            }
        }

        public class GetFileName : CmdRoot
        {
            public required string path { get; set; }

            public override object? Run()
            {
                if (string.IsNullOrEmpty(path))
                    throw new Exception("必要属性 `path` 未填写");

                return Path.GetFileName(path);
            }
        }
    }
}
