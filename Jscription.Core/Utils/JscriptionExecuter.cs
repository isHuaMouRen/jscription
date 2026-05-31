using Jscription.Core.Exceptions;
using Jscription.Core.Classes;

namespace Jscription.Core.Utils
{
    public class JscriptionExecuter
    {
        public required JscriptionExecutInfo ExecuteInfo { get; set; }

        public void Run()
        {
            foreach (var cmd in ExecuteInfo.Commands)
            {
                if (cmd == null)
                    continue;

                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    throw new JscriptionRuntimeException(cmd, ex.Message, ex);
                }
            }
        }
    }
}