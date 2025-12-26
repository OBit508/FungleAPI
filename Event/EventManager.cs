using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AsmResolver.DotNet.Collections;
using DiscordConnect;
using FungleAPI.Event.Types;
using FungleAPI.PluginLoading;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;

namespace FungleAPI.Event
{
    public static class EventManager
    {
        public static List<Type> EventTypes = new List<Type>();
        public static Dictionary<Type, List<MethodInfo>> AllEvents = new Dictionary<Type, List<MethodInfo>>();
        public static void CallEvent<T>(T value) where T : FungleEvent
        {
            Type eventType = typeof(T);
            if (AllEvents.ContainsKey(eventType))
            {
                foreach (MethodInfo methodInfo in AllEvents[eventType])
                {
                    try
                    {
                        methodInfo.Invoke(null, new object[] { value });
                    }
                    catch (Exception ex)
                    {
                        FungleAPIPlugin.Instance.Log.LogError(ex);
                    }
                }
            }
        }
        public static void RegisterEvents(ModPlugin plugin)
        {
            foreach (Type type in plugin.AllTypes)
            {
                foreach (MethodInfo methodInfo in type.GetMethods())
                {
                    if (methodInfo.GetCustomAttribute<EventRegister>() != null)
                    {
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (parameters.Length == 1)
                        {
                            Type parameterType = parameters[0].ParameterType;
                            if (EventTypes.Contains(parameterType))
                            {
                                if (methodInfo.IsStatic)
                                {
                                    if (AllEvents.ContainsKey(parameterType))
                                    {
                                        AllEvents[parameterType].Add(methodInfo);
                                    }
                                    else
                                    {
                                        AllEvents.Add(parameterType, new List<MethodInfo>() { methodInfo });
                                    }
                                    plugin.BasePlugin.Log.LogInfo(type.Name + "." + methodInfo.Name + " added to EventManager, Event: " + parameterType.Name);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
