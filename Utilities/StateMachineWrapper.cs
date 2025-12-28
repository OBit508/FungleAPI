using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using System.Reflection;

namespace FungleAPI.Utilities
{
    [Comment("MiraAPI class")]
    public class StateMachineWrapper<T> where T : Il2CppObjectBase
    {
        public T Instance
        {
            get
            {
                T result;
                if ((result = this._parentInstance) == null)
                {
                    result = (this._parentInstance = (T)((object)this._thisProperty.GetValue(this._stateMachine)));
                }
                return result;
            }
        }
        public StateMachineWrapper(Il2CppObjectBase stateMachine)
        {
            this._stateMachine = stateMachine;
            Type type = this._stateMachine.GetType();
            this._thisProperty = AccessTools.Property(type, "__4__this");
            this._stateProperty = AccessTools.Property(type, "__1__state");
            if (this._thisProperty == null || this._stateProperty == null)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
                defaultInterpolatedStringHandler.AppendLiteral("Could not find required properties in type '");
                defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
                defaultInterpolatedStringHandler.AppendLiteral("'.");
                throw new MissingMemberException(defaultInterpolatedStringHandler.ToStringAndClear());
            }
            this._propertyCache = new Dictionary<string, PropertyInfo>();
        }
        public int GetState()
        {
            return (int)this._stateProperty.GetValue(this._stateMachine);
        }
        public TField GetParameter<TField>(string parameterName)
        {
            PropertyInfo property;
            if (this._propertyCache.TryGetValue(parameterName, out property))
            {
                return (TField)((object)property.GetValue(this._stateMachine));
            }
            PropertyInfo propertyInfo = AccessTools.Property(this._stateMachine.GetType(), parameterName);
            if (propertyInfo == null)
            {
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 2);
                defaultInterpolatedStringHandler.AppendLiteral("Could not find parameter '");
                defaultInterpolatedStringHandler.AppendFormatted(parameterName);
                defaultInterpolatedStringHandler.AppendLiteral("' in state machine of type '");
                defaultInterpolatedStringHandler.AppendFormatted<Type>(this._stateMachine.GetType());
                defaultInterpolatedStringHandler.AppendLiteral("'.");
                throw new MissingFieldException(defaultInterpolatedStringHandler.ToStringAndClear());
            }
            property = propertyInfo;
            this._propertyCache[parameterName] = property;
            return (TField)((object)property.GetValue(this._stateMachine));
        }
        private readonly Il2CppObjectBase _stateMachine;
        private readonly PropertyInfo _thisProperty;
        private readonly PropertyInfo _stateProperty;
        private readonly Dictionary<string, PropertyInfo> _propertyCache;
        private T _parentInstance;
    }
}
