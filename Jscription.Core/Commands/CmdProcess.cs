using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdProcess
    {
        public class Start : CmdRoot
        {
            public required string path { get; set; }
            public string? args { get; set; }
            public bool? useShell { get; set; }
            public string? workingDir { get; set; }
            public bool? createNoWindow { get; set; }


            public override object? Run()
            {
                if (string.IsNullOrEmpty(path))
                    throw new Exception("必要参数 path 未定义");

                var processstartinfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = args,
                    UseShellExecute = useShell ?? false,
                    WorkingDirectory = workingDir ?? "",
                    CreateNoWindow=createNoWindow??false
                };

                Process.Start(processstartinfo);

                return null;
            }
        }
    }
}
