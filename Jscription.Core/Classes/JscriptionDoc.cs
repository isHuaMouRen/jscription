using Jscription.Core.Commands;
using Newtonsoft.Json;

namespace Jscription.Core.Classes
{
    public class JscriptionDoc
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("variables")]
        public Dictionary<string, object>? Variables { get; set; }

        [JsonProperty("commands")]
        public List<CommandInfo>? Commands { get; set; }

        public class CommandInfo
        {
            [JsonProperty("command")]
            public string? Command { get; set; }

            [JsonProperty("arguments")]
            public Dictionary<string, object>? Arguments { get; set; }

            [JsonProperty("return")]
            public string? Return { get; set; }

            
            //记录行号
            [JsonIgnore]
            public int LineNumber { get; set; }
        }
    }

    public class JscriptionExecutInfo
    {
        public required string Name { get; set; }

        public required List<CmdRoot> Commands { get; set; }
    }
}
