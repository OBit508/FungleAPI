using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AsmResolver.DotNet.Collections;
using DiscordConnect;
using FungleAPI.Base.Events;
using FungleAPI.PluginLoading;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Event
{
    /// <summary>
    /// A class that helps the event system to work
    /// </summary>
    public static class EventManager
    {
        public static T CallEvent<T>(T fungleEvent) where T : FungleEvent
        {
            return fungleEvent;
        }
        public static void RegisterEvents(ModPlugin plugin)
        {
        }
    }
}
