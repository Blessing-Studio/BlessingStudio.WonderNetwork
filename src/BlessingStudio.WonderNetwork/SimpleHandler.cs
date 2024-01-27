using BlessingStudio.WonderNetwork.Events;
using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork;

public class SimpleHandler : IHandler
{
    public event Events.EventHandler<ReceivedBytesEvent>? ReceivedBytes;
    public event Events.EventHandler<ReceivedObjectEvent>? ReceivedObject;
    public event Events.EventHandler<ChannelCreatedEvent>? ChannelCreated;
    public event Events.EventHandler<ChannelDeletedEvent>? ChannelDeleted;
    public event Events.EventHandler<DisposedEvent>? Disposed;
    public void OnChannelCreated(ChannelCreatedEvent @event)
    {
        if(ChannelCreated != null)
        {
            ChannelCreated(@event);
        }
    }

    public void OnChannelDeleted(ChannelDeletedEvent @event)
    {
        if (ChannelDeleted != null)
        {
            ChannelDeleted(@event);
        }
    }

    public void OnDisposed(DisposedEvent @event)
    {
        if (Disposed != null)
        {
            Disposed(@event);
        }
    }

    public void OnReceivedBytes(ReceivedBytesEvent @event)
    {
        if (ReceivedBytes != null)
        {
            ReceivedBytes(@event);
        }
    }

    public void OnReceivedObject(ReceivedObjectEvent @event)
    {
        if (ReceivedObject != null)
        {
            ReceivedObject(@event);
        }
    }
}
