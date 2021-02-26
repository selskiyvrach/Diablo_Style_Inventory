using UnityEngine;


[CreateAssetMenu(menuName="Scriptable Objects/Item Data/Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] Sprite sprite;
    [SerializeField] Vector2Int size;

    public string Name => itemName;
    public Sprite Sprite => sprite; 
    public Vector2Int SizeInt => size;
}
