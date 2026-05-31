using Jscription.Core.Commands;

namespace Jscription.Core.Utils
{
    public static class CommandRegistry
    {
        private static readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase)
        {
            { "print", typeof(CmdPrint) }
        };

        public static CmdRoot? CreateCommand(string? cmdName, Dictionary<string, object>? args)
        {
            if (cmdName == null || !_registry.TryGetValue(cmdName, out var cmdType))
                return null;

            var instance = (CmdRoot)Activator.CreateInstance(cmdType)!;
            instance.Initialize(args);
            return instance;
        }
    }
}
