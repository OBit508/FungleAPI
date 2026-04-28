using BepInEx.Configuration;
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
        private static string FungleAPI_Folder = Path.Combine(Application.dataPath, "FungleAPI");
        public static string GetAPI_Folder()
        {
            if (!Directory.Exists(FungleAPI_Folder))
            {
                Directory.CreateDirectory(FungleAPI_Folder);
            }
            return FungleAPI_Folder;
        }
        public static string GetPlugin_Folder(ModPlugin modPlugin)
        {
            string path = Path.Combine(GetAPI_Folder(), TurnSafe($"{modPlugin.RealName} - {modPlugin.LocalMod.GUID}"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public static string GetFolder(ModPlugin modPlugin, FolderType folderType)
        {
            string path = Path.Combine(GetPlugin_Folder(modPlugin), folderType.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        private static string TurnSafe(string str)
        {
            foreach (char c in InvalidChars)
            {
                str = str.Replace(c.ToString(), string.Empty);
            }
            return str;
        }
        private static char[] InvalidChars = new char[] { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' };
    }
}
