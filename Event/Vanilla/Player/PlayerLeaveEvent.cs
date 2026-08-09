using FungleAPI.Base.Events;
using InnerNet;

namespace FungleAPI.Event.Vanilla.Player
{
    public sealed class PlayerLeaveEvent : FungleEvent
    {
        public ClientData ClientData { get; }
        public PlayerLeaveEvent(ClientData clientData)
        {
            ClientData = clientData;
        }
    }
}
