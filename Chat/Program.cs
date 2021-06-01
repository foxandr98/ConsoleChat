using System;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            ChooseApp();
        }
        static void ChooseApp()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Выберите приложение.\n1: Сервер\n2: Клиент");
            int numberOfApp = 0;
            while (!Int32.TryParse(Console.ReadLine(), out numberOfApp))
                Console.WriteLine("Укажите число!");
            switch (numberOfApp)
            {
                case 1:
                    ServerStart();
                    break;
                case 2:
                    ClientStart();
                    break;
                default:
                    Console.WriteLine("Такого приложения нет!");
                    break;
            }
        }

        static void ServerStart()
        {
            var server = new Server(ChoosePort());
            try
            {      
                server.ListenThread = new Thread(server.Listen);
                server.ListenThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                server.Disconnect();
                Console.ReadLine();
            }
        }
        static void ClientStart()
        {
            var client = new Client(ChoosePort());
            try
            {
                client.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Disconnect(ConsoleColor.Red);
            }

        }

        static int ChoosePort()
        {
            Console.WriteLine("Введите порт");
            int port;
            while (!Int32.TryParse(Console.ReadLine(), out port))
            {
                Console.WriteLine("Порт должен быть числом");
            }
            return port;
        }
    }
}