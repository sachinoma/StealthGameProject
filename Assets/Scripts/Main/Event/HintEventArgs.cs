using System;

public class HintEventArgs : EventArgs
{
    public HintType Type { get; private set; }
    public string HintText { get; private set; }

    public HintEventArgs(HintType hintType)
    {
        Type = hintType;
    }
}