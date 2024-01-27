using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Events;

public interface IEvent
{
    public string GetEventName()
    {
        return GetType().Name;
    }
}
