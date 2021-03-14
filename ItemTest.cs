using UnityEngine;
using TMPro;
using System;

public class ItemTest : MonoBehaviour
{
    [SerializeField] InventoryItemData[] itemsData;
    [SerializeField] InventoryController inventory;
    [SerializeField] ContainersSwitcher switcher;
    [SerializeField] TextMeshProUGUI eventLogger;

    private void Awake() 
        => InventoryEventsManager.SubscribeToAll(DebugEvent, false);

    private void DebugEvent(object sender, EventArgs args)
    {
        
    }

    private void Update() {
        if(Input.GetMouseButtonDown(1))
            inventory.AddItem(GetRandomItem());
    }

    private InventoryItemData GetRandomItem()
        => itemsData[UnityEngine.Random.Range(0, itemsData.Length)];
}
