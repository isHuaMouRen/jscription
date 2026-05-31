namespace Jscription.Core.Commands
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase)
        {
            { "console.print", typeof(CmdConsole.Print) },
            { "console.printline", typeof(CmdConsole.PrintLine) },
            { "file.write", typeof(CmdFile.Write) },
            { "file.delete", typeof(CmdFile.Delete) },
            { "file.read", typeof(CmdFile.Read) },
            { "variable.set", typeof(CmdVariable.Set) },
            { "control.sleep", typeof(CmdControl.Sleep) },
            { "control.if", typeof(CmdControl.If) }
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
