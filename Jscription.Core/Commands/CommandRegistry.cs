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
            { "file.write", typeof(CmdFile.Write) },
            { "file.delete", typeof(CmdFile.Delete) },
            { "file.read", typeof(CmdFile.Read) },
            { "file.exists", typeof(CmdFile.Exists) },
            { "variable.set", typeof(CmdVariable.Set) },
            { "variable.get", typeof(CmdVariable.Get) },
            { "control.sleep", typeof(CmdControl.Sleep) },
            { "control.if", typeof(CmdControl.If) },
            { "control.loop", typeof(CmdControl.Loop) }
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
