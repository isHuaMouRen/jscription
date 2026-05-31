using Jscription.Core.Commands;
using Newtonsoft.Json;

namespace Jscription.Core.Classes
{
    public class JscriptionDoc
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("commands")]
        public List<CommandInfo>? Commands { get; set; }

        public class CommandInfo
        {
            [JsonProperty("command")]
            public string? Command { get; set; }

            [JsonProperty("arguments")]
            public Dictionary<string, object>? Arguments { get; set; }
        }
    }

    public class JscriptionExecutInfo
    {
        public required string Name { get; set; }

        public required List<CmdRoot> Commands { get; set; }
    }
}
