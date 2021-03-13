using System;
using UnityEngine;

public class InventoryItemDragger : MonoBehaviour
{
    [SerializeField] Canvas parentCanvas;
    private Vector2 _posOffset;
    private bool _withOffset;

    public InventoryItem DraggedItem { get; private set; }
    public bool Empty { get; private set; } = true;
    private Vector3 PointerPos => Input.mousePosition;

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

    private void Update()
        => MoveItemAlongCursor();

    private void MoveItemAlongCursor()
    {
        if (DraggedItem != null)
            DraggedItem.ScreenPos = _withOffset ? (Vector2)Input.mousePosition - _posOffset : (Vector2)Input.mousePosition;
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
