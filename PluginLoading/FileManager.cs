using BepInEx.Configuration;
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
        private static string FungleAPI_Folder = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "FungleAPI");
        private static Dictionary<string, ConfigFile> ConfigFiles = new Dictionary<string, ConfigFile>();
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
            string path = Path.Combine(GetAPI_Folder(), $"{modPlugin.RealName} - {modPlugin.LocalMod.GUID}");
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
        public static string GetName(ConfigFileType configFileType)
        {
            switch (configFileType)
            {
                case ConfigFileType.RoleOptions: return "Options";
                case ConfigFileType.RoleCountAndChance: return "CountAndChance";
                case ConfigFileType.TeamOptions: return "Options";
                case ConfigFileType.TeamCountAndPriority: return "CountAndPriority";
                default: return "Options";
            }
        }
        public static ConfigFile GetConfigFile(ModPlugin modPlugin, ConfigFileType configFileType)
        {
            FolderType folderType = FolderType.Options;
            switch (configFileType)
            {
                case ConfigFileType.RoleOptions: folderType = FolderType.Roles; break;
                case ConfigFileType.RoleCountAndChance: folderType = FolderType.Roles; break;
                case ConfigFileType.TeamOptions: folderType = FolderType.Teams; break;
                case ConfigFileType.TeamCountAndPriority: folderType = FolderType.Teams; break;
            }
            string path = Path.Combine(GetFolder(modPlugin, folderType), $"{GetName(configFileType)}.cfg");
            if (ConfigFiles.TryGetValue(path, out ConfigFile configFile))
            {
                return configFile;
            }
            else
            {
                ConfigFile cf = new ConfigFile(path, true);
                ConfigFiles[path] = cf;
                return cf;
            }
        }
    }
}
