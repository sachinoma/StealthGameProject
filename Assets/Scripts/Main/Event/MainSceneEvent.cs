using System;

public class MainSceneEvent
{
    public string EventId { get; private set; }
    public event EventHandler Handler;

    public MainSceneEvent(string eventId)
    {
        EventId = eventId;
        Handler = null;
    }

    public void Invoke(object sender, EventArgs e)
    {
        Handler?.Invoke(sender, e);
    }
}
