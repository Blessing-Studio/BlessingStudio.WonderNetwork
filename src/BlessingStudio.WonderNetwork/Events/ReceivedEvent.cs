using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Events
{
    public class ReceivedEvent : IEvent
    {
        public Channel Channel { get; private set; }
        public IConnection Connection { get; private set; }
        public byte[] Data { get; private set; }
        public ReceivedEvent(Channel channel, IConnection connection, byte[] data)
        {
            Channel = channel;
            Connection = connection;
            Data = data;
        }
    }
}
