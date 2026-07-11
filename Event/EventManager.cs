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
using FungleAPI.Utilities;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FungleAPI.Event
{
    /// <summary>
    /// Manage the events
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<Type, List<RegisteredEvent>> Events = new Dictionary<Type, List<RegisteredEvent>>();
        private static long _registrationOrder;
        public static T CallEvent<T>(T fungleEvent) where T : FungleEvent
        {
            if (Events.TryGetValue(typeof(T), out List<RegisteredEvent> handlers))
            {
                foreach (RegisteredEvent handler in handlers)
                {
                    handler.Handler.DynamicInvoke(fungleEvent);
                }
            }
            return fungleEvent;
        }
        public static void RegisterEvents(ModPlugin modPlugin)
        {
            foreach (Type type in modPlugin.AllTypes)
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

                    if (!Events.TryGetValue(eventType, out List<RegisteredEvent> handlers))
                    {
                        handlers = new List<RegisteredEvent>();
                        Events[eventType] = handlers;
                    }

                    handlers.Add(new RegisteredEvent(handler, methodInfo.GetCustomAttribute<EventRegister>().Priority, _registrationOrder++));
                    handlers.Sort((left, right) =>
                    {
                        int priority = right.Priority.CompareTo(left.Priority);
                        return priority != 0 ? priority : left.Order.CompareTo(right.Order);
                    });
                }
            }
        }

        private sealed class RegisteredEvent
        {
            public RegisteredEvent(Delegate handler, int priority, long order)
            {
                Handler = handler;
                Priority = priority;
                Order = order;
            }

            public Delegate Handler { get; }
            public int Priority { get; }
            public long Order { get; }
        }
    }
    public class EventRegister : Attribute
    {
        public EventRegister(int priority = 0)
        {
            Priority = priority;
        }

        public int Priority { get; }
    }
}
