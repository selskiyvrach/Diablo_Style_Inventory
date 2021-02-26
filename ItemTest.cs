using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] WeaponData[] weapons;
    [SerializeField] InventoryUI inventory;
    [SerializeField] ItemFitType fitType;

    private static ItemTest _instance;

    private void Awake() {
        if(_instance != null)
            Destroy(gameObject);
        _instance = this;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(1))
            inventory.TryAddItemAuto(GetRandomItem());
    }

    public static Item GetRandomItem()
        => new Weapon(_instance.weapons[Random.Range(0, _instance.weapons.Length)]);
}
