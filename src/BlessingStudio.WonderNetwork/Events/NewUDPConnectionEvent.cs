﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BlessingStudio.WonderNetwork.Events
{
    public class NewUDPConnectionEvent : IEvent
    {
        public IPEndPoint IPEndPoint { get; set; }
        public Socket Socket { get; set; }
        public NewUDPConnectionEvent(IPEndPoint iPEndPoint, Socket socket)
        {
            IPEndPoint = iPEndPoint;
            Socket = socket;
        }
    }
}
