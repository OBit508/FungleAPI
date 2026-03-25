using AmongUs.InnerNet.GameDataMessages;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using FungleAPI.Configuration.Networking;
using FungleAPI.Networking;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using Rewired.Internal.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FungleAPI.ModCompatibility
{
    /// <summary>
    /// FungleAPI doesn't use Reactor, so this class was created to allow Reactor and its mods to work together with FungleAPI
    /// </summary>
    public static class ReactorSupport
    {
        public static string ReactorCreditsText => ReactorCredits_GetText == null ? null : (string)ReactorCredits_GetText.Invoke(null, new object[] { ReactorCredits_Location_PingTracker });
        public static bool DisableServerAuthority => DisableServerAuthorityPatch_Enabled == null ? false : (bool)DisableServerAuthorityPatch_Enabled.GetValue(null);
        public static PropertyInfo DisableServerAuthorityPatch_Enabled;
        public static MethodInfo ReactorCredits_GetText;
        public static MethodInfo Reactor_LocalizationManager_TryGetTextFormatted;
        public static object ReactorCredits_Location_PingTracker;
        public static Assembly ReactorAssembly;
        public static BasePlugin ReactorPlugin;
        public static Harmony ReactorHarmony;
        public static void Initialize()
        {
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("gg.reactor.api", out PluginInfo pluginInfo))
            {
                FungleAPIPlugin.Instance.Log.LogInfo("Initializing Reactor Support");
                ReactorAssembly = pluginInfo.Instance.GetType().Assembly;
                ReactorPlugin = (BasePlugin)pluginInfo.Instance;
                ReactorHarmony = (Harmony)pluginInfo.Instance.GetType().GetProperty("Harmony").GetValue(ReactorPlugin);
                foreach (Type type in ReactorAssembly.GetTypes())
                {
                    if (type.FullName == "Reactor.Utilities.ReactorCredits")
                    {
                        ReactorCredits_GetText = type.GetMethod("GetText", AccessTools.all);
                        ReactorCredits_Location_PingTracker = Enum.Parse(type.GetNestedType("Location"), "PingTracker");
                        type.GetMethod("Register", new Type[] { typeof(string), typeof(string), typeof(bool), type.GetField("AlwaysShow").FieldType }).Invoke(null, new object[] { "FungleAPI", FungleAPIPlugin.ModV, false, null });
                    }
                    else if (type.FullName == "Reactor.Patches.Miscellaneous.DisableServerAuthorityPatch")
                    {
                        DisableServerAuthorityPatch_Enabled = type.GetProperty("Enabled");
                        ReactorHarmony.Unpatch(typeof(Constants).GetMethod("GetBroadcastVersion"), type.GetMethod("GetBroadcastVersionPatch"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Patches.Miscellaneous.DisableServerAuthorityPatch.GetBroadcastVersionPatch");
                        ReactorHarmony.Unpatch(typeof(Constants).GetMethod("IsVersionModded"), type.GetMethod("IsVersionModdedPatch"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Patches.Miscellaneous.DisableServerAuthorityPatch.IsVersionModdedPatch");
                    }
                    else if (type.FullName == "Reactor.Patches.Miscellaneous.PingTrackerPatch")
                    {
                        ReactorHarmony.Unpatch(typeof(PingTracker).GetMethod("Update"), type.GetMethod("Postfix"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Patches.Miscellaneous.PingTrackerPatch.Postfix");
                    }
                    else if (type.FullName == "Reactor.Localization.Utilities.CustomStringName")
                    {
                        FungleAPIPlugin.Harmony.Patch(type.GetMethod("Create"), new HarmonyMethod(typeof(ReactorSupport).GetMethod("CustomStringName_Create_Prefix")));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Localization.Utilities.CustomStringName.Create");
                    }
                    else if (type.FullName == "Reactor.Localization.LocalizationManager")
                    {
                        Reactor_LocalizationManager_TryGetTextFormatted = type.GetMethod("TryGetTextFormatted", AccessTools.all);
                    }
                    else if (type.FullName == "Reactor.Localization.Patches.GetStringPatch")
                    {
                        ReactorHarmony.Unpatch(typeof(TranslationController).GetMethod("GetString", new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) }), type.GetMethod("StringNamesPatch"));
                        ReactorHarmony.Unpatch(typeof(TranslationController).GetMethod("GetStringWithDefault", new Type[] { typeof(StringNames), typeof(string), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) }), type.GetMethod("StringNamesPatch"));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Localization.Patches.GetStringPatch.StringNamesPatch");
                    }
                    else if (type.FullName == "Reactor.Networking.Patches.ReactorClientData")
                    {
                        FungleAPIPlugin.Harmony.Patch(type.GetMethod("Set", AccessTools.all), null, new HarmonyMethod(typeof(ReactorSupport).GetMethod("ReactorClientData_Set_Postfix")));
                        FungleAPIPlugin.Instance.Log.LogWarning("Patched Reactor.Networking.Patches.ReactorClientData");
                    }
                }   
            }
        }
        public static void ReactorClientData_Set_Postfix(int clientId)
        {
            Rpc<RpcSyncAllConfigs>.Instance.Send(PlayerControl.LocalPlayer, SendOption.Reliable, clientId);
        }
        public static bool CustomStringName_Create_Prefix(ref StringNames __result)
        {
            __result = (StringNames)TranslationManager.validId;
            TranslationManager.validId++;
            return false;
        }
        public static bool LocalizationManager_TryGetTextFormatted(StringNames stringName, Il2CppReferenceArray<Il2CppSystem.Object> parts, out string text)
        {
            if (Reactor_LocalizationManager_TryGetTextFormatted != null)
            {
                object[] args = new object[] { stringName, parts, null };
                bool result = (bool)Reactor_LocalizationManager_TryGetTextFormatted.Invoke(null, args);
                text = (string)args[2];
                return result;
            }
            text = "";
            return false;
        }
    }
}
