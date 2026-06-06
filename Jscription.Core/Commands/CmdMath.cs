using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdMath
    {
        public class Abs : CmdRoot
        {
            public required double value { get; set; }

            public override object? Run() => Math.Abs(value);
        }

        public class Max : CmdRoot
        {
            public required double value1 { get; set; }
            public required double value2 { get; set; }

            public override object? Run() => Math.Max(value1, value2);
        }

        public class Min : CmdRoot
        {
            public required double value1 { get; set; }
            public required double value2 { get; set; }

            public override object? Run() => Math.Min(value1, value2);
        }

        public class Sign : CmdRoot
        {
            public required double value { get; set; }

            public override object? Run() => Math.Sign(value);
        }

        public class Clamp : CmdRoot
        {
            public required double value { get; set; }
            public required double max { get; set; }
            public required double min { get; set; }

            public override object? Run() => Math.Clamp(value, min, max);
        }

        public class Ceiling : CmdRoot
        {
            public required double value { get; set; }

            public override object? Run() => Math.Ceiling(value);
        }

        public class Floor : CmdRoot
        {
            public required double value { get; set; }

            public override object? Run() => Math.Floor(value);
        }

        public class Round : CmdRoot
        {
            public required double value { get; set; }

            public override object? Run() => Math.Round(value, MidpointRounding.AwayFromZero);
        }

        public class Truncate : CmdRoot
        {
            public required double value { get; set; }

            public override object? Run() => Math.Truncate(value);
        }
    }
}
