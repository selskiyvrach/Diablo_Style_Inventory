using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] InventoryItemData[] itemsData;
    [SerializeField] Inventory inventory;
    [SerializeField] ContainersSwitcher switcher;

    private static ItemTest _instance;

    private void Awake() {
        if(_instance != null)
            Destroy(gameObject);
        _instance = this;
        InventoryEventsManager.ClearAllEvents();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(1))
            inventory.AddItemAuto(GetRandomItem());
        if(Input.GetKeyDown(KeyCode.I))
            if(inventory.IsOn)
                inventory.SetInventoryActive(false);
            else 
                inventory.SetInventoryActive(true);
            
        if(Input.GetKeyDown(KeyCode.W))
            switcher.SetSecondOption();
        if(Input.GetKeyDown(KeyCode.E))
            switcher.SetFirstOption();
        
        if(Input.GetMouseButtonDown(0))
            inventory.PerformPrimaryInteraction();
    }

    public static InventoryItemData GetRandomItem()
        => _instance.itemsData[Random.Range(0, _instance.itemsData.Length)];
}
