using System;

public class PrototypeMessageEvent
{
    public static event Action<string> MessageReceived = null;

    public static void Invoke(string message)
    {
        MessageReceived?.Invoke(message);
    }
}
