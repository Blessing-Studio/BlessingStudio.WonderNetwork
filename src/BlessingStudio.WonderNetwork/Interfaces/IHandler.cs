using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Interfaces
{
    public interface IHandler<T>
    {
        public void OnChannelRead(Channel channel, T @object);
        public void OnChannelWrite(Channel channel, T @object);
    }
}
