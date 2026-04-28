using Epic.OnlineServices;
using FungleAPI.Attributes;
using FungleAPI.GameOptions.Collections;
using FungleAPI.PluginLoading;
using FungleAPI.Translation;
using FungleAPI.Utilities;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Globalization.CultureInfo;

namespace FungleAPI.GameOptions
{
    /// <summary>
    /// Group with options (used in ModSettings)
    /// </summary>
    public abstract class SettingsGroup
    {
        public DefaultOptionCollection OptionCollection = new DefaultOptionCollection();
        public abstract StringNames GroupName { get; }
        public virtual void Initialize(ModPlugin modPlugin)
        {
            OptionCollection.Initialize(GetType(), modPlugin);
        }
    }
}
