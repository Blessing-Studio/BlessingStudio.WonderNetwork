using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Events
{
    public delegate void EventHandler<in T>(T @event) where T : IEvent;
}
