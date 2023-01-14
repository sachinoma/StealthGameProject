using System;

public class PickUpEventArgs : EventArgs
{
    public PickUpItem.ItemType Type { get; private set; }

    public PickUpEventArgs(PickUpItem.ItemType type)
    {
        Type = type;
    }
}
