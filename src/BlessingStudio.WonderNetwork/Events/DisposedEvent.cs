using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Events
{
    public class DisposedEvent : IEvent
    {
        public IConnection Connection { get; private set; }
        public DisposedEvent(IConnection connection)
        {
            Connection = connection;
        }
    }
}
