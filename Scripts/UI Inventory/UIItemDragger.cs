using System;
using UnityEngine;

public class UIItemDragger
{
    private Vector2 _posOffset;
    private InventoryUI _inventory;
    private bool _withOffset;

    public InventoryItem DraggedItem { get; private set; }
    public bool Empty { get; private set; } = true;

    public UIItemDragger(InventoryUI ui)
        => _inventory = ui;
    
    public void AddMouseFollower(InventoryItem toDrag, bool withOffset)
    {
        if(_withOffset = withOffset)
            _posOffset = (Vector2)Input.mousePosition - DraggedItem.ScreenPos;

        DraggedItem = toDrag;
        DraggedItem.EnableInventoryViewOfItem(_inventory.InventoryCanvas, _inventory.UnitSize);
        Empty = false;
    } 

    public void ExternalUpdate()
    {
        if(DraggedItem != null)
        {
            DraggedItem.ScreenPos = _withOffset ? (Vector2)Input.mousePosition - _posOffset : (Vector2)Input.mousePosition;
            if(Input.GetMouseButtonDown(0))
            {
                _inventory.TryAddItemAtItsCurrPos(DraggedItem, out bool notOverIventory, out bool CannotReplaceItems, out InventoryItem replaced);
                if(notOverIventory)
                    DropItemIntoWorld();
                else if(!CannotReplaceItems)
                {
                    ExtractMouseFollower(out InventoryItem newItem);
                    if(replaced != null)
                        AddMouseFollower(replaced, _withOffset);
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
            if(_inventory.TryExtractItemAtCursorPos(out InventoryItem newItem))
                AddMouseFollower(newItem, _withOffset);
    }

    private void DropItemIntoWorld()
    {
        Debug.Log($"Item {DraggedItem.ItemData.Name} dropped into the world at {DraggedItem.ScreenPos} screen pos");
        DraggedItem.DisableInventoryViewOfItem();
        ExtractMouseFollower(out InventoryItem newItem);
    }

    public void ExtractMouseFollower(out InventoryItem item)
    {
        item = DraggedItem;
        DraggedItem = null;
        Empty = true;
    }
}
