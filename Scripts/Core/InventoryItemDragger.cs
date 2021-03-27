using System;
using UnityEngine;

public class InventoryItemDragger
{
    private Canvas _parentCanvas;

    public InventoryItem DraggedItem { get; private set; }
    public bool Empty => DraggedItem == null;
    private Vector3 PointerPos => Input.mousePosition;

// CONSTRUCTOR

    public InventoryItemDragger(Canvas parentCanvas)
        => _parentCanvas = parentCanvas;

// PUBLIC

    public void PickUp(InventoryItem toDrag)
    {
        if(toDrag == null) return;

        if(!Empty)
            RemoveMouseFollower();
        
        DraggedItem = toDrag;
        DraggedItem.MoveOnTopOfViewSorting();
    } 

    public void Drop()
        => DropItemIntoWorld();

// PRIVATE

    public void UpdateDraggersCursor(Vector2 screenPos)
        => MoveItemAlongCursor(screenPos);

    private void MoveItemAlongCursor(Vector2 screenPos)
    {
        if (DraggedItem != null)
            DraggedItem.ScreenPos = screenPos;
    }

    private void DropItemIntoWorld()
    {
        if(Empty) return;
        DraggedItem.DisableInventoryViewOfItem();
        RemoveMouseFollower();
    }

    public InventoryItem ExtractItem()
    {
        var outt = DraggedItem;
        DraggedItem = null;
        return outt;   
    }

    public void RemoveMouseFollower()
    {
        DraggedItem?.MoveInTheBackOfViewSorting();
        DraggedItem = null;
    }

}
