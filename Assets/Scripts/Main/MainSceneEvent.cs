using System;

public class MainSceneEvent
{
    public int EventId { get; private set; }
    public event EventHandler Handler;

    public MainSceneEvent(int eventId)
    {
        EventId = eventId;
        Handler = null;
    }

    public void Invoke(object sender, EventArgs e)
    {
        Handler?.Invoke(sender, e);
    }
}
