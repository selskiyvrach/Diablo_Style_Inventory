using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using MNS.Utils.Values;
using D2Inventory;

public class Test : MonoBehaviour
{
    [SerializeField] InventoryItemData[] itemsData;
    [SerializeField] InventoryController controller;
    [SerializeField] Button newItemButton;
    [SerializeField] Button switchWeaponsButton;
    [SerializeField] Button openCloseButton;
    [SerializeField] TextMeshProUGUI eventLogger;
    [SerializeField] TextMeshProUGUI minorEventLogger;

    [Header("Controls")]
    [SerializeField] Vector2HandlerSource cursorPosChanged;
    [SerializeField] IntHandlerSource itemPickedUp;
    [SerializeField] IntHandlerSource draggedItemChanged;
    [SerializeField] IntHandlerSource itemDroppedToWorld;
    [SerializeField] BoolHandlerSource inventoryOpenClose;
    [SerializeField] ItemManager itemManager;
    [SerializeField] InventoryItemData itemData;

    private void Awake() 
    {
        itemPickedUp.Value.Invoke(this, -1);
        draggedItemChanged.Value.Invoke(this, -1);
        // // BUTTONS

        // newItemButton.onClick.AddListener(() => controller.AddItem(GetRandomItemData()));
        // switchWeaponsButton.onClick.AddListener(() => controller.SwitchWeapons());
        // openCloseButton.onClick.AddListener(() => controller.SwitchState());

        // //  MAJOR
        
        // InventoryEventsManager.OnItemTakenByCursor.AddListener((object sender, EventArgs args) => DebugEvent(
        //     $"{GetName(args)} has been taken from the world at {GetPos(args)}"), false);

        // InventoryEventsManager.OnItemEquipped.AddListener((object sender, EventArgs args) => DebugEvent(
        //     $"{GetName(args)} has been equipped to {GetSlot(args)}"), false);

        // InventoryEventsManager.OnItemUnequipped.AddListener((object sender, EventArgs args) => DebugEvent(
        //     $"{GetName(args)} has been unequipped from {GetSlot(args)}"), false);

        // InventoryEventsManager.OnItemDroppedIntoWorld.AddListener((object sender, EventArgs args) => DebugEvent(
        //     $"{GetName(args)} has been dropped to the world at {GetPos(args)}"), false);

        // InventoryEventsManager.OnImpossibleToProceed.AddListener((object sender, EventArgs args) => DebugEvent(
        //     $"I can't!"), false);

        // // MINOR

        // InventoryEventsManager.OnInventoryOpened.AddListener((object sender, EventArgs args) => DebugMinorEvent(
        //     $"Inventory has been opened"), false);

        // InventoryEventsManager.OnInventoryClosed.AddListener((object sender, EventArgs args) => DebugMinorEvent(
        //     $"Inventory has been closed"), false);

        // InventoryEventsManager.OnNewHighlight.AddListener((object sender, EventArgs args) => DebugMinorEvent(
        //     ((InventoryItemEventArgs)args).Item == null ? "New empty highlight" : $"Cursor hovered over {((InventoryItemEventArgs)args).Item.ItemData.Name}"), false);

        // InventoryEventsManager.OnHighlightOff.AddListener((object sender, EventArgs args) => DebugMinorEvent(
        //     $"Highlight turned off"), false);
    }

    private void Start() {
        itemManager.ItemData.cursorPos = new Vector2(400, 500);
        inventoryOpenClose.Value.Invoke(this, true);
        var firstItem = itemManager.CreateItem(itemData);
        itemManager.ItemData.VisibleOnScreen[firstItem] = true;
        itemPickedUp.Value.Invoke(this, firstItem);
        draggedItemChanged.Value.Invoke(this, firstItem);


        itemManager.ItemData.cursorPos = new Vector2(800, 350);
        var secondItem = itemManager.CreateItem(itemData);
        itemManager.ItemData.VisibleOnScreen[secondItem] = true;
        itemPickedUp.Value.Invoke(this, secondItem);
        draggedItemChanged.Value.Invoke(this, secondItem);
        itemDroppedToWorld.Value.Invoke(this, firstItem);
    }


    private void Update() {
        itemManager.ItemData.cursorPos = Input.mousePosition;
    }

    private string GetName(EventArgs args)
        => ((InventoryItemEventArgs)args).Item.ItemData.Name;
    
    private string GetPos(EventArgs args)
        => ((InventoryItemEventArgs)args).ScreenPos.ToString();

    private string GetSlot(EventArgs args)
        => ((InventoryItemEventArgs)args).Container.gameObject.name;

    private void DebugEvent(string message)
        => eventLogger.text += "\n" + message;

    private void DebugMinorEvent(string message)
        => minorEventLogger.text += "\n" + message;

    private InventoryItemData GetRandomItemData()
        => itemsData[UnityEngine.Random.Range(0, itemsData.Length)];
}
