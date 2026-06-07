using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class ExecutionContext
    {
        public Dictionary<string, object> Variables { get; }

        public int CurrentLineNumber { get; set; }

        public ExecutionContext(Dictionary<string, object> initialVariables)
        {
            Variables = new Dictionary<string, object>(initialVariables, StringComparer.OrdinalIgnoreCase);
        }
    }
}
