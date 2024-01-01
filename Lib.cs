// NetMQChatLibrary.cs
using System;
using NetMQ;
using NetMQ.Sockets;

public interface IMessageSource
{
    event EventHandler<MessageEventArgs> MessageReceived;
    void Start(string address);
    void Stop();
    void SendMessage(string message);
}

public interface IMessageSourceClient
{
    event EventHandler<MessageEventArgs> MessageReceived;
    void Connect(string serverAddress, string clientAddress);
    void Disconnect();
    void SendMessage(string message);
}

public class MessageEventArgs : EventArgs
{
    public string Message { get; }

    public MessageEventArgs(string message)
    {
        Message = message;
    }
}

public class NetMQChat : IMessageSource, IMessageSourceClient, IDisposable
{
    private readonly PublisherSocket publisher;
    private readonly SubscriberSocket subscriber;

    public event EventHandler<MessageEventArgs> MessageReceived;

    public NetMQChat()
    {
        publisher = new PublisherSocket();
        subscriber = new SubscriberSocket();
    }

    public void Start(string address)
    {
        publisher.Bind(address);
        Task.Factory.StartNew(ReceiveMessages);
    }

    public void Stop()
    {
        publisher.Dispose();
        subscriber.Dispose();
    }

    public void SendMessage(string message)
    {
        publisher.SendFrame(message);
    }

    public void Connect(string serverAddress, string clientAddress)
    {
        subscriber.Connect(serverAddress);
        subscriber.SubscribeToAnyTopic();
        subscriber.ReceiveReady += OnReceive;

        subscriber.Bind(clientAddress);
    }

    public void Disconnect()
    {
        subscriber.Dispose();
    }

    private void OnReceive(object sender, NetMQSocketEventArgs e)
    {
        string message = e.Socket.ReceiveFrameString();
        MessageReceived?.Invoke(this, new MessageEventArgs(message));
    }

    private void ReceiveMessages()
    {
        while (true)
        {
            string message = subscriber.ReceiveFrameString();
            MessageReceived?.Invoke(this, new MessageEventArgs(message));
        }
    }

    public void Dispose()
    {
        publisher.Dispose();
        subscriber.Dispose();
    }
}