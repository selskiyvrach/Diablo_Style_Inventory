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

    public void AddMouseFollower(InventoryItem toDrag, bool withOffset)
    {
        if(!Empty)
            RemoveMouseFollower();
        
        if(_withOffset = withOffset)
            _posOffset = (Vector2)Input.mousePosition - DraggedItem.ScreenPos;

        DraggedItem = toDrag;
        DraggedItem.MoveOnTopOfViewSorting();
        Empty = false;
    } 

    private void Update()
    {
        if(DraggedItem != null)
            DraggedItem.ScreenPos = _withOffset ? (Vector2)Input.mousePosition - _posOffset : (Vector2)Input.mousePosition;
    }

// PRIVATE

    private void DropItemIntoWorld()
    {
        Debug.Log($"Item {DraggedItem.ItemData.Name} dropped into the world at {DraggedItem.ScreenPos} screen pos");
        DraggedItem.DisableInventoryViewOfItem();
        RemoveMouseFollower();
    }

    public void RemoveMouseFollower()
    {
        DraggedItem.MoveInTheBackOfViewSorting();
        DraggedItem = null;
        Empty = true;
    }

}
