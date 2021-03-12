using System;

[Flags]
public enum Manipulation
{
    Taken = 1,
    Released = 2,
    Dragged = 4,
    Dropped = 8,
    PutInContainer = 16,
    Impossible = 32
}