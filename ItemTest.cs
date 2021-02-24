using UnityEngine;

public class ItemTest : MonoBehaviour
{
    [SerializeField] WeaponData[] weapons;
    [SerializeField] InventoryUI inventory;

    private static ItemTest _instance;

    private void Awake() {
        if(_instance != null)
            Destroy(gameObject);
        _instance = this;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(1))
            inventory.AddItem(GetRandomItem());
    }

    public static Item GetRandomItem()
        => new Weapon(_instance.weapons[Random.Range(0, _instance.weapons.Length)]);
}
