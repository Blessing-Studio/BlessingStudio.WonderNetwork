﻿using System;
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
        public void DestroyChannel(string name);
        public void DestroyChannel(Channel channel)
        {
            DestroyChannel(channel.ChannelName);
        }
    }
}
