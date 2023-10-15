﻿using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace BlessingStudio.WonderNetwork
{
    public class Channel
    {
        public IConnection Connection { get; private set; }
        public string ChannelName { get; private set; }
        public static bool operator ==(Channel left, Channel right)
        {
            return left.Connection == right.Connection && left.ChannelName == right.ChannelName;
        }
        public static bool operator !=(Channel left, Channel right)
        {
            return !(left == right);
        }
        public override bool Equals(object obj)
        {
            if(obj is Channel other)
            {
                return this == other;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Connection.GetHashCode() + ChannelName.GetHashCode();
        }
        public Channel(IConnection connection, string channelName)
        {
            Connection = connection;
            ChannelName = channelName;
        }
        public void Send(byte[] data)
        {
            Connection.Send(ChannelName, data);
        }
    }
}