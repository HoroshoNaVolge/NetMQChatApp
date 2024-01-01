using System;

class Program
{
    static void Main()
    {
        using (var chatServer = new NetMQChat())
        using (var chatClient = new NetMQChat())
        {
            chatServer.Start("tcp://127.0.0.1:5555");

            chatClient.Connect("tcp://127.0.0.1:5555", "tcp://127.0.0.1:5556");

            chatServer.MessageReceived += (sender, args) =>
            {
                Console.WriteLine($"Server received: {args.Message}");
            };

            chatClient.MessageReceived += (sender, args) =>
            {
                Console.WriteLine($"Client received: {args.Message}");
            };

            chatServer.SendMessage("Hello from server!");
            chatClient.SendMessage("Hello from client!");

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            // Dispose resources
            chatServer.Stop();
            chatClient.Disconnect();
        }
    }
}