// ChatApp.cs
using System;

class Program
{
    static void Main()
    {
        using (var chatServer = new NetMQChat())
        using (var chatClient = new NetMQChat())
        {
            // Start the server
            chatServer.Start("tcp://127.0.0.1:5555");

            // Connect the client
            chatClient.Connect("tcp://127.0.0.1:5555", "tcp://127.0.0.1:5556");

            // Subscribe to the MessageReceived event
            chatServer.MessageReceived += (sender, args) =>
            {
                Console.WriteLine($"Server received: {args.Message}");
            };

            chatClient.MessageReceived += (sender, args) =>
            {
                Console.WriteLine($"Client received: {args.Message}");
            };

            // Send messages
            chatServer.SendMessage("Hello from server!");
            chatClient.SendMessage("Hello from client!");

            Console.ReadLine(); // Keep the application running

            // Dispose resources
            chatServer.Stop();
            chatClient.Disconnect();
        }
    }
}