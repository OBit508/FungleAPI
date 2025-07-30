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
        public static bool InvokeMurderPlayerEvent(this RoleBehaviour role, PlayerControl target, MurderResultFlags resultFlags, EventTime time = EventTime.After)
        {
            foreach (MethodInfo method in role.GetType().GetMethods())
            {
                MurderEvent murderPlayer = method.GetCustomAttribute<MurderEvent>();
                if (murderPlayer != null && murderPlayer.Time == time)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Count() == 2 && parameters[0].ParameterType == typeof(PlayerControl) && parameters[1].ParameterType == typeof(MurderResultFlags))
                    {
                        object result = role.InvokeMethod(method, new object[] { target, resultFlags });
                        if (time == EventTime.Before && method.ReturnType == typeof(bool) && result != null && !(bool)result)
                        {
                            return (bool)result;
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
                    if (meetingEvent != null && isEnd ? meetingEvent.Time == EventTime.After : meetingEvent.Time == EventTime.Before)
                    {
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
