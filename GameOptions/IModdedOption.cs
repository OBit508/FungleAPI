using BepInEx.Configuration;
using FungleAPI.PluginLoading;
using Hazel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FungleAPI.GameOptions
{
    public interface IModdedOption
    {
        ConfigEntry<string> Entry { get; set; }
        BaseGameSetting Data { get; set; }
        uint OptionId { get; set; }
        string StringOptionId { get; set; }
        ModPlugin OwnerPlugin { get; set; }
        object DefaultValue { get; set; }
        void SetValue(object value, bool amHost);
        string GetStringValue(bool amHost);
        void Serialize(MessageWriter messageWriter);
        void Deserialize(MessageReader messageReader);
        void SaveValue(ConfigEntry<string> configEntry);
        void LoadValue(ConfigEntry<string> configEntry);
        void SetOnValueChance(Action action);
        OptionBehaviour CreateOption(Transform parent);
    }
}
