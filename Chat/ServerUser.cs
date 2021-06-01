using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    class ServerUser
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        private string UserName { get; set; }
        private readonly TcpClient client;
        private readonly Server server;

        public ServerUser(TcpClient tcpClient, Server server)
        {
            client = tcpClient;
            this.server = server; 
            Id = Guid.NewGuid().ToString();
            server.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                var message = GetMessage();
                UserName = message;

                message = UserName + " вошел в чат";
                server.BroadcastMessage(message, this.Id);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", UserName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        message = String.Format("{0}: покинул чат", UserName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            var data = new byte[100];
            var message = new StringBuilder();
            int numberOfBytes = 0;
            do
            {
                numberOfBytes = Stream.Read(data, 0, data.Length);
                message.Append(Encoding.Unicode.GetString(data, 0, numberOfBytes));
            }
            while (Stream.DataAvailable);

            return message.ToString();
        }

        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
