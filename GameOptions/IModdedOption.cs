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
        BaseGameSetting Data { get; set; }
        string OptionId { get; set; }
        object DefaultValue { get; set; }
        void SetValue(object value, bool amHost);
        string GetStringValue(bool amHost);
        void Serialize(MessageWriter messageWriter);
        void Deserialize(MessageReader messageReader);
        void WriteLocalValue(BinaryWriter binaryWriter);
        void ReadLocalValue(BinaryReader binaryReader);
        void SetOnValueChance(Action<bool> action);
        void SyncNonHostWithLocal();
        OptionBehaviour CreateOption(Transform parent);
    }
}
