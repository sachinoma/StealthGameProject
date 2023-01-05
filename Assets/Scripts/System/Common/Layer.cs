using System;
using UnityEngine;

public static class Layer
{
    public const int Default                = 1 << 0;
    public const int TransparentFX          = 1 << 1;
    public const int IgnoreRaycast          = 1 << 2;

    public const int Water                  = 1 << 4;
    public const int UI                     = 1 << 5;
    public const int MiniMap                = 1 << 6;

    public const int Reactable              = 1 << 8;
    public const int ReactableDetector      = 1 << 9;
    public const int IgnoreDecal            = 1 << 10;


    public const int EnemySight             = Physics.DefaultRaycastLayers & ~ReactableDetector;
}