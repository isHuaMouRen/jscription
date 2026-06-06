using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdString
    {
        public class Replace : CmdRoot
        {
            public required string origin { get; set; }
            public required string oldChar { get; set; }
            public required string newChar { get; set; }

            public override object? Run()
            {
                return origin.Replace(oldChar, newChar);
            }
        }

        public class Trim : CmdRoot
        {
            public required string origin { get; set; }

            public override object? Run()
            {
                return origin.Trim();
            }
        }
    }
}
