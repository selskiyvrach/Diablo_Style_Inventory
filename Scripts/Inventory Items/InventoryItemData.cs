using UnityEngine;


[CreateAssetMenu(menuName="Scriptable Objects/Inventory/Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] Sprite sprite;
    [SerializeField] EquipmentFitType fitType;
    [SerializeField] Vector2IntSpaceData size;

    public string Name => itemName;
    public Sprite Sprite => sprite; 
    public EquipmentFitType FitType => fitType;
    public Vector2Int SizeInt => size.SizeInt;
}
