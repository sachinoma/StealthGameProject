using System;

public class TerminalOperatedEventArgs : EventArgs
{
    public CardType Type { get; private set; }

    public TerminalOperatedEventArgs(CardType type)
    {
        Type = type;
    }
}