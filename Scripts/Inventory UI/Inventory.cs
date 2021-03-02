using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] InventoryHighlighter highlighter;
    [SerializeField] InventoryItemDragger dragger;
    [SerializeField] ItemStorePanel[] panels;

    // TRACKERS
    private ItemStorePanel _currPanel = null;    
    private Vector3 _cursorPos => Input.mousePosition;

    private void Awake() {
        ForeachPanel((ItemStorePanel p) => p.Init(inventoryCanvas));
        highlighter.Initialize(inventoryCanvas);
    }

    private void Update() {
        ForeachPanel(CheckIfOverlappedByPointer);

        if(_currPanel != null)
            if(Input.GetMouseButtonDown(0))
                if(!dragger.Empty)
                {
                    if(_currPanel.CanPlaceItem(dragger.DraggedItem))
                    {
                        _currPanel.PlaceItem(dragger.DraggedItem, out InventoryItem replaced);
                        dragger.RemoveMouseFollower();
                        if(replaced != null)
                            dragger.AddMouseFollower(replaced, false);
                    }
                }
                else 
                {
                    if(_currPanel.PeekItem(_cursorPos, out InventoryItem item))
                    {
                        dragger.AddMouseFollower(item, false);
                        _currPanel.RemoveItem(item);
                    }
                }
    }

    private void CheckIfOverlappedByPointer(ItemStorePanel p) 
    {
        if(p.ContainsPoint(_cursorPos))
        {
            if(p != _currPanel)
            {
                _currPanel = p; 
                var rect = _currPanel.GetHighlightRect(_cursorPos);
                highlighter.NewHighlight(rect.center, rect.size, dragger.Empty ? false : !_currPanel.CanPlaceItem(dragger.DraggedItem));
            }
        }
        else 
            if(p == _currPanel)
            {
                _currPanel = null;
                highlighter.HideHighlight();
            }
    }  

    private void ForeachPanel(Action<ItemStorePanel> toDo)
    {
        foreach(var p in panels)
            toDo(p);
    }
}
