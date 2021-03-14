using System;
using UnityEngine;

public class InventoryItemDragger
{
    private Canvas _parentCanvas;
    private Vector2 _posOffset;
    private bool _withOffset;

    public InventoryItem DraggedItem { get; private set; }
    public bool Empty { get; private set; } = true;
    private Vector3 PointerPos => Input.mousePosition;

// CONSTRUCTOR

    public InventoryItemDragger(Canvas parentCanvas)
        => _parentCanvas = parentCanvas;

// PUBLIC

    public void PickUp(InventoryItem toDrag, bool withOffset)
    {
        if(!Empty)
            RemoveMouseFollower();
        
        if(_withOffset = withOffset)
            _posOffset = (Vector2)Input.mousePosition - DraggedItem.ScreenPos;
            
        DraggedItem = toDrag;
        DraggedItem.MoveOnTopOfViewSorting();
        Empty = false;
    } 

    public void Drop()
        => DropItemIntoWorld();

// PRIVATE

    public void UpdateDraggersCursor(Vector2 screenPos)
        => MoveItemAlongCursor(screenPos);

    private void MoveItemAlongCursor(Vector2 screenPos)
    {
        if (DraggedItem != null)
            DraggedItem.ScreenPos = _withOffset ? screenPos - _posOffset : screenPos;
    }

    private void DropItemIntoWorld()
    {
        if(Empty) return;
        DraggedItem.DisableInventoryViewOfItem();
        RemoveMouseFollower();
    }

    public void RemoveMouseFollower()
    {
        DraggedItem?.MoveInTheBackOfViewSorting();
        DraggedItem = null;
        Empty = true;
    }

}
