using System;
using System.Collections.Generic;
using System.Text;

namespace BlessingStudio.WonderNetwork.Utils
{
    public static class BufferUtils
    {
        public static bool CheckBufferArgs(byte[] buffer, int offset, int count)
        {
            return buffer.Length - offset >= count;
        }
    }
}
