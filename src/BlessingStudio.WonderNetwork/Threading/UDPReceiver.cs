using BlessingStudio.WonderNetwork.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BlessingStudio.WonderNetwork.Threading
{
    public class UDPReceiver
    {
        public static int DefaultThreadCount = 4;
        public event Events.EventHandler<NewUDPConnectionEvent>? NewUDPConnection;
        private List<Thread> threads = new List<Thread>();
        private object threadLock = new object();
        private int reducingReadyed = 0;
        private object threadCountChangingLock = new();
        public int ThreadCount {  get { return threads.Count; } }
        public int buffersize { get; set; } = 4 * 1024;
        public Dictionary<IPEndPoint, UDPNetworkStream> NetworkStreams { get; private set; } = new();
        public bool ThreadCountReducing { get; private set; } = false;
        public Socket Socket { get; private set; }
        public UDPReceiver(Socket socket)
        {
            Socket = socket;
            Socket.Blocking = false;
            for (int i = 0; i < DefaultThreadCount; i++)
            {
                Thread thread = new(ReceivingThread);
                thread.Start(this);
                threads.Add(thread);
            }
        }
        public void SetThreadCount(int count)
        {
            if (count <= 0) throw new ArgumentException();
            lock (threadCountChangingLock)
            {
                if (count > ThreadCount)
                {
                    ThreadCountReducing = true;
                    while (true)
                    {
                        if (reducingReadyed == ThreadCount)
                        {
                            break;
                        }
                        Thread.Sleep(10);
                    }
                    while (ThreadCount == count)
                    {
                        threads.First().Abort();
                        threads.RemoveAt(0);
                    }
                    ThreadCountReducing = false;
                    reducingReadyed = 0;
                }
                else if (count < ThreadCount)
                {
                    while(ThreadCount == count)
                    {
                        Thread thread = new(ReceivingThread);
                        thread.Start(this);
                        threads.Add(thread);
                    }
                }
            }
        }
        private static void ReceivingThread(object? arg)
        {
            UDPReceiver receiver = (UDPReceiver)arg!;
            while(true)
            {
                if(receiver.ThreadCountReducing)
                {
                    receiver.reducingReadyed++;
                    while (receiver.ThreadCountReducing)
                    {
                        Thread.Sleep(10);
                    }
                }
                EndPoint endPoint = new IPEndPoint(new IPAddress(new byte[] {127, 0, 0, 1}), 1);
                byte[] bytes = new byte[receiver.buffersize];
                int count = 0;
                try
                {
                    count = receiver.Socket.ReceiveFrom(bytes, ref endPoint);
                }
                catch
                {
                    Thread.Sleep(10);
                    continue;
                }
                lock (receiver.threadLock)
                {
                    IPEndPoint iPEndPoint = (IPEndPoint)endPoint;
                    byte[] buffer = new byte[count];
                    MemoryStream memoryStream = new(bytes);
                    memoryStream.Read(buffer);
                    memoryStream.Close();
                    if (receiver.NetworkStreams.ContainsKey(iPEndPoint))
                    {
                        receiver.NetworkStreams[iPEndPoint].OnReceive(buffer);
                    }
                    else
                    {
                        receiver.NetworkStreams[iPEndPoint] = new(receiver.Socket, iPEndPoint);
                        if(receiver.NewUDPConnection != null)
                        {
                            receiver.NewUDPConnection(new(iPEndPoint, receiver.Socket));
                        }
                        receiver.NetworkStreams[iPEndPoint].OnReceive(buffer);
                    }
                }
            }
        }
    }
}
