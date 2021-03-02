using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] InventoryItemData[] itemsData;
    [SerializeField] Inventory inventory;

    private static ItemTest _instance;

    private void Awake() {
        if(_instance != null)
            Destroy(gameObject);
        _instance = this;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(1))
            inventory.AddItemToCursor(GetRandomItem());
    }

    public static InventoryItem GetRandomItem()
        => new InventoryItem(_instance.itemsData[Random.Range(0, _instance.itemsData.Length)]);
}
