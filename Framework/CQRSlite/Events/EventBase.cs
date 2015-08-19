using System;
using System.Xml.Serialization;

namespace CQRSlite.Events
{
    [Serializable]
    public abstract class EventBase : IEvent
    {
        private int eventTypeVersion = 1;
        [XmlIgnore]
        public string EventTypeFullName { get { return GetType().FullName; } }

        public int EventTypeVersion
        {
            get { return eventTypeVersion; }
            protected set { eventTypeVersion = value; }
        }

        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}