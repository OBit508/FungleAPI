using FungleAPI.MonoBehaviours;
using FungleAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FungleAPI.Role.RoleEvent
{
    public static class EventManager
    {
        public static bool InvokeMurderPlayerEvent(this RoleBehaviour role, PlayerControl target, MurderResultFlags resultFlags, bool after)
        {
            foreach (MethodInfo method in role.GetType().GetMethods())
            {
                MurderEvent murderPlayer = method.GetCustomAttribute<MurderEvent>();
                if (murderPlayer != null)
                {
                    if (murderPlayer.Time == EventTime.Before && after)
                    {
                        return true;
                    }
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(PlayerControl) && parameters[1].ParameterType == typeof(MurderResultFlags))
                    {
                        object result = role.InvokeMethod(method, new object[] { target, resultFlags });
                        if (!after && method.ReturnType == typeof(bool) && result != null && result is bool value)
                        {
                            return value;
                        }
                    }
                }
            }
            return true;
        }
        public static void InvokeMeetingEvent(this MeetingHud meetingHud, bool isEnd)
        {
            foreach (RoleBehaviour role in RoleManager.Instance.AllRoles)
            {
                foreach (MethodInfo method in role.GetType().GetMethods())
                {
                    MeetingEvent meetingEvent = method.GetCustomAttribute<MeetingEvent>();
                    if (meetingEvent != null)
                    {
                        if (isEnd && meetingEvent.Time == EventTime.Before)
                        {
                            return;
                        }
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Count() == 1 && parameters[0].ParameterType == typeof(MeetingHud))
                        {
                            role.InvokeMethod(method, new object[] { meetingHud });
                        }
                    }
                }
            }
        }
    }
}
