using System;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] InventoryHighlighter highlighter;
    [SerializeField] InventoryItemDragger dragger;
    [SerializeField] ItemStorageSpace itemStorage;
    [SerializeField] ItemStorePanel[] equipmentSlots;

    // TRACKERS
    private float _unitSize;
    private ItemStorePanel[] _allpanels;
    private ItemStorePanel _currPanel = null;    
    private Vector3 _cursorPos => Input.mousePosition;

    public void AddItemToCursor(InventoryItem item)
    {
        item.EnableInventoryViewOfItem(_unitSize, inventoryCanvas);
        if(dragger.Empty)
            dragger.AddMouseFollower(item, false);
    }

    private void Awake() {
        _allpanels = equipmentSlots.Concat(new ItemStorePanel[] { itemStorage }).ToArray();
        ForeachPanel((ItemStorePanel p) => p.Init(inventoryCanvas));
        _unitSize = itemStorage.UnitSize;
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
        // IF CURR CURSOR POS IS INSIDE PANEL'S AREA
        if(dragger.Empty ? 
            // CURSOR POS IF NOTHING DRAGGED
            p.ContainsPoint(_cursorPos) : 
            // ITEM'S TOP-LEFT AND BOTTOM-RIGHT CORNERS FOR WHEN ONE IS BEING DRAGGED
            p.ContainsItemCorners(dragger.DraggedItem))
        {
            // add: rescale dragged item

            if(p != _currPanel || (dragger.Empty ? _currPanel.NeedHighlightRecalculation(_cursorPos) : _currPanel.NeedHighlightRecalculation(dragger.DraggedItem)))
            {
                _currPanel = p;
                var rect = dragger.Empty ? _currPanel.GetHighlightRect(_cursorPos) : _currPanel.GetHighlightRect(dragger.DraggedItem);
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
        foreach(var p in _allpanels)
            toDo(p);
    }
}
