using UnityEngine;
using D2Inventory;

public class Test : MonoBehaviour
{
    [SerializeField] InventoryItemData[] data;
    [SerializeField] InventoryController controller;

    public void PickUpItem()
        => controller.PickUpItem(GetRandomData());

    private InventoryItemData GetRandomData()
        => data[UnityEngine.Random.Range(0, data.Length)];

}
