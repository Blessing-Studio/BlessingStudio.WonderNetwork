using BlessingStudio.WonderNetwork.Enums;
using BlessingStudio.WonderNetwork.Events;
using BlessingStudio.WonderNetwork.Extensions;
using BlessingStudio.WonderNetwork.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace BlessingStudio.WonderNetwork
{
    public class ClientConnection : IConnection, IEnumerable<Channel>
    {
        private NetworkStream networkStream;
        private Thread ReceivingThread = new(new ParameterizedThreadStart(Listening));
        public object sendingLock = new object();
        private List<string> channels = new List<string>();
        public bool IsDisposed { get; private set; }
        public event Events.EventHandler<ReceivedEvent>? Received;
        public ClientConnection(NetworkStream networkStream)
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
                    networkStream.WriteByte((byte)PacketType.SendChannelData);
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
            ClientConnection clientConnection = (ClientConnection)arg!;
            NetworkStream networkStream = clientConnection.networkStream;
            while (true)
            {
                PacketType packetType = (PacketType)networkStream.ReadByte();
                clientConnection.CheckDisposed();
                switch (packetType)
                {
                    case PacketType.CreateChannel:
                        {
                            string name = networkStream.ReadString();
                            if (!clientConnection.channels.Contains(name))
                            {
                                clientConnection.channels.Add(name);
                            }
                        }
                        break;
                    case PacketType.DestroyChannel:
                        {
                            string name = networkStream.ReadString();
                            if (clientConnection.channels.Contains(name))
                            {
                                clientConnection.channels.Remove(name);
                            }
                        }
                        break;
                    case PacketType.SendChannelData:
                        {
                            string name = networkStream.ReadString();
                            int length = networkStream.ReadVarInt();
                            byte[] data = new byte[length];
                            networkStream.Read(data);
                            ReceivedEvent @event = new(clientConnection.GetChannel(name), clientConnection, data);
                            if (clientConnection.Received != null)
                            {
                                clientConnection.Received(@event);
                            }
                        }
                        break;
                    default: break;
                }
            }
        }

        public void Dispose()
        {
            networkStream.Dispose();
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
