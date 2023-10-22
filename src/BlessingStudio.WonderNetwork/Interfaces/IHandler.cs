using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Interfaces
{
    public interface IHandler
    {
        public void OnChannelRead(Channel channel, object @object);
        public void OnChannelWrite(Channel channel, object @object);
    }
}
