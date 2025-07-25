using BepInEx.Configuration;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;

namespace FungleAPI.Role.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomConfig : Attribute
    {
        public StringNames ConfigName;
        internal string onlineValue;
        internal ConfigEntry<string> localValue;
        public string GetValue()
        {
            if (AmongUsClient.Instance.AmHost)
            {
                return localValue.Value;
            }
            else
            {
                return onlineValue;
            }
        }
        public void SetValue(object value)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                localValue.Value = value.ToString();
            }
            else
            {
                onlineValue = value.ToString();
            }
        }
        public virtual void Initialize(Type type, PropertyInfo property, object obj)
        {
        }
    }
}
