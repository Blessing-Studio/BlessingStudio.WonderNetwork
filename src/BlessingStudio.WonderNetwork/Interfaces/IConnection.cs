using BlessingStudio.WonderNetwork.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Interfaces
{
    public interface IConnection : IDisposable
    {
        public Channel CreateChannel(string name);
        public Channel GetChannel(string name);
        public IReadOnlyList<Channel> GetChannels();
        public void Send(string channelName, byte[] data);
        public void Send<T>(string channelName, T @object);
        public void DestroyChannel(string name);
        public void DestroyChannel(Channel channel)
        {
            DestroyChannel(channel.ChannelName);
        }
        public void AddHandler(Events.EventHandler<ReceivedBytesEvent> handler);
        public void AddHandler(Events.EventHandler<ReceivedObjectEvent> handler);
    }
}
