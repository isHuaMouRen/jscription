using System;
using System.Collections.Generic;
using System.Text;

namespace Jscription.Core.Commands
{
    public class CmdNet
    {
        public class GetHttp : CmdRoot
        {
            public required string url { get; set; }

            public override object? Run()
            {
                using var client = new HttpClient();
                var result = client.GetStringAsync(url).GetAwaiter().GetResult();

                return result;
            }
        }
    }
}
