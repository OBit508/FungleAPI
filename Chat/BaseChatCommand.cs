using FungleAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.Chat
{
    [FungleIgnore]
    public class BaseChatCommand
    {
        public virtual string CommandName { get; }
        public virtual string[] Arguments { get; } = new string[0];
        public virtual void ExecuteCommand(IEnumerable<string> args, ref bool cancelSend) { }
    }
}
