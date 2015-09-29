using System;
using System.Runtime.Serialization;
using CQRSlite.Commands;
using CQRSlite.Messages;

namespace CQRSlite.Domain.Exception
{
    [Serializable]
    public class CommandHandlerFailedException : System.Exception
    {

        public int ExpectedVersion { get; private set; }
        public string CommandTypeFullName { get; private set; }
        public string MethodName { get; private set; }

        public CommandHandlerFailedException(ICommand command, Action<IMessage> handler, System.Exception innerException) :
            base("CQRS command handler failed; See properties and inner exception for more information.", innerException)
        {
            ExpectedVersion = command.ExpectedVersion;
            CommandTypeFullName = command.GetType().FullName;
            MethodName = handler.Method.Name;
        }

        protected CommandHandlerFailedException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                ExpectedVersion = (int)info.GetValue("ExpectedVersion", typeof(int));
                CommandTypeFullName = info.GetString("CommandTypeFullName");
                MethodName = info.GetString("MethodName");
            }
        }

        public override void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);

            if (info != null)
            {
                info.AddValue("ExpectedVersion", ExpectedVersion);
                info.AddValue("CommandTypeFullName", CommandTypeFullName);
                info.AddValue("MethodName", MethodName);
            }
        }
    }
}