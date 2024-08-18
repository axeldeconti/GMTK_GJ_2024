using System.Collections.Generic;

namespace Avenyrh
{
    public delegate void EventHandler(params object[] args);

    public class EventManager
    {
        private static EventManager _instance;
        private Dictionary<string, EventHandler> _events;

        private EventManager()
        {
            _events = new Dictionary<string, EventHandler>();
        }

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                }

                return _instance;
            }
        }

        public static void Subscribe(Ev eventId, EventHandler handler)
        {
            Subscribe(eventId.ToString(), handler);
        }

        public static void Subscribe(string eventId, EventHandler handler)
        {
            if (!Instance._events.ContainsKey(eventId))
            {
                Instance._events[eventId] = null;
            }

            Instance._events[eventId] += handler;
        }

        internal void UnsubscribeAll()
        {
            _events.Clear();
        }

        public static void Unsubscribe(Ev eventId, EventHandler handler)
        {
            Unsubscribe(eventId.ToString(), handler);
        }

        public static void Unsubscribe(string eventId, EventHandler handler)
        {
            if (Instance._events.ContainsKey(eventId))
            {
                Instance._events[eventId] -= handler;
            }
        }

        public static void Trigger(Ev eventId, params object[] args)
        {
            Trigger(eventId.ToString(), args);
        }

        public static void Trigger(string eventId, params object[] args)
        {
            if (Instance._events.ContainsKey(eventId))
            {
                EventHandler handlers = Instance._events[eventId];
                if (handlers != null)
                {
                    handlers(args);
                }
            }
        }
    }

    public class Mut<T>
    {
        public T Value { get; internal set; }

        public static implicit operator T(Mut<T> m) => m.Value;
    }
}