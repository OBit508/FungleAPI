using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FungleAPI.Chat.Patches
{
    [HarmonyPatch(typeof(ChatController))]
    internal static class ChatControllerPatch
    {
        public static string lastText;
        public static TextMeshPro command;
        [HarmonyPatch(nameof(ChatController.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(ChatController __instance)
        {
            command = GameObject.Instantiate<TextMeshPro>(__instance.freeChatField.textArea.outputText, __instance.freeChatField.textArea.outputText.transform.parent);
            command.color = new Color(0.7f, 0.7f, 0.7f, 0.7f);
            lastText = string.Empty;
        }
        [HarmonyPatch(nameof(ChatController.Update))]
        [HarmonyPostfix]
        public static void UpdatePostfix(ChatController __instance)
        {
            if (lastText != __instance.freeChatField.textArea.text)
            {
                command.transform.localPosition = __instance.freeChatField.textArea.outputText.transform.localPosition;

                lastText = __instance.freeChatField.textArea.text;

                BaseChatCommand baseChatCommand = null;
                command.text = ChatCommandManager.TryFindSomeCommand(lastText, ref baseChatCommand);
            }
        }
        [HarmonyPatch(nameof(ChatController.SendFreeChat))]
        [HarmonyPrefix]
        public static bool SendFreeChatPrefix(ChatController __instance)
        {
            string text = __instance.freeChatField.Text;

            BaseChatCommand baseChatCommand = null;
            ChatCommandManager.TryFindSomeCommand(text, ref baseChatCommand);

            if (baseChatCommand != null)
            {
                bool cancelSend = false;
                baseChatCommand.ExecuteCommand(text.Split(" ").Skip(1), ref cancelSend);

                if (cancelSend)
                {
                    return false;
                }
            }

            int num;
            int num2;
            if (UrlFinder.TryFindUrl(text.ToCharArray(), out num, out num2))
            {
                ChatController.Logger.Warning(string.Format("{0}() :: ABORTED, URL was found. Showing {1} instead!", "SendFreeChat", StringNames.FreeChatLinkWarning), null);
                __instance.AddChatWarning(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FreeChatLinkWarning));
                return false;
            }
            ChatController.Logger.Debug("SendFreeChat () :: Sending message: '" + text + "'", null);

            PlayerControl.LocalPlayer.RpcSendChat(text);

            return false;
        }
    }
}
