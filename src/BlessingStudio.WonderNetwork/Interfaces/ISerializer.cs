using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Interfaces
{
    public interface ISerializer
    {

    }
    public interface ISerializer<T> : ISerializer
    {
        public T Deserialize(byte[] data);
        public byte[] Serialize(T @object);
    }
}
