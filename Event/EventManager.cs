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
        private static Dictionary<Type, Delegate> Events = new Dictionary<Type, Delegate>();
        public static T CallEvent<T>(T fungleEvent) where T : FungleEvent
        {
            if (Events.TryGetValue(typeof(T), out Delegate @delegate))
            {
                ((Action<T>)@delegate)?.Invoke(fungleEvent);
            }
            return fungleEvent;
        }
        public static void RegisterEvents(ModPlugin modPlugin)
        {
            foreach (Type type in modPlugin.AllTypes)
            {
                foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
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
