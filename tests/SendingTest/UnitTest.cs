using BlessingStudio.WonderNetwork;
using System.Net.Sockets;

namespace SendingTest
{
    public class Tests
    {
        public Connection ServerConnection;
        public Connection ClientConnection;
        [SetUp]
        public void Setup()
        {
            TcpListener tcpListener = new(25301);
            tcpListener.Start();
            Task.Run(() =>
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect("localhost", 25301);
                ServerConnection = new(tcpClient.GetStream());
            });
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            ClientConnection = new(tcpClient.GetStream());
        }

        [Test]
        public void BytesSendingTest()
        {
            bool pass = false;
            byte[] bytes = { 1, 2, 3, 4 };

            ServerConnection.ReceivedBytes += (e) =>
            {
                if(e.Data == bytes)
                pass = true;
            };
            Channel channel = ClientConnection.CreateChannel("master");
            channel.Send(bytes);
            Thread.Sleep(5000);
            if (pass)
            {
                Assert.Pass();
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}