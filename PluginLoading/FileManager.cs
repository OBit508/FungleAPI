using BepInEx.Configuration;
using FungleAPI.GameOptions.Collections;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.PluginLoading
{
    public static class FileManager
    {
        private static string FungleAPI_Folder = Path.Combine(Application.persistentDataPath, "FungleAPI");
        private static Dictionary<string, ConfigFile> Files = new Dictionary<string, ConfigFile>();
        public static string GetAPI_Folder()
        {
            if (!Directory.Exists(FungleAPI_Folder))
            {
                Directory.CreateDirectory(FungleAPI_Folder);
            }
            return FungleAPI_Folder;
        }
        public static ConfigFile GetFile(ModPlugin modPlugin)
        {
            string identifier = string.IsNullOrWhiteSpace(modPlugin.LocalMod.GUID)
                ? modPlugin.ModAssembly?.GetName().Name ?? modPlugin.FunglePlugin?.ModName ?? "UnknownMod"
                : modPlugin.LocalMod.GUID;
            string path = Path.Combine(GetAPI_Folder(), $"{TurnSafe(identifier)}.cfg");
            ConfigFile configFile;
            if (Files.TryGetValue(path, out configFile))
            {
                return configFile;
            }
            else
            {
                configFile = new ConfigFile(path, false);
                Files[path] = configFile;
                return configFile;
            }
        }
        private static string TurnSafe(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return "UnknownMod";
            }
            foreach (char c in InvalidChars)
            {
                str = str.Replace(c.ToString(), string.Empty).Replace(".", "-");
            }
            return str;
        }
        private static char[] InvalidChars = new char[] { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' };
    }
}
