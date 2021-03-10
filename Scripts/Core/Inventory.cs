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
    [SerializeField] ContainersManager containersManager;

    private ScreenSpaceItemContainer _mainStorage; // set once in Awake()
    private ScreenSpaceItemContainer[] _activeEquipmentSlots => containersManager.GetActiveEquipmentSlots();
    private ScreenSpaceItemContainer[] _allActiveContainers => containersManager.GetAllActiveContainers();

    // TRACKERS
    private float _unitSize;
    private ScreenSpaceItemContainer _currContainer = null;
    private Vector3 _cursorPos => Input.mousePosition;

    public bool IsOn { get; private set; } 

    public void SetInventoryActive(bool value)
    {
        if(value == false)
            dragger.Drop();
        backgorund.gameObject.SetActive(IsOn = value);
        ForeachPanel(_allActiveContainers, (ScreenSpaceItemContainer p) => p.SetActive(IsOn));
        InventoryItem.SetInventoryItemsActive(value);
        highlighter.gameObject.SetActive(value);
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
            ForeachPanel(_activeEquipmentSlots, (ScreenSpaceItemContainer p) => { 
                if(!placed && p.Empty() && p.CanPlaceItemAuto(item))
                {
                    p.PlaceItem(item, out InventoryItem replaced); 
                    placed = true; 
                } 
            });
            if(!placed)
                placed = _mainStorage.TryPlaceItemAuto(item);
            if(!placed)
            {
                dragger.PickUp(item, false);
                dragger.Drop();
            }
        }
    }

    private void Awake() {
        _mainStorage = containersManager.GetMainStorage();
        InventoryItem.Init(inventoryCanvas);
        SetInventoryActive(IsOn);
        _unitSize = _mainStorage.UnitSize;
        highlighter.Initialize(inventoryCanvas);
    }

    private void Update() {
        if(!IsOn)
            return;

        ForeachPanel(_allActiveContainers, CheckIfOverlappedByPointer);

        if(Input.GetMouseButtonDown(0))
        {
            if(!dragger.Empty)
            {
                if(_currContainer != null)
                {
                    if(_currContainer.CanPlaceItem(dragger.DraggedItem))
                    {
                        _currContainer.PlaceItem(dragger.DraggedItem, out InventoryItem replaced);
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
                if(_currContainer != null && _currContainer.PeekItem(_cursorPos, out InventoryItem item))
                {
                    dragger.PickUp(item, false);
                    _currContainer.RemoveItem(item);
                }
            }
        }
    }

    private void CheckIfOverlappedByPointer(ScreenSpaceItemContainer c) 
    {
        // IF CURR CURSOR POS IS INSIDE PANEL'S AREA
        if(dragger.Empty ? 
            c.ContainsPoint(_cursorPos) :
            c == _mainStorage ? c.ContainsItemCorners(dragger.DraggedItem) : c.ContainsPoint(_cursorPos))
        {
            if(c != _currContainer || (dragger.Empty ? _currContainer.NeedHighlightRecalculation(_cursorPos) : _currContainer.NeedHighlightRecalculation(dragger.DraggedItem)))
            {
                _currContainer = c;
                var rect = dragger.Empty ? _currContainer.GetHighlightRect(_cursorPos) : _currContainer.GetHighlightRect(dragger.DraggedItem);
                highlighter.NewHighlight(rect.center, rect.size, dragger.Empty ? false : !_currContainer.CanPlaceItem(dragger.DraggedItem));
            }
        }
        else 
            if(c == _currContainer)
            {
                _currContainer = null;
                highlighter.HideHighlight();
            }
    }  

    private void ForeachPanel(ScreenSpaceItemContainer[] arr, Action<ScreenSpaceItemContainer> toDo)
    {
        foreach(var p in arr)
            toDo(p);
    }

}
