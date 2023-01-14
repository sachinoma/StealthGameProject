using System;

public class PickUpEventArgs : EventArgs
{
    public CardType Type { get; private set; }

    public PickUpEventArgs(CardType type)
    {
        Type = type;
    }
}
