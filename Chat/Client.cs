using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private readonly int port;
        public Client(int port)
        {
            this.port = port;
        }
        public void Start()
        {
            Console.Write("Введите свое имя: ");
            var clientProfile = new ClientProfile(Console.ReadLine());
            Console.Clear();
            client = new TcpClient();
            try
            {
                client.Connect("127.0.0.1", port);
                stream = client.GetStream();

                string helloMessage = clientProfile.UserName;
                byte[] data = Encoding.Unicode.GetBytes(helloMessage);
                stream.Write(data, 0, data.Length);

                var receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Добро пожаловать, {0}", clientProfile.UserName);
                Console.WriteLine("Вводите сообщения: ");
                Console.ForegroundColor = ConsoleColor.White;
                SendMessages();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect(ConsoleColor.Red);
            }
        }
        void SendMessages()
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
        void ReceiveMessages()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[100];
                    StringBuilder message = new StringBuilder();
                    int numberOfBytes = 0;
                    do
                    {
                        numberOfBytes = stream.Read(data, 0, data.Length);
                        message.Append(Encoding.Unicode.GetString(data, 0, numberOfBytes));
                    }
                    while (stream.DataAvailable);

                    string responseData = message.ToString();
                    if (responseData.Contains("покинул чат", StringComparison.OrdinalIgnoreCase))
                        WriteToConsole(responseData, ConsoleColor.Red);
                    else if (responseData.Contains("вошел в чат", StringComparison.OrdinalIgnoreCase))
                        WriteToConsole(responseData, ConsoleColor.Cyan);
                    else
                        WriteToConsole(responseData);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Подключение прервано!");
                    Disconnect(ConsoleColor.Red);
                }
            }
        }
        private void WriteToConsole(string response, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(response);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Disconnect(ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Console.WriteLine("Сеанс чата окончен!");
            Console.ReadLine();
        }
    }
}
