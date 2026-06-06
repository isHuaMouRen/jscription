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

        public class Contains : CmdRoot
        {
            public required string origin { get; set; }
            public required string value { get; set; }

            public override object? Run()
            {
                return origin.Contains(value);
            }
        }

        public class Length : CmdRoot
        {
            public required string origin { get; set; }

            public override object? Run()
            {
                return origin.Length;
            }
        }

        public class SubString : CmdRoot
        {
            public required string origin { get; set; }
            public required int startIndex { get; set; }
            public required int length { get; set; }

            public override object? Run()
            {
                return origin.Substring(startIndex, length);
            }
        }
    }
}
