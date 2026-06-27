using FungleAPI.PluginLoading;
using Il2CppSystem.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xCloud;

namespace FungleAPI.Chat
{
    public static class ChatCommandManager
    {
        public static Dictionary<string, BaseChatCommand> AllCommands = new Dictionary<string, BaseChatCommand>();

        internal static void RegisterCommand(Type type, ModPlugin modPlugin)
        {
            BaseChatCommand baseChatCommand = (BaseChatCommand)Activator.CreateInstance(type);

            if (AllCommands.ContainsKey(baseChatCommand.CommandName))
            {
                modPlugin.BasePlugin.Log.LogError($"Some command has the same name from {type.Name}");
                return;
            }

            AllCommands.Add(baseChatCommand.CommandName, baseChatCommand);

            modPlugin.BasePlugin.Log.LogInfo($"Registered Chat Command {type.Name}");
        }
        public static string TryFindSomeCommand(string entry, ref BaseChatCommand baseChatCommand)
        {
            if (!entry.StartsWith("/")) return string.Empty;

            entry = entry.Substring(1);

            if (entry.Length <= 0) return string.Empty;

            string[] parts = entry.Split(" ");
            string commandPart = parts[0];

            if (commandPart.Length <= 0) return string.Empty;

            int args = parts.Skip(1).Count(p => p.Length > 0);

            string anyCommand = AllCommands.Keys.FirstOrDefault(c => c.StartsWith(commandPart));

            if (anyCommand == null || anyCommand.Length <= 0) return string.Empty;

            if (AllCommands.TryGetValue(anyCommand, out baseChatCommand))
            {
                string command = $"/{baseChatCommand.CommandName}";

                for (int i = 0; i < baseChatCommand.Arguments.Length; i++)
                {
                    if (args > i) continue;

                    command += " {" + baseChatCommand.Arguments[i] + "}";
                }

                return command;
            }

            return string.Empty;
        }
    }
}
