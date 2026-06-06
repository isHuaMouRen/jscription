namespace Jscription.Core.Commands
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase)
        {
            { "console.write", typeof(CmdConsole.Write) },
            { "console.writeline", typeof(CmdConsole.WriteLine) },
            { "console.readline", typeof(CmdConsole.ReadLine) },
            { "console.setcolor", typeof(CmdConsole.SetColor) },
            { "console.getcolor", typeof(CmdConsole.GetColor) },

            { "control.sleep", typeof(CmdControl.Sleep) },
            { "control.if", typeof(CmdControl.If) },
            { "control.loop", typeof(CmdControl.Loop) },

            { "dialog.messagebox", typeof(CmdDialog.MessageBox) },

            { "dir.create", typeof(CmdDir.Create) },
            { "dir.delete", typeof(CmdDir.Delete) },
            { "dir.exists", typeof(CmdDir.Exists) },

            { "env.getvar", typeof(CmdEnv.GetVar) },

            { "file.write", typeof(CmdFile.Write) },
            { "file.delete", typeof(CmdFile.Delete) },
            { "file.read", typeof(CmdFile.Read) },
            { "file.exists", typeof(CmdFile.Exists) },
            { "file.copy", typeof(CmdFile.Copy) },
            { "file.move", typeof(CmdFile.Move) },

            { "math.abs", typeof(CmdMath.Abs) },
            { "math.max", typeof(CmdMath.Max) },
            { "math.min", typeof(CmdMath.Min) },
            { "math.sign", typeof(CmdMath.Sign) },
            { "math.clamp", typeof(CmdMath.Clamp) },
            { "math.ceiling", typeof(CmdMath.Ceiling) },
            { "math.floor", typeof(CmdMath.Floor) },
            { "math.round", typeof(CmdMath.Round) },
            { "math.truncate", typeof(CmdMath.Truncate) },

            { "net.gethttp", typeof(CmdNet.GetHttp) },

            { "path.combine", typeof(CmdPath.Combine) },
            { "path.getfilename", typeof(CmdPath.GetFileName) },
            { "path.getextension", typeof(CmdPath.GetExtension) },

            { "process.start", typeof(CmdProcess.Start) },

            { "string.replace", typeof(CmdString.Replace) },
            { "string.trim", typeof(CmdString.Trim) },
            { "string.contains", typeof(CmdString.Contains) },
            { "string.length", typeof(CmdString.Length) },
            { "string.substring", typeof(CmdString.SubString) },

            { "var.set", typeof(CmdVar.Set) },
            { "var.get", typeof(CmdVar.Get) },

        };

        public static CmdRoot? CreateCommand(string? cmdName, Dictionary<string, object>? args)
        {
            if (cmdName == null || !_registry.TryGetValue(cmdName, out var cmdType))
                return null;

            var instance = (CmdRoot)Activator.CreateInstance(cmdType)!;
            return instance;
        }
    }
}
