using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] InventoryItemData[] itemsData;
    [SerializeField] InventoryController controller;
    [SerializeField] Button newItemButton;
    [SerializeField] Button switchWeaponsButton;
    [SerializeField] Button openCloseButton;
    [SerializeField] TextMeshProUGUI eventLogger;
    [SerializeField] TextMeshProUGUI minorEventLogger;

    private void Awake() 
    {
        // BUTTONS

        newItemButton.onClick.AddListener(() => controller.AddItem(GetRandomItemData()));
        switchWeaponsButton.onClick.AddListener(() => controller.SwitchWeapons());
        openCloseButton.onClick.AddListener(() => controller.SwitchState());

        //  MAJOR
        
        InventoryEventsManager.OnItemTakenByCursor.AddListener((object sender, EventArgs args) => DebugEvent(
            $"{GetName(args)} has been taken from the world at {GetPos(args)}"), false);

        InventoryEventsManager.OnItemEquipped.AddListener((object sender, EventArgs args) => DebugEvent(
            $"{GetName(args)} has been equipped to {GetSlot(args)}"), false);

        InventoryEventsManager.OnItemUnequipped.AddListener((object sender, EventArgs args) => DebugEvent(
            $"{GetName(args)} has been unequipped from {GetSlot(args)}"), false);

        InventoryEventsManager.OnItemDroppedIntoWorld.AddListener((object sender, EventArgs args) => DebugEvent(
            $"{GetName(args)} has been dropped to the world at {GetPos(args)}"), false);

        InventoryEventsManager.OnImpossibleToProceed.AddListener((object sender, EventArgs args) => DebugEvent(
            $"I can't!"), false);

        // MINOR

        InventoryEventsManager.OnInventoryOpened.AddListener((object sender, EventArgs args) => DebugMinorEvent(
            $"Inventory has been opened"), false);

        InventoryEventsManager.OnInventoryClosed.AddListener((object sender, EventArgs args) => DebugMinorEvent(
            $"Inventory has been closed"), false);

        InventoryEventsManager.OnNewHighlight.AddListener((object sender, EventArgs args) => DebugMinorEvent(
            $"New highlight turned on"), false);

        InventoryEventsManager.OnHighlightOff.AddListener((object sender, EventArgs args) => DebugMinorEvent(
            $"Highlight turned off"), false);
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
