using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] Image backgorund;
    [SerializeField] InventoryHighlighter highlighter;
    [SerializeField] InventoryItemDragger dragger;
    [SerializeField] ScreenSpaceItemContainer itemStorage;
    [SerializeField] ScreenSpaceItemContainer[] equipmentSlots;

    // TRACKERS
    private float _unitSize;
    private ScreenSpaceItemContainer[] _allpanels;
    private ScreenSpaceItemContainer _currPanel = null;    
    private Vector3 _cursorPos => Input.mousePosition;

    public bool IsOn { get; private set; } 

    public void SetInventoryActive(bool value)
    {
        if(value == false)
            dragger.Drop();
        backgorund.gameObject.SetActive(IsOn = value);
        ForeachPanel(_allpanels, (ScreenSpaceItemContainer p) => p.SetActive(IsOn));
        InventoryItem.SetInventoryItemsActive(value);
    }

    public void AddItemAuto(InventoryItem item)
    {
        item.EnableInventoryViewOfItem(_unitSize);

        if(IsOn)
        {
            if(!dragger.Empty)
                dragger.Drop();
            dragger.PickUp(item, false);
        }
        else 
        {
            bool placed = false;
            ForeachPanel(equipmentSlots, (ScreenSpaceItemContainer p) => { 
                if(!placed && p.Empty() && p.CanPlaceItem(item))
                {
                    p.PlaceItem(item, out InventoryItem replaced); 
                    placed = true; 
                } 
            });
            if(!placed)
                placed = itemStorage.TryPlaceItemAuto(item);
            if(!placed)
            {
                dragger.PickUp(item, false);
                dragger.Drop();
            }
        }
    }

    private void Awake() {
        InventoryItem.Init(inventoryCanvas);
        _allpanels = equipmentSlots.Concat(new ScreenSpaceItemContainer[] { itemStorage }).ToArray();
        SetInventoryActive(IsOn);
        _unitSize = itemStorage.UnitSize;
        highlighter.Initialize(inventoryCanvas);
    }

    private void Update() {
        if(!IsOn)
            return;

        ForeachPanel(_allpanels, CheckIfOverlappedByPointer);

        if(Input.GetMouseButtonDown(0))
        {
            if(!dragger.Empty)
            {
                if(_currPanel != null)
                {
                    if(_currPanel.CanPlaceItem(dragger.DraggedItem))
                    {
                        _currPanel.PlaceItem(dragger.DraggedItem, out InventoryItem replaced);
                        dragger.RemoveMouseFollower();
                        if(replaced != null)
                            dragger.PickUp(replaced, false);
                    }
                    else
                        Debug.Log(string.Format("Cannot place {0} in {1} slot", dragger.DraggedItem.ItemData.Name, "this"));
                }
                else
                    dragger.Drop();
            }
            else 
            {
                if(_currPanel != null && _currPanel.PeekItem(_cursorPos, out InventoryItem item))
                {
                    dragger.PickUp(item, false);
                    _currPanel.RemoveItem(item);
                }
            }
        }
    }

    private void CheckIfOverlappedByPointer(ScreenSpaceItemContainer c) 
    {
        // IF CURR CURSOR POS IS INSIDE PANEL'S AREA
        if(dragger.Empty ? 
            // CURSOR POS IF NOTHING DRAGGED
            c.ContainsPoint(_cursorPos) : 
            // ITEM'S TOP-LEFT AND BOTTOM-RIGHT CORNERS FOR WHEN ONE IS BEING DRAGGED
            c.ContainsItemCorners(dragger.DraggedItem))
        {
            if(c != _currPanel || (dragger.Empty ? _currPanel.NeedHighlightRecalculation(_cursorPos) : _currPanel.NeedHighlightRecalculation(dragger.DraggedItem)))
            {
                _currPanel = c;
                var rect = dragger.Empty ? _currPanel.GetHighlightRect(_cursorPos) : _currPanel.GetHighlightRect(dragger.DraggedItem);
                highlighter.NewHighlight(rect.center, rect.size, dragger.Empty ? false : !_currPanel.CanPlaceItem(dragger.DraggedItem));
            }
        }
        else 
            if(c == _currPanel)
            {
                _currPanel = null;
                highlighter.HideHighlight();
            }
    }  

    private void ForeachPanel(ScreenSpaceItemContainer[] arr, Action<ScreenSpaceItemContainer> toDo)
    {
        foreach(var p in arr)
            toDo(p);
    }

}
