using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    class Server
    {
        public Thread ListenThread { get; set; }
        private static TcpListener tcpListener;
        private List<ServerUser> clients = new List<ServerUser>();
        private readonly int port;
        public Server(int port)
        {
            this.port = port;
        }
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                tcpListener.Start();
                Console.Clear();
                Console.WriteLine($"Сервер запущен. Ожидание подключений по порту {port} ...");
                while (true)
                {
                    var tcpClient = tcpListener.AcceptTcpClient();
                    var serverUser = new ServerUser(tcpClient, this);
                    var clientThread = new Thread(serverUser.Process);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }
        public void AddConnection(ServerUser serverUser)
        {
            clients.Add(serverUser);
        }
        public void RemoveConnection(string id)
        {
            var client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
        public void BroadcastMessage(string message, string id)
        {
            var data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id)
                    continue;
                clients[i].Stream.Write(data, 0, data.Length);
            }
        }
        public void Disconnect()
        {
            tcpListener.Stop();
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            Console.WriteLine("Сервер отключен!");
        }
    }
}