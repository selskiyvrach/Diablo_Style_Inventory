using System;
using UnityEngine;

public class InventoryItemEventArgs : EventArgs
{
    public InventoryItem Item { get; private set; }
    public ScreenSpaceItemContainer Container { get; private set; }
    public Vector2 ScreenPos { get; private set; }
    public string Message { get; private set; }
    
    public InventoryItemEventArgs(InventoryItem item, ScreenSpaceItemContainer container, Vector2 screenPos)
    {
        Item = item;
        Container = container;
        ScreenPos = screenPos;
    }

    public InventoryItemEventArgs(InventoryItem item, ScreenSpaceItemContainer container, Vector2 screenPos, string message) : this(item, container, screenPos)
        => Message = message;

}
