using System;
using System.Runtime.Serialization;
using CQRSlite.Events;
using CQRSlite.Messages;

namespace CQRSlite.Domain.Exception
{
    [Serializable]
    public class EventHandlerFailedException : System.Exception
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public DateTimeOffset TimeStamp { get; private set; }
        public string EventTypeFullName { get; private set; }
        public int EventTypeVersion { get; private set; }

        public EventHandlerFailedException(IEvent @event,System.Exception innerException) :
            base("CQRS Eventhandler failed; See properties and inner exception for more information.", innerException)
        {
            Id = @event.Id;
            Version = @event.Version;
            TimeStamp = @event.TimeStamp;
            EventTypeFullName = @event.EventTypeFullName;
            EventTypeVersion = @event.EventTypeVersion;
        }

        protected EventHandlerFailedException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                Id = Guid.Parse(info.GetString("Id"));
                Version = (int) info.GetValue("Version", typeof (int));
                TimeStamp = (DateTimeOffset) info.GetValue("TimeStamp", typeof (DateTimeOffset));
                EventTypeFullName = info.GetString("EventTypeFullName");
                EventTypeVersion = (int) info.GetValue("EventTypeVersion", typeof (int));
            }
        }

        public override void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("Id", Id);
                info.AddValue("Version", Version);
                info.AddValue("TimeStamp", TimeStamp);
                info.AddValue("EventTypeFullName", EventTypeFullName);
                info.AddValue("EventTypeVersion", EventTypeVersion);
            }
        }
    }
}