using System;
using UnityEngine;

public class UIItemDragger
{
    private Vector2 _posOffset;
    private InventoryUI _inventory;
    private bool _withOffset;

    public UIItem MouseFollower { get; private set; }
    public bool Empty { get; private set; } = true;

    public UIItemDragger(InventoryUI ui)
        => _inventory = ui;
    
    public void AddMouseFollower(UIItem toDrag, bool withOffset)
    {
        if(_withOffset = withOffset)
            _posOffset = Input.mousePosition - MouseFollower.transform.position;

        MouseFollower = toDrag;
        MouseFollower.gameObject.SetActive(true);
        Empty = false;
    } 

    public void ExternalUpdate()
    {
        if(MouseFollower != null)
        {
            MouseFollower.transform.position = _withOffset ? (Vector2)Input.mousePosition - _posOffset : (Vector2)Input.mousePosition;
            if(Input.GetMouseButtonDown(0))
            {
                _inventory.TryAddItemAtItsCurrPos(MouseFollower.TheItem, out bool notOverIventory, out bool CannotReplaceItems, out UIItem replaced);
                if(notOverIventory)
                    DropItemIntoWorld();
                else if(!CannotReplaceItems)
                {
                    ExtractMouseFollower(out UIItem newItem);
                    if(replaced != null)
                        AddMouseFollower(replaced, _withOffset);
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
            if(_inventory.TryExtractItemAtCursorPos(out UIItem newItem))
                AddMouseFollower(newItem, _withOffset);
    }

    private void DropItemIntoWorld()
    {
        Debug.Log($"Item {MouseFollower.TheItem.ItemData.Name} dropped into the world at {MouseFollower.transform.position} screen pos");
        MouseFollower.TheItem.HideUIItem();
        ExtractMouseFollower(out UIItem newItem);
    }

    public void ExtractMouseFollower(out UIItem item)
    {
        item = MouseFollower;
        MouseFollower = null;
        Empty = true;
    }
}
