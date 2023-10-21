using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Events
{
    public class ReceivedObjectEvent : IEvent
    {
        public Channel Channel { get; private set; }
        public IConnection Connection { get; private set; }
        public object Object {  get; private set; }
        public ReceivedObjectEvent(Channel channel, IConnection connection, object @object)
        {
            Channel = channel;
            Connection = connection;
            Object = @object;
        }
    }
}
