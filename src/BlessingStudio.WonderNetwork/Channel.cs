using BlessingStudio.WonderNetwork.Events;
using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BlessingStudio.WonderNetwork
{
    public class Channel
    {
        public IConnection Connection { get; private set; }
        public string ChannelName { get; private set; }

        public event Events.EventHandler<ReceivedBytesEvent>? ReceivedBytes;
        public event Events.EventHandler<ReceivedObjectEvent>? ReceivedObject;
        public static bool operator ==(Channel left, Channel right)
        {
            return left.Connection == right.Connection && left.ChannelName == right.ChannelName;
        }
        public static bool operator !=(Channel left, Channel right)
        {
            return !(left == right);
        }
        public override bool Equals(object? obj)
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
        public void Send<T>(T @object)
        {
            Connection.Send(ChannelName, @object);
        }
        public void OnReceive(byte[] data)
        {
            if(ReceivedBytes != null)
            {
                ReceivedBytes(new(this, Connection, data));
            }
        }
        public void OnReceive(object @object)
        {
            if(ReceivedObject != null)
            {
                ReceivedObject(new(this, Connection, @object));
            }
        }
    }
}
