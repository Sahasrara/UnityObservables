using System;

namespace Studio.ShortSleeve.UnityObservables
{
    // We're jumping through some hoops here to limit allocations to the list/dict combination in EventBus.
    public readonly struct EventHandlerWrapper<TPayload> : IEquatable<EventHandlerWrapper<TPayload>>
    {
        private readonly EventHandler _handlerDelegateVoid;
        private readonly EventHandler<TPayload> _handlerDelegateGeneric;
        private readonly IEventHandler _handlerInterfaceVoid;
        private readonly IEventHandler<TPayload> _handlerInterfaceGeneric;
        private readonly EventHandlerType _type;

        internal EventHandlerWrapper(EventHandler handler)
        {
            _type = EventHandlerType.DelegateVoid;
            _handlerDelegateVoid = handler;
            _handlerDelegateGeneric = null;
            _handlerInterfaceVoid = null;
            _handlerInterfaceGeneric = null;
        }

        internal EventHandlerWrapper(EventHandler<TPayload> handler)
        {
            _type = EventHandlerType.DelegateGeneric;
            _handlerDelegateVoid = null;
            _handlerDelegateGeneric = handler;
            _handlerInterfaceVoid = null;
            _handlerInterfaceGeneric = null;
        }

        internal EventHandlerWrapper(IEventHandler handler)
        {
            _type = EventHandlerType.InterfaceVoid;
            _handlerDelegateVoid = null;
            _handlerDelegateGeneric = null;
            _handlerInterfaceVoid = handler;
            _handlerInterfaceGeneric = null;
        }

        internal EventHandlerWrapper(IEventHandler<TPayload> handler)
        {
            _type = EventHandlerType.InterfaceGeneric;
            _handlerDelegateVoid = null;
            _handlerDelegateGeneric = null;
            _handlerInterfaceVoid = null;
            _handlerInterfaceGeneric = handler;
        }

        public object ID // custom editor, don't change name
        {
            get
            {
                switch (_type)
                {
                    case EventHandlerType.DelegateVoid:
                        return _handlerDelegateVoid;
                    case EventHandlerType.DelegateGeneric:
                        return _handlerDelegateGeneric;
                    case EventHandlerType.InterfaceVoid:
                        return _handlerInterfaceVoid;
                    default:
                        return _handlerInterfaceGeneric;
                }
            }
        }

        private enum EventHandlerType
        {
            DelegateVoid,
            DelegateGeneric,
            InterfaceVoid,
            InterfaceGeneric,
        }

        public bool Equals(EventHandlerWrapper<TPayload> other) => Equals(ID, other.ID);

        public override bool Equals(object obj) =>
            obj is EventHandlerWrapper<TPayload> other && Equals(other);

        public override int GetHashCode() => ID.GetHashCode();

        public void HandleEvent(TPayload payload)
        {
            switch (_type)
            {
                case EventHandlerType.DelegateVoid:
                    _handlerDelegateVoid.Invoke();
                    break;
                case EventHandlerType.DelegateGeneric:
                    _handlerDelegateGeneric.Invoke(payload);
                    break;
                case EventHandlerType.InterfaceVoid:
                    _handlerInterfaceVoid.HandleEvent();
                    break;
                default:
                    _handlerInterfaceGeneric.HandleEvent(payload);
                    break;
            }
        }
    }
}
