using AsmResolver.DotNet.Collections;
using DiscordConnect;
using FungleAPI.Api;
using FungleAPI.Base.Events;
using FungleAPI.PluginLoading;
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Event
{
    /// <summary>
    /// Manage the events
    /// </summary>
    public static class EventManager
    {
        private static Dictionary<Type, Delegate> Events = new Dictionary<Type, Delegate>();
        public static T CallEvent<T>(T fungleEvent) where T : FungleEvent
        {
            if (Events.TryGetValue(typeof(T), out Delegate @delegate))
            {
                foreach (Action<T> handler in @delegate.GetInvocationList())
                {
                    try
                    {
                        handler(fungleEvent);
                    }
                    catch (Exception ex)
                    {
                        FunglePlugin<FungleApiPlugin>.Logger.LogError($"Failed to execute event '{typeof(T).Name}'.\n{ex}");
                    }
                }
            }
            return fungleEvent;
        }
        public static void RegisterEvents(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (type.ShouldIgnore()) continue;

                foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (methodInfo.ShouldIgnore()) continue;

                    if (methodInfo.GetCustomAttribute<EventRegister>() == null || methodInfo.IsSpecialName || methodInfo.ReturnType != typeof(void)) continue;

                    ParameterInfo[] parameters = methodInfo.GetParameters();

                    if (parameters.Length != 1 || !typeof(FungleEvent).IsAssignableFrom(parameters[0].ParameterType)) continue;

                    Type eventType = parameters[0].ParameterType;
                    Delegate handler = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(eventType), methodInfo);

                    if (Events.TryGetValue(eventType, out Delegate @delegate))
                    {
                        Events[eventType] = Delegate.Combine(@delegate, handler);
                    }
                    else
                    {
                        Events[eventType] = handler;
                    }
                }
            }
        }
    }
    public class EventRegister : Attribute { }
}
