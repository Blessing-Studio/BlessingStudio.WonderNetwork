﻿using BlessingStudio.WonderNetwork.Enums;
using BlessingStudio.WonderNetwork.Events;
using BlessingStudio.WonderNetwork.Extensions;
using BlessingStudio.WonderNetwork.Interfaces;
using BlessingStudio.WonderNetwork.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace BlessingStudio.WonderNetwork
{
    public class Connection : IConnection, IEnumerable<Channel>
    {
        private NetworkStream networkStream;
        private Thread ReceivingThread = new(new ParameterizedThreadStart(Listening));
        public object sendingLock = new object();
        private List<string> channels = new List<string>();
        public Dictionary<Type, ISerilizer> Serilizers { get; private set; } = new();
        public bool IsDisposed { get; private set; }
        public event Events.EventHandler<ReceivedBytesEvent>? ReceivedBytes;
        public event Events.EventHandler<ReceivedObjectEvent>? ReceivedObject;
        public event Events.EventHandler<ChannelCreatedEvent>? ChannelCreated;
        public Connection(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
            ReceivingThread.Start(this);
        }
        public void Send(string channelName, byte[] data)
        {
            CheckDisposed();
            if (channels.Contains(channelName))
            {
                lock (sendingLock)
                {
                    networkStream.WriteByte((byte)PacketType.SendChannelByteData);
                    networkStream.WriteString(channelName);
                    networkStream.WriteVarInt(data.Length);
                    networkStream.Write(data);
                }
            }
        }
        public async Task SendAsync(string channelName, byte[] data)
        {
            await Task.Run(() => Send(channelName, data));
        }
        public void Send<T>(string channelName, T data)
        {
            Type type = typeof(T);
            if (Serilizers.ContainsKey(type))
            {
                ISerilizer<T> serilizer = (ISerilizer<T>)Serilizers[type];
                byte[] buffer = serilizer.Serialize(data);
                lock (sendingLock)
                {
                    networkStream.WriteByte((byte)PacketType.SendChannelObjectData);
                    networkStream.WriteString(channelName);
                    networkStream.WriteString(type.FullName!);
                    networkStream.WriteVarInt(buffer.Length);
                    networkStream.Write(buffer);
                }
            }
            else
            {
                throw new InvalidOperationException("Serilizer Not Found");
            }
        }
        public Channel CreateChannel(string name)
        {
            CheckDisposed();
            if (channels.Contains(name))
            {
                return GetChannel(name);
            }
            lock (sendingLock)
            {
                networkStream.WriteByte((byte)PacketType.CreateChannel);
                channels.Add(name);
                if(ChannelCreated != null)
                {
                    ChannelCreated(new(GetChannel(name), this));
                }
                return GetChannel(name);
            }
        }

        public void DestroyChannel(string name)
        {
            CheckDisposed();
            if (channels.Contains(name))
            {
                lock (sendingLock)
                {
                    networkStream.WriteByte((byte)PacketType.DestroyChannel);
                    channels.Remove(name);
                }
            }
        }

        public Channel GetChannel(string name)
        {
            CheckDisposed();
            if (channels.Contains(name))
            {
                return new(this, name);
            }
            throw new InvalidOperationException("Not Found");
        }

        public IReadOnlyList<Channel> GetChannels()
        {
            CheckDisposed();
            List<Channel> channels = new List<Channel>();
            foreach (string channel in this.channels)
            {
                channels.Add(GetChannel(channel));
            }
            return channels;
        }
        public static void Listening(object? arg)
        {
            Connection connection = (Connection)arg!;
            NetworkStream networkStream = connection.networkStream;
            try
            {
            while (true)
                {
                    PacketType packetType = (PacketType)networkStream.ReadByte();
                    connection.CheckDisposed();
                    switch (packetType)
                    {
                        case PacketType.CreateChannel:
                            {
                                string name = networkStream.ReadString();
                                if (!connection.channels.Contains(name))
                                {
                                    connection.channels.Add(name);
                                    if(connection.ChannelCreated != null)
                                    {
                                        connection.ChannelCreated(new(connection.GetChannel(name), connection));
                                    }
                                }
                            }
                            break;
                        case PacketType.DestroyChannel:
                            {
                                string name = networkStream.ReadString();
                                if (connection.channels.Contains(name))
                                {
                                    connection.channels.Remove(name);
                                }
                            }
                            break;
                        case PacketType.SendChannelByteData:
                            {
                                string name = networkStream.ReadString();
                                int length = networkStream.ReadVarInt();
                                byte[] data = new byte[length];
                                networkStream.Read(data);
                                ReceivedBytesEvent @event = new(connection.GetChannel(name), connection, data);
                                if (connection.ReceivedBytes != null)
                                {
                                    connection.ReceivedBytes(@event);
                                }
                            }
                            break;
                        case PacketType.SendChannelObjectData:
                            {
                                string name = networkStream.ReadString();
                                string typeName = networkStream.ReadString();
                                int length = networkStream.ReadVarInt();
                                byte[] data = new byte[length];
                                networkStream.Read(data);
                                Type? type = Type.GetType(typeName);
                                if(type == null)
                                {
                                    break;
                                }
                                if(connection.Serilizers[type] != null)
                                {
                                    ISerilizer serilizer = connection.Serilizers[type];
                                    object @object = ReflectionUtils.Deserilize(type, serilizer, data);
                                    if(connection.ReceivedObject != null)
                                    {
                                        connection.ReceivedObject(new(connection.GetChannel(name), connection, @object));
                                    }
                                }
                            }
                            break;
                        default: break;
                    }
                }
            }
            catch(Exception ex)
            {
                connection.Dispose();
            }
        }

        public void Dispose()
        {
            networkStream.Dispose();
            IsDisposed = true;
        }
        private void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public IEnumerator<Channel> GetEnumerator()
        {
            return GetChannels().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetChannels().GetEnumerator();
        }
    }
}