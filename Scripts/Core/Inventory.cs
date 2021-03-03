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
    [SerializeField] ItemStorageSpace itemStorage;
    [SerializeField] ItemStorePanel[] equipmentSlots;

    // TRACKERS
    private float _unitSize;
    private ItemStorePanel[] _allpanels;
    private ItemStorePanel _currPanel = null;    
    private Vector3 _cursorPos => Input.mousePosition;

    public bool IsOn { get; private set; } 

    public void SetInventoryActive(bool value)
    {
        if(value == false)
            dragger.Drop();
        backgorund.gameObject.SetActive(IsOn = value);
        ForeachPanel(_allpanels, (ItemStorePanel p) => p.SetPanelActive(IsOn));
        InventoryItem.SetInventoryItemsActive(value);
    }

    public void AddItemAuto(InventoryItem item)
    {
        item.EnableInventoryViewOfItem(_unitSize, inventoryCanvas);

        if(IsOn)
        {
            if(!dragger.Empty)
                dragger.Drop();
            dragger.PickUp(item, false);
        }
        else 
        {
            bool placed = false;
            ForeachPanel(equipmentSlots, (ItemStorePanel p) => { 
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
        _allpanels = equipmentSlots.Concat(new ItemStorePanel[] { itemStorage }).ToArray();
        SetInventoryActive(IsOn);
        ForeachPanel(_allpanels, (ItemStorePanel p) => p.Init(inventoryCanvas));
        _unitSize = itemStorage.UnitSize;
        highlighter.Initialize(inventoryCanvas);
    }

    private void Update() {
        if(!IsOn)
            return;

        ForeachPanel(_allpanels, CheckIfOverlappedByPointer);

        if(Input.GetMouseButtonDown(0))
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
                        Debug.Log(string.Format("Cannot place {0} in {1} slot", dragger.DraggedItem.ItemData.Name, ((EquipmentSlot)_currPanel).FitType.FitTypeName));
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

    private void ForeachPanel(ItemStorePanel[] arr, Action<ItemStorePanel> toDo)
    {
        foreach(var p in arr)
            toDo(p);
    }

}
