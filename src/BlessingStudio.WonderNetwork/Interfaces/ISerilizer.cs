using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Interfaces
{
    public interface ISerilizer
    {

    }
    public interface ISerilizer<T> : ISerilizer
    {
        public T Deserialize(byte[] data);
        public byte[] Serialize(T @object);
    }
}
