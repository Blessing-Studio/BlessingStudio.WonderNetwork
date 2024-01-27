using BlessingStudio.WonderNetwork.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Interfaces;

public interface IHandler
{
    public void OnReceivedBytes(ReceivedBytesEvent @event);
    public void OnReceivedObject(ReceivedObjectEvent @event);
    public void OnChannelCreated(ChannelCreatedEvent @event);
    public void OnChannelDeleted(ChannelDeletedEvent @event);
    public void OnDisposed(DisposedEvent @event);
}
