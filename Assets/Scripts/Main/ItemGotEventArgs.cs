using System;

public class ItemGotEventArgs : EventArgs
{
    public CardType Type { get; private set; }

    public ItemGotEventArgs(CardType type)
    {
        Type = type;
    }
}