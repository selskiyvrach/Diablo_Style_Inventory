using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] Image background;
    [SerializeField] InventoryHighlighter highlighter;
    [SerializeField] InventoryItemDragger dragger;
    [SerializeField] ContainersManager containersManager;
    
    // SET ONCE IN AWAKE
    private ScreenSpaceItemContainer _mainStorage; 
    // SET EACH TIME SINCE CONTAINS SWITCHABLE SLOTS
    private ScreenSpaceItemContainer[] _activeEquipmentSlots => containersManager.GetActiveEquipmentSlots();
    // SET EACH TIME SINCE CONTAINS SWITCHABLE SLOTS
    private ScreenSpaceItemContainer[] _allActiveContainers => containersManager.GetAllActiveContainers();

    // TRACKERS
    private float _unitSize;
    private ScreenSpaceItemContainer _currContainer = null;
    private Vector3 _cursorPos => Input.mousePosition;
    private InventoryItemEventArgs args;

    public bool IsOn { get; private set; } 

    public void SetInventoryActive(bool value)
    {
        if(value == false)
            dragger.Drop();
        background.gameObject.SetActive(IsOn = value);
        ForeachPanel(_allActiveContainers, (ScreenSpaceItemContainer p) => p.SetActive(IsOn));
        InventoryItem.SetInventoryItemsActive(value);
        highlighter.gameObject.SetActive(value);
        
        if(value)
            InventoryEventsManager.OnInventoryOpened.Invoke(this, null);
        else 
            InventoryEventsManager.OnInventoryClosed.Invoke(this, null);
    }

    public void AddItemAuto(InventoryItemData itemData)
    {
        InventoryItem item = InventoryItemFactory.GetInventoryItem(itemData, _unitSize);
        
        item.EnableInventoryViewOfItem();

        if(IsOn)
            TakeViaDragger(item);
        else
            TryPlaceToContainers(item);
    }

    private void TryPlaceToContainers(InventoryItem item)
    {
        bool placed = false;
        // TRY PLACE TO EQUIPMENT SLOTS IF EMPTY
        ForeachPanel(_activeEquipmentSlots, (ScreenSpaceItemContainer p) =>
        {
            if (!placed && p.Empty() && p.CanPlaceItemAuto(item))
            {
                p.PlaceItem(item, out InventoryItem replaced);
                placed = true;
            }
        });
        // TRY PLACE TO MAIN STORAGE
        if (!placed)
            placed = _mainStorage.TryPlaceItemAuto(item);
        // DROP BACK INTO WORLD
        if (!placed)
        {
            dragger.PickUp(item, false);
            dragger.Drop();
        }
    }

    private void TakeViaDragger(InventoryItem item)
    {
        if (!dragger.Empty)
            dragger.Drop();
        dragger.PickUp(item, false);
    }

    private void Awake()
        => InitializeStuff();

    private void Start() 
        => SetInventoryActive(IsOn);
    
    private void InitializeStuff()
    {
        _mainStorage = containersManager.GetMainStorage();
        _unitSize = _mainStorage.UnitSize;
        InventoryItem.Init(inventoryCanvas);
        highlighter.Initialize(inventoryCanvas);
    }


    private void Update()
    {
        if(IsOn) 
            ForeachPanel(_allActiveContainers, CheckIfPointerIsOverAndItsCurrPanel);
    }

    public void PerformPrimaryInteraction()
    {
        args = new InventoryItemEventArgs(dragger.DraggedItem, _currContainer, _cursorPos);

        if(!dragger.Empty)
            PutItemInContainerOrDrop();
        else 
            TryToRetrieveItemFromContainer();
    }

    private void TryToRetrieveItemFromContainer()
    {
        if (_currContainer != null && _currContainer.PeekItem(_cursorPos, out InventoryItem item))
        {
            dragger.PickUp(item, false);
            _currContainer.RemoveItem(item);
        }
    }

    private void PutItemInContainerOrDrop()
    {
        if(_currContainer != null)
        {
            if (_currContainer.CanPlaceItem(dragger.DraggedItem))
            {
                _currContainer.PlaceItem(dragger.DraggedItem, out InventoryItem replaced);
                
                if (replaced != null)
                {
                    var locArgs = new InventoryItemEventArgs(replaced, _currContainer, _cursorPos);

                    dragger.PickUp(replaced, false);
                    InventoryEventsManager.OnItemUnequipped.Invoke(this, locArgs);
                    InventoryEventsManager.OnItemTakenByCursor.Invoke(this, locArgs);
                }
                InventoryEventsManager.OnItemReleasedByCursor.Invoke(this, args);
                InventoryEventsManager.OnItemEquipped.Invoke(this, args);
                dragger.RemoveMouseFollower();
            }
            else
                InventoryEventsManager.OnImpossibleToProceed.Invoke(this, args);
        }
        else
        {
            InventoryEventsManager.OnItemDroppedIntoWorld.Invoke(this, args);
            dragger.Drop();
        }
    }

    private void CheckIfPointerIsOverAndItsCurrPanel(ScreenSpaceItemContainer c) 
    {
        // IF CURR CURSOR POS IS INSIDE PANEL'S AREA
        if(dragger.Empty ? 
            c.ContainsPoint(_cursorPos) :
            c == _mainStorage ? 
            c.ContainsItemCorners(dragger.DraggedItem) : 
            c.ContainsPoint(_cursorPos))
        {
            if(c != _currContainer || (dragger.Empty ? _currContainer.NeedHighlightRecalculation(_cursorPos) : _currContainer.NeedHighlightRecalculation(dragger.DraggedItem)))
            {
                Debug.Log("recalc");
                _currContainer = c;
                // NOTE: IMPORTANT TO FIRST CHECK IF CAN BE PLACED SINCE POTENTIALLY REPLACED ITEM WILL BE CASHED AND USED FURTHER IN DETERMINIG THE RECT
                var canPlace = dragger.Empty ? false : !_currContainer.CanPlaceItem(dragger.DraggedItem); 
                var rect = dragger.Empty ? _currContainer.GetHighlightRect(_cursorPos) : _currContainer.GetHighlightRect(dragger.DraggedItem);
                highlighter.NewHighlight(rect.center, rect.size, canPlace);
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
